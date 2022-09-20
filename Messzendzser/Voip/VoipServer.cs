﻿using SIPSorcery.Media;
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
        private static ConcurrentDictionary<string, SIPUserAgent> _calls = new ConcurrentDictionary<string, SIPUserAgent>();

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

            Console.WriteLine($"Incoming call request: {localSIPEndPoint}<-{remoteEndPoint} {sipRequest.URI}.");
            int calledNumber = Convert.ToInt32(sipRequest.Header.To.ToURI.UserWithoutParameters);
            SIPUserRegistration called = null;
            try {
                called = DialLookup(calledNumber);
            }
            catch (ArgumentException)
            {
                SIPResponse notRegisteredResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.DoesNotExistAnywhere, null);                
                await _sipTransport.SendResponseAsync(notRegisteredResponse);
                return;
            }
            SIPResponse tryingResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Trying, null);
            await _sipTransport.SendResponseAsync(tryingResponse);
            await Task.Delay(500);
            SIPClientUserAgent ua = new SIPClientUserAgent(_sipTransport, called.RemoteEndPoint);
            ua.Call(called.SIPCallDescriptor);
            SIPResponse ringingResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ringing, null);
            await _sipTransport.SendResponseAsync(ringingResponse);
            await Task.Delay(500);
            SIPResponse okResponse = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
            await _sipTransport.SendResponseAsync(okResponse);
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
                    SIPClientUserAgent userAgent = new SIPClientUserAgent(_sipTransport, remoteEndPoint);
                    _registrations.TryAdd(sipRequest.Header.AuthenticationHeaders[0].SIPDigest.Username, new SIPUserRegistration(remoteEndPoint, sipRequest.Header.From.FromName,"",sipRequest.Header.From.FromURI.ToString()));
                    Console.WriteLine($"Voip: User {sipRequest.Header.AuthenticationHeaders[0].SIPDigest.Username} registered with id: {id}");
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
