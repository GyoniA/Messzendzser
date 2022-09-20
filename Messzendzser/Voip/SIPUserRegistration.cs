using SIPSorcery.SIP.App;
using System.Net;

namespace Messzendzser.Voip
{
    public class SIPUserRegistration
    {
        public IPEndPoint RemoteEndPoint { get; set; }
        public string Username { get; set; }

        public SIPCallDescriptor SIPCallDescriptor { get { return new SIPCallDescriptor($"sip:{Username}@{RemoteEndPoint}",null); } }
        public SIPUserRegistration(IPEndPoint remoteEndPoint, string username)
        {
            RemoteEndPoint = remoteEndPoint;
            Username = username;
        }

    }
}
