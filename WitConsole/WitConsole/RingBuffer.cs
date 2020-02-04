using System;
using System.Collections.Generic;
using System.Text;

namespace WitConsole
{
    public class RingBuffer
    {
        private byte[] _buffer;
        private int _maxPacketSize;

        public RingBuffer(int capacity, int maxPacketSize)
        {
            _buffer = new byte[capacity];
            _maxPacketSize = maxPacketSize;
        }

        public byte this[int index]
        {
            get => _buffer[index];
            set => _buffer[index] = value;
        }

        public byte[] this[Range range] => _buffer[range];

        

    }
}
