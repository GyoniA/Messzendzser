using SIPSorcery.SIP;

namespace Messzendzser.Voip
{
    public class VoipCallDescription
    {
        public class CallMember
        {
            public SIPEndPoint EndPoint { get; set; }
            public SIPURI SipUri { get; }

            public CallMember(SIPEndPoint endPoint)
            {
                EndPoint = endPoint;
                SipUri = new SIPURI(SIPSchemesEnum.sip, endPoint); ;
            }
        }
        public string CallId { get; set; }

        private List<CallMember> _members;
        public IReadOnlyList<CallMember> Members { get => _members; }

        public VoipCallDescription(string callId)
        {
            CallId = callId;
            _members = new List<CallMember>();
        }
        public void AddMember(CallMember member)
        {
            _members.Add(member);
        }
    }
}
