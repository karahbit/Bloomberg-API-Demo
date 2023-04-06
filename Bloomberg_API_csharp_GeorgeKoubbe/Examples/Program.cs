// Program.cs
// Example Bloomberg API C# Request/Reply Program
// George Koubbe
using System;
using Bloomberglp.Blpapi;

namespace Example
{
    class Program
    {
        static void Main()
        {
            bool exit = false;
            bool marketDataRequest = false;

            while (!exit)
            {
                if (!marketDataRequest)
                {
                    Console.WriteLine("####################################################");
                    Console.WriteLine("To send a market data request, push 1");
                    Console.WriteLine("To send a reference data request, push 2");
                    Console.WriteLine("To exit, push 3\n");
                }
                

                string input = Console.ReadLine();
                Console.WriteLine("\n\n");

                switch (input)
                {
                    case "1":
                        marketDataRequest = true;
                        MarketDataRequest.RunExample();
                        break;
                    case "2":
                        ReferenceDataRequest.RunExample();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
            
        }
    }
}