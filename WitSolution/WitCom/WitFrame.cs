using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WitCom
{
    public class WitFrame
    {
        private DateTime TimeStart = DateTime.Now;
        private double[] LastTime = new double[10];

        public WitFrame(TimeSpan clock, ReadOnlySpan<byte> block)
        {
            this.Clock = clock;
            Decode(block);
        }

        public TimeSpan Clock { get; }
        public Data LinearAcceleration { get; private set; }
        public Data AngularVelocity { get; private set; }
        public Data Angle { get; private set; }

        public void Decode(ReadOnlySpan<byte> block)
        {
            if (block.Length != 33)
            {
                Debug.WriteLine($"Bad length: {block.Length}");
                return;
            }

            if (block[0] != 0x55)
            {
                Debug.WriteLine($"Bad Start: {block[0]}");
                return;
            }

            if (block[1] != 0x51)
            {
                Debug.WriteLine($"Bad Header: {block[1]}");
                return;
            }

            var block1 = block.Slice(0, 11);
            var block2 = block.Slice(11, 11);
            var block3 = block.Slice(22, 11);

            SetLinearAcceleration(block1);
            SetAngularVelocity(block2);
            SetAngle(block3);
        }

        private void SetLinearAcceleration(ReadOnlySpan<byte> block)
        {
            var nums = ToDouble(block);
            var temperature = nums[3] / 340.0 + 36.25;
            nums[0] = nums[0] / 32768.0 * 16;
            nums[1] = nums[1] / 32768.0 * 16;
            nums[2] = nums[2] / 32768.0 * 16;
            LinearAcceleration = new Data(nums[0], nums[1], nums[2], temperature);
        }

        private void SetAngularVelocity(ReadOnlySpan<byte> block)
        {
            var nums = ToDouble(block);
            var temperature = nums[3] / 340.0 + 36.25;
            nums[0] = nums[0] / 32768.0 * 2000;
            nums[1] = nums[1] / 32768.0 * 2000;
            nums[2] = nums[2] / 32768.0 * 2000;
            AngularVelocity = new Data(nums[0], nums[1], nums[2], temperature);
        }

        private void SetAngle(ReadOnlySpan<byte> block)
        {
            var nums = ToDouble(block);
            var temperature = nums[3] / 340.0 + 36.25;
            nums[0] = nums[0] / 32768.0 * 180;
            nums[1] = nums[1] / 32768.0 * 180;
            nums[2] = nums[2] / 32768.0 * 180;
            Angle = new Data(nums[0], nums[1], nums[2], temperature);
        }

        private double[] ToDouble(ReadOnlySpan<byte> block)
        {
            double[] Data = new double[4];
            Data[0] = BitConverter.ToInt16(block.Slice(2));
            Data[1] = BitConverter.ToInt16(block.Slice(4));
            Data[2] = BitConverter.ToInt16(block.Slice(6));
            Data[3] = BitConverter.ToInt16(block.Slice(8));

            return Data;
        }

        public readonly struct Data
        {
            public Data(double x, double y, double z, double t)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.T = t;
            }

            public double X { get; }
            public double Y { get; }
            public double Z { get; }
            public double T { get; }

            public override string ToString()
            {
                return $"{X.ToString("f2")} g\r\n{Y.ToString("f2")} g\r\n{Z.ToString("f2")} g\r\n\r\n";
            }

            public string ToString(bool includeTemp)
            {
                if (!includeTemp) return ToString();
                return $"{X.ToString("f2")} g\r\n{Y.ToString("f2")} g\r\n{Z.ToString("f2")} g\r\n{T.ToString("f2")} °C\r\n\r\n";
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Clock.ToString("hh\\:mm\\:ss\\:fffff") + "\r\n");
            sb.Append(LinearAcceleration);
            sb.Append(AngularVelocity);
            sb.Append(Angle.ToString(true));
            return sb.ToString();
        }
    }
}
