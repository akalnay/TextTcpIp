////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public interface ITcpClient
    {
        bool Connected { get; }
        Socket Client { get; set; }
        INetworkStream GetStream();
        Task ConnectAsync(IPAddress address, int port);
        void Close();
    }
}
