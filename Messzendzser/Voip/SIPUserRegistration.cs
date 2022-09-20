using SIPSorcery.SIP.App;
using SIPSorcery.SIP;

namespace Messzendzser.Voip
{
    public class SIPUserRegistration
    {
        public string DisplayName { get => Username; }
        public SIPEndPoint RemoteEndPoint { get; }
        public string Domain { get; }
        public string Username { get; }

        public string DestinationURI {get; }

        public SIPCallDescriptor SIPCallDescriptor { get => new SIPCallDescriptor(DestinationURI,null); }//return new SIPCallDescriptor($"sip:{Username}@{Domain}",null); } }
        public SIPUserRegistration(SIPEndPoint remoteEndPoint, string username,string domain,string destinationURI)
        {
            RemoteEndPoint = remoteEndPoint;
            Username = username;
            Domain = domain;
            DestinationURI = destinationURI;
        }

    }
}
