////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;

namespace TextTcpIp
{
    public sealed class BufferedDataReceivedEventArgs : EventArgs
    {
        public BufferedDataReceivedEventArgs(byte[] buffer, int bytesReceived)
        {
            Buffer        = buffer;
            BytesReceived = bytesReceived;
        }

        public byte[] Buffer { get; }

        public int BytesReceived { get; }
    }
}
