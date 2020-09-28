////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;

namespace TextTcpIp
{
    public class TcpIpListenerListen12EventArgs : EventArgs
    {
        public TcpIpListenerListen12EventArgs(ITcpListener tcpListener)
        {
            TcpListener = tcpListener;
        }

        public ITcpListener TcpListener { get; }
    }
}
