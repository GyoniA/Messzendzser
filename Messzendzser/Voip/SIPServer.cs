//
// Author(s):
// Aaron Clauson (aaron@sipsorcery.com)
// 
// History: 
// 15 Nov 2016	Aaron Clauson	Created, Hobart, Australia.
// 13 Oct 2019  Aaron Clauson   Updated to use the SIPSorcery nuget package.
// 25 Jan 2020  Aaron Clauson   Converted from net452 to netcoreapp3.0.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Notes:
//
// A convenient tool to test SIP applications is [SIPp] (https://github.com/SIPp/sipp). The OPTIONS request handling can 
// be tested from Ubuntu or [WSL] (https://docs.microsoft.com/en-us/windows/wsl/install-win10) using the steps below.
//
// $ sudo apt install sip-tester
// $ wget https://raw.githubusercontent.com/saghul/sipp-scenarios/master/sipp_uac_options.xml
// $ sipp -sf sipp_uac_options.xml -max_socket 1 -r 1 -p 5062 -rp 1000 -trace_err 127.0.0.1
//
// To test registrations (note SIPp returns an error due to no 401 response, if this demo app registers the contact then
// it worked correctly):
//
// $ wget http://tomeko.net/other/sipp/scenarios/REGISTER_client.xml
// $ wget http://tomeko.net/other/sipp/scenarios/REGISTER_SUBSCRIBE_client.csv
// $ sipp 127.0.0.1 -sf REGISTER_client.xml -inf REGISTER_SUBSCRIBE_client.csv -m 1 -trace_msg -trace_err 
//-----------------------------------------------------------------------------
using LumiSoft.Net.SIP;

using LumiSoft.Net.SIP.Proxy;
using LumiSoft.Net.SIP.Stack;
using Messzendzser.Model.DB.Models;
using System.Net;
using WebSocketSharp;

namespace Messzendzser.Voip
{
    public class SIPServer
    {
        SIP_Proxy proxyCore;
        SIP_Stack stack;
        public SIPServer()
        {
            stack = new SIP_Stack();
            //stack.HostName = "192.168.0.104";
            stack.BindInfo = new LumiSoft.Net.IPBindInfo[] { 
                new LumiSoft.Net.IPBindInfo("192.168.0.104",LumiSoft.Net.BindInfoProtocol.TCP, IPAddress.Any, 5060) ,
                new LumiSoft.Net.IPBindInfo("192.168.0.104",LumiSoft.Net.BindInfoProtocol.UDP, IPAddress.Any, 5060) ,
            };
            proxyCore = new SIP_Proxy(stack);
            proxyCore.Registrar.CanRegister += Registrar_CanRegister;
            proxyCore.Authenticate += ProxyCore_Authenticate;
            proxyCore.ProxyMode = SIP_ProxyMode.Statefull | SIP_ProxyMode.Registrar;
            proxyCore.AddressExists += ProxyCore_AddressExists;
            stack.Start();
        }

        private bool ProxyCore_AddressExists(string address)
        {
            return true;
        }

        private void Stack_ValidateRequest(SIP_ValidateRequestEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
        }

        private void ProxyCore_Authenticate(SIP_AuthenticateEventArgs e)
        {
            Console.WriteLine("Authentication request: {0}, {1}", e.AuthContext.UserName, e.AuthContext.Password);
            VoipCredential voipCredential = VoipCredentialManager.GetVoipCredential(e.AuthContext.UserName);
            if (/*e.AuthContext.Password.IsNullOrEmpty() ||*/ voipCredential == null) { 
                e.Authenticated = false;
                return;
            }
            if (e.AuthContext.Authenticate(voipCredential.VoipUsername, voipCredential.VoipPassword))
            {
                e.Authenticated = true;
                Console.WriteLine("Successful registration: {0}", e.AuthContext.UserName);
                return;
            }
            else
            {
                e.Authenticated = false;
                Console.WriteLine("Unsuccessful registration: {0}", e.AuthContext.UserName);
                return;
            }
        }

        private bool Registrar_CanRegister(string userName, string address)
        {
            return true;
        }
    }

}
