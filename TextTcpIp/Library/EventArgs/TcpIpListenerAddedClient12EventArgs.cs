////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

namespace TextTcpIp
{
    public sealed class TcpIpListenerAddedClient12EventArgs : TcpIpClientConnect12EventArgs
    {
        public TcpIpListenerAddedClient12EventArgs(string tcpClientKey, ITcpClient tcpClient) : base(tcpClient)
        {
            TcpClientKey = tcpClientKey;
        }

        public string TcpClientKey { get; }
    }
}
