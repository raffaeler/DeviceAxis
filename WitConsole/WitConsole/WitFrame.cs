using System;
using System.Collections.Generic;
using System.Text;

namespace WitConsole
{
    public readonly struct WitFrame
    {
        private const int _packetLen = 11;
        private const int _packets = 3;
        private const int _minimumReadBytes = _packetLen * _packets;

        public byte[] Buffer { get; }
        public TimeSpan Clock { get; }

        public WitFrame(Span<byte> buffer, TimeSpan clock)
        {
            if (buffer.Length != _minimumReadBytes) throw new Exception("Wrong packet size");

            this.Buffer = new byte[_minimumReadBytes];
            buffer.CopyTo(Buffer);
            this.Clock = clock;
        }

        public WitFrame(byte[] buffer, TimeSpan clock)
        {
            if (buffer.Length != _minimumReadBytes) throw new Exception("Wrong packet size");

            Buffer = buffer;
            this.Clock = clock;
        }

    }
}
