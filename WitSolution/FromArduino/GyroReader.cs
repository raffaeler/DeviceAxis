using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FromArduino
{
    public class GyroReader
    {
        public IList<GyroSensor> ReadData(string filename)
        {
            var lines = File.ReadAllLines(filename);
            var results = new List<GyroSensor>();
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length != 10) throw new Exception("Belin! Il formato è sbagliato");
                var gyroSensor = new GyroSensor();

                gyroSensor.Microsec = ParseInt(parts[0]);

                gyroSensor.AccX = ParseDouble(parts[1]);
                gyroSensor.AccY = ParseDouble(parts[2]);
                gyroSensor.AccZ = ParseDouble(parts[3]);

                gyroSensor.GyroX = ParseDouble(parts[4]);
                gyroSensor.GyroY = ParseDouble(parts[5]);
                gyroSensor.GyroZ = ParseDouble(parts[6]);

                gyroSensor.MagX = ParseDouble(parts[7]);
                gyroSensor.MagY = ParseDouble(parts[8]);
                gyroSensor.MagZ = ParseDouble(parts[9]);

                results.Add(gyroSensor);
            }

            return results;
        }

        private int ParseInt(string text)
        {
            return int.Parse(text);
        }

        private double ParseDouble(string text)
        {
            return double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
        }


    }
}
