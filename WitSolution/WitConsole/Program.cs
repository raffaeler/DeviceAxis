﻿using System;
using System.Text;
using System.Threading.Tasks;
using WitCom;

namespace WitConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var wit = new Wit("COM3");

            Task.Run(() => wit.Print());

            await wit.Open();
            await wit.Start();

            Console.ReadKey();
        }

    }
}
