using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WitConsole.Helpers
{
    public class WitDecoder
    {
        private DateTime TimeStart = DateTime.Now;
        private double[] LastTime = new double[10];
        private double TimeElapse;
        private TimeSpan _clock;

        public WitDecoder(TimeSpan clock, ReadOnlySpan<byte> block)
        {
            _clock = clock;
            Decode(block);
        }

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

            //TimeElapse = (DateTime.Now - TimeStart).TotalMilliseconds / 1000;

            var block1 = block.Slice(0, 11);
            var block2 = block.Slice(11, 11);
            var block3 = block.Slice(22, 11);

            SetLinearAcceleration(block1);
            SetAngularVelocity(block2);
            SetAngle(block3);

            //???
            //if ((TimeElapse - LastTime[1]) < 0.1) return;
            //LastTime[1] = TimeElapse;
            //???
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
            Data[0] = BitConverter.ToInt16(block.Slice(2));  //(block, 2);
            Data[1] = BitConverter.ToInt16(block.Slice(4));  //(block, 4);
            Data[2] = BitConverter.ToInt16(block.Slice(6));  //(block, 6);
            Data[3] = BitConverter.ToInt16(block.Slice(8));  //(block, 8);

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
                return $"{X.ToString("f2")} g\r\n{Z.ToString("f2")} g\r\n{Z.ToString("f2")} g\r\n\r\n";
            }

            public string ToString(bool includeTemp)
            {
                if (!includeTemp) return ToString();
                return $"{X.ToString("f2")} g\r\n{Z.ToString("f2")} g\r\n{Z.ToString("f2")} g\r\n{T.ToString("f2")} °C\r\n\r\n";
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            //sb.Append(DateTime.Now.ToLongTimeString() + "\r\n");
            sb.Append(_clock.ToString("hh\\:mm\\:ss\\:fffff") + "\r\n");
            sb.Append(LinearAcceleration);
            sb.Append(AngularVelocity);
            sb.Append(Angle.ToString(true));
            var str = sb.ToString();
            //var str = DateTime.Now.ToLongTimeString() + "\r\n"
            //                + TimeElapse.ToString("f3") + "\r\n\r\n"
            //                + a[0].ToString("f2") + " g\r\n"
            //                + a[1].ToString("f2") + " g\r\n"
            //                + a[2].ToString("f2") + " g\r\n\r\n"
            //                + w[0].ToString("f2") + " deg/s\r\n"
            //                + w[1].ToString("f2") + " deg/s\r\n"
            //                + w[2].ToString("f2") + " deg/s\r\n\r\n"
            //                + Angle[0].ToString("f2") + " °\r\n"
            //                + Angle[1].ToString("f2") + " °\r\n"
            //                + Angle[2].ToString("f2") + " °\r\n\r\n"
            //                + h[0].ToString("f0") + " mG\r\n"
            //                + h[1].ToString("f0") + " mG\r\n"
            //                + h[2].ToString("f0") + " mG\r\n\r\n"
            //                + Temperature.ToString("f2") + " ℃\r\n"
            //                + Pressure.ToString("f0") + " Pa\r\n"
            //                + Altitude.ToString("f2") + " m\r\n\r\n"
            //                + (Longitude / 10000000).ToString("f0") + "°" + ((double)(Longitude % 10000000) / 1e5).ToString("f5") + "'\r\n"
            //                + (Latitude / 10000000).ToString("f0") + "°" + ((double)(Latitude % 10000000) / 1e5).ToString("f5") + "'\r\n"
            //                + GPSHeight.ToString("f1") + " m\r\n"
            //                + GPSYaw.ToString("f1") + " °\r\n"
            //                + GroundVelocity.ToString("f3") + " km/h";

            return str;
        }
    }
}
