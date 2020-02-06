using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;

namespace WitCom
{
    public class Wit : IDisposable
    {
        private static readonly SequencePosition ZeroPosition = new SequencePosition();

        //private const byte Start = 0x55;
        //private const byte P1 = 0x51;
        //private const byte P2 = 0x52;
        //private const byte P3 = 0x53;

        private static byte[] _seq1 = new byte[] { 0x55, 0x51 };
        private static byte[] _seq2 = new byte[] { 0x55, 0x52 };
        private static byte[] _seq3 = new byte[] { 0x55, 0x53 };


        private const int _packetLen = 11;
        private const int _packets = 3;
        private const int _minimumReadBytes = _packetLen * _packets;

        private SerialPort _serial;
        private Stopwatch _clock = new Stopwatch();

        private Pipe _pipe;
        private Task _writeTask;
        private Task _readTask;
        private ConcurrentQueue<WitFrame> _queue = new ConcurrentQueue<WitFrame>();
        private bool _isSync;
        private int _counter;
        private TaskCompletionSource<WitFrame> _completion = new TaskCompletionSource<WitFrame>();


        public Wit(string portName)
        {
            _serial = new SerialPort(portName);
            _serial.BaudRate = 115200;
            _serial.Parity = Parity.None;
            _serial.DataBits = 8;
            _serial.StopBits = StopBits.One;
            _serial.Handshake = Handshake.None;

            _pipe = new Pipe(new PipeOptions());
        }

        public Task Open()
        {
            if (_serial.IsOpen) return Task.CompletedTask;

            int i = 20;
            while (i > 0)
            {
                Thread.Sleep(50);
                try
                {
                    _serial.Open();
                }
                catch (Exception)
                {
                    Console.Write(".");
                    i--;
                }
            }

            Console.WriteLine();

            _clock.Reset();
            _clock.Start();

            _writeTask = _serial.BaseStream.CopyToAsync(_pipe.Writer);
            _readTask = ConsumeAsync(_pipe.Reader, Consume);

            return Task.WhenAll(_writeTask, _readTask);

            //_serial.Write(new byte[] { 0xFF, 0xAA, 0x52 }, 0, 3);
            //var buf = new Byte[10240];
            //_serial.Read(buf, 0, 1024);
        }

        public void Close()
        {
            if (!_serial.IsOpen) return;
            _serial.Close();
            _clock.Stop();
        }

        public void Dispose()
        {
            Close();
        }

        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames().OrderBy(f => f).ToArray();
        }

        private Task<SequencePosition> Consume(ReadOnlySequence<byte> data)
        {
            if (data.Length < _minimumReadBytes)
            {
                return Task.FromResult(ZeroPosition);
            }

            var buffer = new byte[_minimumReadBytes];
            buffer[0] = _seq1[0];
            buffer[1] = _seq1[1];
            var buf = buffer.AsSpan().Slice(2);
            var reader = new SequenceReader<byte>(data);
            while (!reader.End)
            {
                if (!_isSync)
                {
                    if (!reader.TryReadTo(out ReadOnlySequence<byte> _, _seq1.AsSpan(), true))
                    {
                        // remove useless data
                        reader.Advance(reader.Length);
                        return Task.FromResult(reader.Position);
                    }

                    _isSync = true;
                    return Task.FromResult(reader.Position);
                }

                if (reader.TryReadTo(out ReadOnlySequence<byte> sequence, _seq1.AsSpan(), true))
                {
                    sequence.CopyTo(buf);
                    _queue.Enqueue(new WitFrame(_clock.Elapsed, buffer));
                    return Task.FromResult(reader.Position);
                }

                Debug.WriteLine($"Wait {sequence.Length}");
                return Task.FromResult(reader.Position);
            }

            // serial connection has been closed
            Debug.WriteLine("End");
            return Task.FromResult(ZeroPosition);
        }

        public void Print()
        {
            while (true)
            {
                if (_queue.TryDequeue(out WitFrame frame))
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine($"Frame {_counter++} - Buffer {_queue.Count}    ");
                    Console.WriteLine(frame.ToString());
                    //Thread.Sleep(8);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private async Task ConsumeAsync(PipeReader reader,
            Func<ReadOnlySequence<byte>, Task<SequencePosition>> consumerFunc)
        {
            try
            {
                while (true)
                {
                    var read = await reader.ReadAsync();
                    var buffer = read.Buffer;
                    if (buffer.IsEmpty && read.IsCompleted)
                    {
                        break;
                    }

                    var consumed = await consumerFunc(buffer);
                    reader.AdvanceTo(consumed);
                }

                reader.Complete();
            }
            catch (Exception err)
            {
                reader.Complete(err);
            }
        }

    }
}
