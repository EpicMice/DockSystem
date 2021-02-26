using System;
using System.Collections.Generic;

namespace TestThing2.Classes
{
    public class InputService {

        public Dictionary<string, EventActionGroup> RegisteredElements = new Dictionary<string, EventActionGroup>();

        public EventHandler event_handler = new EventHandler();

        public int RegisterElement(EventActionGroup eag)
        {
            Console.WriteLine("Registering: " + eag.ID);
            this.RegisteredElements.Add(eag.ID, eag);

            this.event_handler.RegisterEvent(eag);
            return 1;
        }

        public void UnregisterElement(EventActionGroup eag)
        {
            Console.WriteLine("Unregistering: " + eag.ID);
            this.RegisteredElements.Remove(eag.ID);
        }
    }
}