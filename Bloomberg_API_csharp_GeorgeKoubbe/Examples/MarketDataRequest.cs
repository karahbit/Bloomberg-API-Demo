using Bloomberglp.Blpapi;
using EventHandler = Bloomberglp.Blpapi.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Example
{
    public static class MarketDataRequest
    {
        //private static IDictionary<long, string> correlationIDs = new Dictionary<long, string>(){
        //    {1, "SPY US EQUITY"},
        //};
        private static List<string> _fields;
        private static string security;
        public static void RunExample()
        {
            MarketDataRequest._fields = new List<String>();
            MarketDataRequest._fields.Add("BID");
            MarketDataRequest._fields.Add("ASK");

            Console.WriteLine("Market Data Request\n");
            Console.WriteLine("Enter the security:");   // AAPL Equity
            MarketDataRequest.security = Console.ReadLine();

            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = "localhost";
            sessionOptions.ServerPort = 8194;
            Session session = new Session(sessionOptions, new EventHandler(MarketDataRequest.ProcessEvent));
            session.StartAsync();
        }

        private static void ProcessEvent(Event evt, Session session)
        {
            switch (evt.Type)
            {
                case Event.EventType.SESSION_STATUS:
                    foreach (Message message in evt.GetMessages())
                    {
                        if (message.MessageType.Equals("SessionStarted"))
                        {
                            try
                            {
                                session.OpenServiceAsync("//blp/mktdata", new CorrelationID(-9999));
                            }
                            catch (Exception)
                            {
                                Console.Error.WriteLine("Could not open //blp/mktdata for async");
                            }
                        }
                    }
                    break;

                case Event.EventType.SERVICE_STATUS:
                    List<Subscription> slist = new List<Subscription>();
                    slist.Add(new Subscription(MarketDataRequest.security, MarketDataRequest._fields));
                    //foreach (KeyValuePair<long, string> cid in correlationIDs)
                    //    slist.Add(new Subscription(cid.Value, MarketDataRequest._fields, new CorrelationID(cid.Key)));
                    session.Subscribe(slist);
                    break;

                case Event.EventType.SUBSCRIPTION_DATA:
                case Event.EventType.RESPONSE:
                case Event.EventType.PARTIAL_RESPONSE:
                    MarketDataRequest.ProcessEvent(evt);
                    break;

                case Event.EventType.SUBSCRIPTION_STATUS:
                    break;

                default:
                    foreach (var msg in evt.GetMessages())
                        Console.WriteLine(msg.ToString());
                    break;
            }
        }

        private static void ProcessEvent(Event evt)
        {
            const bool excludeNullElements = true;
            foreach (Message message in evt.GetMessages())
            {
                //string security = message.TopicName;
                //string security = correlationIDs[message.CorrelationID.Value];
                foreach (var field in MarketDataRequest._fields)
                {
                    if (message.HasElement(new Name(field), excludeNullElements))
                    {
                        Element elmField = message[field];

                        Console.WriteLine(string.Format("{0:HH:mm:ss}: {1}, {2}",
                            DateTime.Now,
                            MarketDataRequest.security,
                            elmField.ToString().Trim()));
                    }
                }
            }
            Console.WriteLine();
        }
    }
}
