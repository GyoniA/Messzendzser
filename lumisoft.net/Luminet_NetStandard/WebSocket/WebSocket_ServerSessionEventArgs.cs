using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.WebSoclet
{
    /// <summary>
    /// This class provides data to .... .
    /// </summary>
    public class WebSocket_ServerSessionEventArgs<T> : EventArgs where T : WebSocket_ServerSession,new()
    {
        private WebSocket_Server<T> m_pServer  = null;
        private T             m_pSession = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">TCP server.</param>
        /// <param name="session">TCP server session.</param>
        internal WebSocket_ServerSessionEventArgs(WebSocket_Server<T> server,T session)
        {
            m_pServer  = server;
            m_pSession = session;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets TCP server.
        /// </summary>
        public WebSocket_Server<T> Server
        {
            get{ return m_pServer; }
        }

        /// <summary>
        /// Gets TCP server session.
        /// </summary>
        public T Session
        {
            get{ return m_pSession; }
        }

        #endregion

    }
}
