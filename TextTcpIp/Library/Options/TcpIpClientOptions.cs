////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;

namespace TextTcpIp
{
    public sealed class TcpIpClientOptions : TcpOptionsBase
    {
        private const int _DEFAULTCLIENTIDENTIFIER = 0;
        private static readonly Func<ITcpClient> _DEFAULTGETTCPCLIENT = () => new TcpClientProxy();

        public override int Identifier { get; set; } = _DEFAULTCLIENTIDENTIFIER;
        public Func<ITcpClient> GetTcpClient { get; set; } = _DEFAULTGETTCPCLIENT;

        public Action<object, ITcpClient> OnConnecting { get; set; }
        public Action<object, ITcpClient> OnConnected { get; set; }
    }
}
