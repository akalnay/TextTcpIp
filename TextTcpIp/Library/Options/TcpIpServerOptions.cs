////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Net;

namespace TextTcpIp
{
    public sealed class TcpIpServerOptions : TcpOptionsBase
    {
        private const int _DEFAULTSERVERIDENTIFIER = int.MinValue;
        private static readonly Func<IPAddress, int, ITcpListener> _DEFAULTGETTCPLISTENER = (ipAddress, port) => new TcpListenerProxy(ipAddress, port);

        public override int Identifier { get; set; } = _DEFAULTSERVERIDENTIFIER;
        public Func<IPAddress, int, ITcpListener> GetTcpListener { get; set; } = _DEFAULTGETTCPLISTENER;

        public Action<object, ITcpListener> OnListeningStarting { get; set; }

        public Action<object, ITcpListener> OnListeningStarted { get; set; }

        public Action<object, string, ITcpClient> OnClientAdded { get; set; }
    }
}
