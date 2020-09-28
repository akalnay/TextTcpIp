////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;

namespace TextTcpIp
{
    public class TcpIpClientConnect12EventArgs : EventArgs
    {
        public TcpIpClientConnect12EventArgs(ITcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public ITcpClient TcpClient { get; }
    }
}
