////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public class TcpReceiver : ITcpReceiver
    {
        public TcpReceiver(ITcpIpNode tcpIpNode, int identifier, int bufferLength, char endOfLine, SynchronizationContext synchronizationContext,
                            Action<object, int, SynchronizationContext, string> onDataItemReceived, Action<object, byte[], int> onDataReceived)
        {
            ThrowIf.ArgumentIsNull(tcpIpNode, nameof(tcpIpNode));
            ThrowIf.ArgumentIsOutOfRange(bufferLength, bLength => bLength < 1, nameof(bufferLength));
            TcpIpNode = tcpIpNode;
            Identifier = identifier;
            BufferLength = bufferLength;
            EndOfLine = endOfLine;
            SynchronizationContext = synchronizationContext;
            if (onDataItemReceived != null)
                DataItemReceived += (o, e) => onDataItemReceived(o, e.Identifier, e.SynchronizationContext, e.Data);
            if (onDataReceived != null)
                DataReceived += (o, e) => onDataReceived(o, e.Buffer, e.BytesReceived);
        }

        public ITcpIpNode TcpIpNode { get; }

        public int Identifier { get; }
        public int BufferLength { get; }
        public char EndOfLine { get; }
        public SynchronizationContext SynchronizationContext { get; }

        public bool Disconnecting { get; set; }

        public event EventHandler<StreamData10EventArgs> DataItemReceived;
        public event EventHandler<BufferedDataReceivedEventArgs> DataReceived;

        private void OnDataReceived(byte[] buffer, int bytesReceived) => DataReceived?.Invoke(this, new BufferedDataReceivedEventArgs(buffer, bytesReceived));

        private void OnDataItemReceived(int identifier, SynchronizationContext synchronizationContext, string message)
        {
            DataItemReceived?.Invoke(this, new StreamData10EventArgs(identifier, synchronizationContext, message));
        }

        public async Task ReceiveAsync(ITcpClient client)
        {
            try
            {
                ThrowIf.ArgumentIsNull(client, nameof(client));
                byte[] buffer = new byte[BufferLength];
                INetworkStream networkStream = default;
                try
                {
                    networkStream = client.GetStream();
                }
                catch (Exception exception)
                {
                    if (!Disconnecting)
                        throw new TcpIpGetStreamFailedException(exception);
                }
                StreamBufferReader2 streamBufferReader = StreamBufferReader2.Create(Identifier, Encoding.ASCII,
                                                                                        Convert.ToByte(EndOfLine), SynchronizationContext,
                                                                                        OnDataItemReceived);
                await ReceiveAsync(networkStream, streamBufferReader, buffer).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (!Disconnecting)
                    throw new TcpIpReceiveFailedException(exception);
            }
        }

        private async Task ReceiveAsync(INetworkStream networkStream, StreamBufferReader2 streamBufferReader, byte[] buffer)
        {
            int bytesReceived = 0;
            do
            {
                try
                {
                    bytesReceived = await ReadAsync(networkStream, buffer).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (!Disconnecting)
                        throw new TcpIpInnerReceiveFailedException(exception);
                }
                if (bytesReceived > 0)
                {
                    OnDataReceived(buffer, bytesReceived);
                    streamBufferReader.GetStrings(buffer, bytesReceived);
                }
            } while (!Disconnecting && bytesReceived > 0);
        }

        public virtual async Task<int> ReadAsync(INetworkStream networkStream, byte[] buffer)
        {
            ThrowIf.ArgumentIsNull(networkStream, nameof(networkStream));
            ThrowIf.ArgumentIsNull(buffer, nameof(buffer));
            int bytesReceived = await networkStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            return bytesReceived;
        }
    }
}
