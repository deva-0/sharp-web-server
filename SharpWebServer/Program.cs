using System;

namespace SharpWebServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Server.Start();
            Console.ReadLine();
        }
    }
}