////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Threading;

namespace TextTcpIp
{
    /// <summary>
    /// Reads data from a <see cref="Stream"/>.
    /// </summary>
    /// <remarks>
    /// The incoming data is separated by the delimiter specified in the class's <see cref="Create"/> method.  As the data is read,
    /// strings are constructed from the byte after the previous delimiter encountered (if any) to the byte prior to the next delimiter.
    /// A <see cref="NewString"/> event is raised whenever the delimiter is encountered and the constructed string can be retrieved
    /// from the <see cref="data"/> member of the event handler.  The delimiter is not part of the returned data.
    /// Bytes after the last delimiter in the stream are ignored.
    /// </remarks>
    public sealed class StreamBufferReader2
    {
        private string _remainder = String.Empty;

        private StreamBufferReader2(int identifier, Encoding encoding, byte delimiter, SynchronizationContext synchronizationContext)
        {
            ThrowIf.ArgumentDoesNotPassValidation(encoding, enc => enc.Equals(Encoding.ASCII), nameof(encoding));
            Identifier = identifier;
            Encoding = encoding;
            Delimiter = delimiter;
            SynchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// An integer streamIdentifier to associate the stream with.
        /// </summary>
        public int Identifier { get; }

        /// <summary>
        /// Data <see cref="Encoding"/> being used.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Determines boundaries for each string segment in the <see cref="Stream"/>.
        /// </summary>
        public byte Delimiter { get; }

        public SynchronizationContext SynchronizationContext { get; }

        private Action<int, SynchronizationContext, string> OnNewString { get; set; }

        /// <summary>
        /// Reads a buffer (a byte array) converts the buffer's bytes into strings.  The converted strings are returned via the <see cref="NewString"/> event.
        /// </summary>
        /// <param name="buffer">The buffer to read data from.</param>
        /// <param name="bytesToRead">The number of bytes to read from the buffer.</param>
        public void GetStrings(byte[] buffer, int bytesToRead)
        {
            ThrowIf.ArgumentIsNull(buffer, nameof(buffer));
            ThrowIf.ArgumentIsOutOfRange(bytesToRead, n => n < 1, nameof(bytesToRead));
            int indexBegin = 0;
            int indexEnd = Array.IndexOf(buffer, Delimiter, indexBegin, bytesToRead);
            if (indexEnd < 0)
                _remainder += Encoding.GetString(buffer, indexBegin, bytesToRead);
            else
            {
                do
                {
                    string s = Encoding.GetString(buffer, indexBegin, indexEnd - indexBegin);
                    string result;
                    if (_remainder.Length == 0)
                        result = s;
                    else
                    {
                        result = _remainder + s;
                        _remainder = String.Empty;
                    }
                    OnNewString(Identifier, SynchronizationContext, result);
                    indexBegin = indexEnd + 1;
                    indexEnd = Array.IndexOf(buffer, Delimiter, indexBegin, bytesToRead - indexBegin);
                } while (indexEnd > indexBegin);
                if (indexBegin > 0 && indexBegin < bytesToRead)
                    _remainder = Encoding.GetString(buffer, indexBegin, bytesToRead - indexBegin);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StreamBufferReader"/> class.
        /// </summary>
        /// <param name="encoding">Data <see cref="Encoding"/> used by the stream.</param>
        /// <param name="delimiter">Determines boundaries for each string segment in the <see cref="Stream"/></param>
        /// <param name="onNewString">Action to assign to the <see cref="NewString"/> event of the <see cref="StreamBufferReader"/> class.</param>
        /// <returns>A new instance of the <see cref="StreamBufferReader"/> class</returns>
        public static StreamBufferReader2 Create(Encoding encoding, byte delimiter, SynchronizationContext synchronizationContext,
            Action<int, SynchronizationContext, string> onNewString)
        {
            const int defaultStreamIdentifier = 0;
            return Create(defaultStreamIdentifier, encoding, delimiter, synchronizationContext, onNewString);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StreamBufferReader"/> class.
        /// </summary>
        /// <param name="identifier">An integer to associate the stream to.</param>
        /// <param name="encoding">Data <see cref="Encoding"/> used by the stream.</param>
        /// <param name="delimiter">Determines boundaries for each string segment in the <see cref="Stream"/></param>
        /// <param name="onNewString">Action to assign to the <see cref="NewString"/> event of the <see cref="StreamBufferReader"/> class.</param>
        /// <returns>A new instance of the <see cref="StreamBufferReader"/> class</returns>
        public static StreamBufferReader2 Create(int identifier, Encoding encoding, byte delimiter,
                                                    SynchronizationContext synchronizationContext,
                                                    Action<int, SynchronizationContext, string> onNewString)
        {
            ThrowIf.ArgumentIsNull(onNewString, nameof(onNewString));
            StreamBufferReader2 streamBufferReader = new StreamBufferReader2(identifier, encoding, delimiter, synchronizationContext)
            {
                OnNewString = onNewString
            };
            return streamBufferReader;
        }
    }
}
