using SIPSorcery.Media;
using SIPSorcery.Net;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorceryMedia.Abstractions;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Messzendzser.Voip
{
    public class VoipServer
    {
        private SIPTransport _sipTransport;

        struct SIPAccountRegistration
        {
            public string Username;

        }

        /// <summary>
        /// Keeps track of the current active calls. It includes both received and placed calls.
        /// </summary>
        private static ConcurrentDictionary<string, SIPUserAgent> _calls = new ConcurrentDictionary<string, SIPUserAgent>();

        /// <summary>
        /// Keeps track of the SIP account registrations.
        /// </summary>
        private static ConcurrentDictionary<string, SIPRegistrationUserAgent> _registrations = new ConcurrentDictionary<string, SIPRegistrationUserAgent>();

        public VoipServer(int sipPort)
        {
            Console.WriteLine("Starting Voip server on port: {0}", sipPort.ToString());

            // Set up a default SIP transport.
            _sipTransport = new SIPTransport();
            _sipTransport.AddSIPChannel(new SIPUDPChannel(new IPEndPoint(IPAddress.Any, sipPort)));
            _sipTransport.AddSIPChannel(new SIPTCPChannel(new IPEndPoint(IPAddress.Any, sipPort)));
            //var localhostCertificate = new X509Certificate2(SIPS_CERTIFICATE_PATH);
            //_sipTransport.AddSIPChannel(new SIPTLSChannel(localhostCertificate, new IPEndPoint(IPAddress.Any, sipsPort)));
            // TODO maybe add secure channel

            _sipTransport.EnableTraceLogs();

            _sipTransport.SIPTransportRequestReceived += OnRequest;
        }        

        private async Task OnRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest sipRequest)
        {
            //Console.WriteLine("Request received from: {0}", remoteEndPoint.ToString());
            try
            {
                if (sipRequest.Header.From != null &&
                sipRequest.Header.From.FromTag != null &&
                sipRequest.Header.To != null &&
                sipRequest.Header.To.ToTag != null)
                {
                    // This is an in-dialog request that will be handled directly by a user agent instance.
                }
                else if (sipRequest.Method == SIPMethodsEnum.INVITE)
                {
                    HandleInviteRequest(localSIPEndPoint, remoteEndPoint, sipRequest);
                }
                else if (sipRequest.Method == SIPMethodsEnum.BYE)
                {
                    SIPResponse byeResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.CallLegTransactionDoesNotExist, null);
                    await _sipTransport.SendResponseAsync(byeResponse);
                }
                else if (sipRequest.Method == SIPMethodsEnum.SUBSCRIBE)
                {
                    SIPResponse notAllowededResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.MethodNotAllowed, null);
                    await _sipTransport.SendResponseAsync(notAllowededResponse);
                }
                else if (sipRequest.Method == SIPMethodsEnum.REGISTER)
                {
                    HandleRegisterRequest(localSIPEndPoint,remoteEndPoint,sipRequest);
                }
                else if (sipRequest.Method == SIPMethodsEnum.OPTIONS)
                {
                    SIPResponse optionsResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                    await _sipTransport.SendResponseAsync(optionsResponse);
                }
            }
            catch (Exception reqExcp)
            {
                Console.WriteLine($"Exception handling {sipRequest.Method}. {reqExcp.Message}");
            }
        }
        private void OnHangup(SIPDialogue dialogue)
        {
            if (dialogue != null)
            {
                string callID = dialogue.CallId;
                if (_calls.ContainsKey(callID))
                {
                    if (_calls.TryRemove(callID, out var ua))
                    {
                        // This app only uses each SIP user agent once so here the agent is 
                        // explicitly closed to prevent is responding to any new SIP requests.
                        ua.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Example of how to create a basic RTP session object and hook up the event handlers.
        /// </summary>
        /// <param name="ua">The user agent the RTP session is being created for.</param>
        /// <param name="dst">THe destination specified on an incoming call. Can be used to
        /// set the audio source.</param>
        /// <returns>A new RTP session object.</returns>
        private VoIPMediaSession CreateRtpSession(SIPUserAgent ua, string dst)
        {
            List<AudioCodecsEnum> codecs = new List<AudioCodecsEnum> { AudioCodecsEnum.PCMU, AudioCodecsEnum.PCMA, AudioCodecsEnum.G722 };

            var audioSource = AudioSourcesEnum.SineWave;
            if (string.IsNullOrEmpty(dst) || !Enum.TryParse(dst, out audioSource))
            {
                audioSource = AudioSourcesEnum.Music;
            }

            Console.WriteLine($"RTP audio session source set to {audioSource}.");

            AudioExtrasSource audioExtrasSource = new AudioExtrasSource(new AudioEncoder(), new AudioSourceOptions { AudioSource = audioSource });
            audioExtrasSource.RestrictFormats(formats => codecs.Contains(formats.Codec));
            var rtpAudioSession = new VoIPMediaSession(new MediaEndPoints { AudioSource = audioExtrasSource });
            rtpAudioSession.AcceptRtpFromAny = true;

            // Wire up the event handler for RTP packets received from the remote party.
            rtpAudioSession.OnRtpPacketReceived += (ep, type, rtp) => OnRtpPacketReceived(ua, type, rtp);
            rtpAudioSession.OnTimeout += (mediaType) =>
            {
                if (ua?.Dialogue != null)
                {
                    Console.WriteLine($"RTP timeout on call with {ua.Dialogue.RemoteTarget}, hanging up.");
                }
                else
                {
                    Console.WriteLine($"RTP timeout on incomplete call, closing RTP session.");
                }

                ua.Hangup();
            };

            return rtpAudioSession;
        }


        private async void HandleInviteRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest sipRequest)
        {
            Console.WriteLine($"Incoming call request: {localSIPEndPoint}<-{remoteEndPoint} {sipRequest.URI}.");

            SIPUserAgent ua = new SIPUserAgent(_sipTransport, null);
            ua.OnCallHungup += OnHangup;
            /*ua.ServerCallCancelled += (uas) => Log.LogDebug("Incoming call cancelled by remote party.");
            ua.OnDtmfTone += (key, duration) => OnDtmfTone(ua, key, duration);
            ua.OnRtpEvent += (evt, hdr) => Log.LogDebug($"rtp event {evt.EventID}, duration {evt.Duration}, end of event {evt.EndOfEvent}, timestamp {hdr.Timestamp}, marker {hdr.MarkerBit}.");*/
            //ua.OnTransactionTraceMessage += (tx, msg) => Log.LogDebug($"uas tx {tx.TransactionId}: {msg}");
            ua.ServerCallRingTimeout += (uas) =>
            {
                Console.WriteLine($"Incoming call timed out in {uas.ClientTransaction.TransactionState} state waiting for client ACK, terminating.");
                ua.Hangup();
            };

            var uas = ua.AcceptCall(sipRequest);
            var rtpSession = CreateRtpSession(ua, sipRequest.URI.User);

            // Insert a brief delay to allow testing of the "Ringing" progress response.
            // Without the delay the call gets answered before it can be sent.
            await Task.Delay(500);

            await ua.Answer(uas, rtpSession);

            if (ua.IsCallActive)
            {
                await rtpSession.Start();
                _calls.TryAdd(ua.Dialogue.CallId, ua);
            }
        }

        private async void HandleRegisterRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest sipRequest)
        {
            if (!sipRequest.Header.HasAuthenticationHeader)
            {
                SIPResponse registerResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Unauthorised, null);
                registerResponse.Header.AuthenticationHeaders.Add(new SIPAuthenticationHeader(SIPAuthorisationHeadersEnum.WWWAuthenticate, "messzendzser", "asdf"));
                await _sipTransport.SendResponseAsync(registerResponse);
            }
            else
            {
                if (VoipCredentialManager.ValidateDigestResponse(sipRequest.Header.AuthenticationHeaders[0], "REGISTER"))
                {
                    Console.WriteLine($"Voip: User {sipRequest.Header.AuthenticationHeaders[0].SIPDigest.Username} registered");
                    SIPResponse optionsResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                    await _sipTransport.SendResponseAsync(optionsResponse);
                    //_registrations.TryAdd(sipRequest.Header.AuthenticationHeaders[0].SIPDigest.Username, new SIPRegistrationUserAgent(_sipTransport, sipRequest.Header.AuthenticationHeaders[0].SIPDigest.Username));
                }
                else
                {
                    Console.WriteLine("Voip: Unsuccessful registration");
                    SIPResponse optionsResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Unauthorised, null);
                    await _sipTransport.SendResponseAsync(optionsResponse);
                }
            }
        }


        /// <summary>
        /// Event handler for receiving RTP packets.
        /// </summary>
        /// <param name="ua">The SIP user agent associated with the RTP session.</param>
        /// <param name="type">The media type of the RTP packet (audio or video).</param>
        /// <param name="rtpPacket">The RTP packet received from the remote party.</param>
        private void OnRtpPacketReceived(SIPUserAgent ua, SDPMediaTypesEnum type, RTPPacket rtpPacket)
        {
            // The raw audio data is available in rtpPacket.Payload.
        }
    }
}
