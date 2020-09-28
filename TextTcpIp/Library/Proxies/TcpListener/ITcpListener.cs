////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System.Threading.Tasks;

namespace TextTcpIp
{
    public interface ITcpListener
    {
        Task<ITcpClient> AcceptTcpClientAsync();
        void Start();
        void Stop();
    }
}
