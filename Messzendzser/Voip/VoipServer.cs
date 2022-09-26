using SIPSorcery.Media;
using SIPSorcery.Net;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorceryMedia.Abstractions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Messzendzser.Voip
{
    public class VoipServer
    {
        private SIPTransport _sipTransport;

        /// <summary>
        /// Keeps track of the current active calls. It includes both received and placed calls.
        /// </summary>
        private static ConcurrentDictionary<string, VoipCallDescription> _calls = new ConcurrentDictionary<string, VoipCallDescription>();

        /// <summary>
        /// Keeps track of the SIP account registrations.
        /// </summary>
        private static ConcurrentDictionary<string, SIPUserRegistration> _registrations = new ConcurrentDictionary<string, SIPUserRegistration>();

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
        
        private SIPUserRegistration DialLookup(int id)
        {
            SIPUserRegistration? userAgent = null;
            if (id == 0)
                _registrations.TryGetValue("voip", out userAgent);
            else if (id == 1)
                _registrations.TryGetValue("voip1", out userAgent);
            if (userAgent == null)
                throw new ArgumentException("Id could not be found");
            return userAgent;
        }

        private async Task OnRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest sipRequest)
        {
            //Console.WriteLine("Packet {1} from: {0}", remoteEndPoint.ToString(),sipRequest.Method.ToString());
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
                else if(sipRequest.Method == SIPMethodsEnum.ACK)
                {
                    if(sipRequest.Header.CallId != null)
                    {
                        VoipCallDescription callDescription = null;
                        if (!_calls.TryGetValue(sipRequest.Header.CallId, out callDescription)) { 
                            // TODO call not found, throw exception
                        }                  
                        callDescription.Members.Where(m => !m.EndPoint.Equals(sipRequest.RemoteSIPEndPoint)).ToList().ForEach(x => {
                            SIPRequest request = new SIPRequest(SIPMethodsEnum.ACK, x.SipUri);
                            
                            _sipTransport.SendRequestAsync(sipRequest);
                        });

                    }
                }
            }
            catch (Exception reqExcp)
            {
                Console.WriteLine($"Exception handling {sipRequest.Method}. {reqExcp.Message}");
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
            /*if (string.IsNullOrEmpty(dst) || !Enum.TryParse(dst, out audioSource))
            {
                audioSource = AudioSourcesEnum.Music;
            }*/

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
            // TODO maybe validate calling user

            //Console.WriteLine($"Incoming call request: {localSIPEndPoint}<-{remoteEndPoint} {sipRequest.URI}.");
            int calledNumber = Convert.ToInt32(sipRequest.Header.To.ToURI.UserWithoutParameters);
            SIPUserRegistration? caller = null;
            SIPUserRegistration? called = null;
            try {
                called = DialLookup(calledNumber);
            }
            catch (ArgumentException)
            {
                SIPResponse notRegisteredResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.DoesNotExistAnywhere, null);                
                await _sipTransport.SendResponseAsync(notRegisteredResponse);
                return;
            }
            if(!_registrations.TryGetValue(sipRequest.Header.From.FromURI.User,out caller))
            {
                SIPResponse unauthotizedResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Unauthorised, null);
                await _sipTransport.SendResponseAsync(unauthotizedResponse);
                return;
            }

            VoipCallDescription callDescription = new VoipCallDescription(sipRequest.Header.CallId);
            callDescription.AddMember(new VoipCallDescription.CallMember(caller.RemoteEndPoint));
            callDescription.AddMember(new VoipCallDescription.CallMember(called.RemoteEndPoint));
            Console.WriteLine("Call registered with id: {0}", sipRequest.Header.CallId);
            _calls.TryAdd(sipRequest.Header.CallId, callDescription);
            SIPClientUserAgent ua = new SIPClientUserAgent(_sipTransport, called.RemoteEndPoint);
            SIPCallDescriptor descriptor = called.SIPCallDescriptor;            
            descriptor.SetGeneralFromHeaderFields(caller.DisplayName, caller.Username, caller.Domain);
            descriptor.CallId = sipRequest.Header.CallId;
            ua.CallTrying += async (uac, resp) =>
            {
                SIPResponse tryingResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Trying, null);
                await _sipTransport.SendResponseAsync(tryingResponse);
            };
            ua.CallRinging += async (uac, resp) =>
            {
                SIPResponse ringingResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ringing, null);
                await _sipTransport.SendResponseAsync(ringingResponse);
            };
            ua.CallAnswered += async (uac, resp) => 
            {
                SIPResponse okResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                await _sipTransport.SendResponseAsync(okResponse);
            };
            ua.CallFailed += async (uac,errorMessage,response) =>
            {
                //TODO handle
                SIPResponse failedResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.RequestTimeout, null);
                await _sipTransport.SendResponseAsync(failedResponse);
            };
            ua.Call(descriptor);            
            
            //SIPRequest request = caller.Call(new SIPCallDescriptor("sip:voip@192.168.0.104", null));
            return;
            /*var rtpSession = CreateRtpSession(called, sipRequest.URI.User);
            var uas1 = called.AcceptCall(sipRequest);
            await Task.Delay(500);
            await called.Answer(uas1, rtpSession);*/
            //await called.Call(new SIPCallDescriptor(caller.ContactURI.ToString(), null), rtpSession);
            
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
                    SIPResponse optionsResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                    await _sipTransport.SendResponseAsync(optionsResponse);
                    int id = _registrations.Count;
                    _registrations.TryAdd(sipRequest.Header.From.FromURI.User, new SIPUserRegistration(remoteEndPoint, sipRequest.Header.From.FromURI.User,sipRequest.Header.From.FromURI.Host,sipRequest.Header.From.FromURI.ToString()));
                    Console.WriteLine($"Voip: User {sipRequest.Header.From.FromURI.User} registered with id: {id}");
                    await Task.Delay(2000);
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
