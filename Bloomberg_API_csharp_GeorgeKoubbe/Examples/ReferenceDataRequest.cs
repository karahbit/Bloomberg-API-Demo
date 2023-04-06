using Bloomberglp.Blpapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Example
{
    public static class ReferenceDataRequest
    {
        public static void RunExample()
        {
            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = "localhost";
            sessionOptions.ServerPort = 8194;
            Session session = new Session(sessionOptions);
            session.Start();
            session.OpenService("//blp/refdata");
            Service referenceService = session.GetService("//blp/refdata");
            Request request = referenceService.CreateRequest("HistoricalDataRequest");

            Console.WriteLine("Reference Data Request\n");
            Console.WriteLine("Enter the security:");   // AAPL Equity
            string securityInput = Console.ReadLine();
            //Console.WriteLine("Enter Bloomberg field"); // PX_LAST
            //string field = Console.ReadLine();
            Console.WriteLine("Enter the start date in the format yyyymmdd:");
            string startDate = Console.ReadLine();
            Console.WriteLine("Enter the end date in the format yyyymmdd:");
            string endDate = Console.ReadLine();
            Console.WriteLine("\nResponse:\n");

            request.Append(new Name("securities"), securityInput);
            //request.Append(new Name("fields"), field);
            request.Append(new Name("fields"), "PX_LAST");
            request.Append(new Name("fields"), "BID");
            request.Append(new Name("fields"), "ASK");
            //request.Append(new Name("fields"), "TICKER");
            request.Set(new Name("startDate"), startDate);
            request.Set(new Name("endDate"), endDate);
            //request.Set(new Name("maxDataPoints"), 20);
            session.SendRequest(request, null);

            bool done = false;
            while (!done)
            {
                Event eventObject = session.NextEvent();
                if (eventObject.Type == Event.EventType.RESPONSE)
                {
                    foreach (Message msg in eventObject)
                    {
                        Console.WriteLine("correlationID = " + msg.CorrelationID);
                        Console.WriteLine("messageType = " + msg.MessageType);

                        Element elmSecurityData = msg["securityData"];
                        string security = elmSecurityData.GetElementAsString(new Name("security"));
                        Console.WriteLine(security + ":");

                        Element elmFieldDataArray = elmSecurityData[new Name("fieldData")];
                        for (int valueIndex = 0; valueIndex < elmFieldDataArray.NumValues; valueIndex++)
                        {
                            Element elmFieldData = elmFieldDataArray.GetValueAsElement(valueIndex);

                            string date = elmFieldData.GetElementAsString(new Name("date"));
                            double pxLast = elmFieldData.GetElementAsFloat64(new Name("PX_LAST"));
                            double bid = elmFieldData.GetElementAsFloat64(new Name("BID"));
                            double ask = elmFieldData.GetElementAsFloat64(new Name("ASK"));
                            Console.WriteLine("\tDATE = " + date);
                            Console.WriteLine("\tPX_LAST = " + pxLast.ToString());
                            Console.WriteLine("\tBID = " + bid.ToString());
                            Console.WriteLine("\tASK = " + ask.ToString() + "\n");
                        }
                    }
                    done = true;
                }
            }

            Console.WriteLine();
            session.Stop();
        }
    }
}