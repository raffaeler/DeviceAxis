﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;

namespace WitConsole
{
    public class ConsumerData
    {
        public ConsumerData()
        {
            Alloc = x => ArrayPool<byte>.Shared.Rent(x).AsMemory(0, x);
            Free = m =>
            {
                if (!MemoryMarshal.TryGetArray(m, out ArraySegment<byte> arraySegment))
                {
                    return;
                }

                ArrayPool<byte>.Shared.Return(arraySegment.Array);
            };
        }

        /// <summary>
        /// The reader, origin of the transform
        /// </summary>
        internal PipeReader Reader { get; set; }

        /// <summary>
        /// </summary>
        public Func<int, Memory<byte>> Alloc { get; internal set; }

        /// <summary>
        /// </summary>
        public Action<Memory<byte>> Free { get; internal set; }

        /// <summary>
        /// The memory read from the reader
        /// </summary>
        public ReadOnlySequence<byte> Memory { get; internal set; }

        /// <summary>
        /// The amount of bytes consumed from Memory
        /// </summary>
        public SequencePosition Consumed { get; set; }

        /// <summary>
        /// Tells whether the reader has completed (no more data will come)
        /// </summary>
        public bool IsCompleted { get; internal set; }

    }
}
