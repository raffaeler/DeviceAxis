using System;

namespace FromArduino
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new GyroReader();
            var data = reader.ReadData(@"c:\temp\results.txt");

            foreach (var d in data)
            {
                Console.WriteLine(d);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
