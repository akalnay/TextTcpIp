////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public sealed class TcpListenerProxy : ITcpListener
    {
        private readonly TcpListener _tcpListener;

        public TcpListenerProxy(IPAddress ipAddress, int port)
        {
            ThrowIf.ArgumentIsNull(ipAddress, nameof(ipAddress));
            _tcpListener = new TcpListener(ipAddress, port);
        }

        public async Task<ITcpClient> AcceptTcpClientAsync()
        {
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
            TaskCompletionSource<ITcpClient> tcs = new TaskCompletionSource<ITcpClient>(TaskCreationOptions.RunContinuationsAsynchronously);
            tcs.SetResult(new TcpClientProxy(tcpClient));
            return await tcs.Task.ConfigureAwait(false);
        }

        public void Start()
        {
            _tcpListener.Start();
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }
    }
}
