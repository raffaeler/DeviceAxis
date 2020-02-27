using System;
using System.Collections.Generic;
using System.Text;

namespace FromArduino
{
    public class GyroSensor
    {
        private static char _sep = '\t';

        public int Microsec { get; set; }

        public double AccX { get; set; }
        public double AccY { get; set; }
        public double AccZ { get; set; }


        public double GyroX { get; set; }
        public double GyroY { get; set; }
        public double GyroZ { get; set; }

        public double MagX { get; set; }
        public double MagY { get; set; }
        public double MagZ { get; set; }

        public override string ToString()
        {
            return $"Acc:{AccX}{_sep}{AccY}{_sep}{AccZ} Gyro:{GyroX}{_sep}{GyroY}{_sep}{GyroZ} Mag: {MagX}{_sep}{MagY}{_sep}{MagZ} - {Microsec}";
        }
    }

}
