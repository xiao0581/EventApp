using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_lib
{
    public class EventRepository
    {

        public List<Event> GetEvents()
        {
            List<Event> events = new List<Event>();
            events.Add(new Event { EventName = "Event 1" });
            events.Add(new Event { EventName = "Event 2" });
            events.Add(new Event { EventName = "Event 3" });
            events.Add(new Event { EventName = "Event 4" });
            events.Add(new Event { EventName = "Event 5" });
            return events;
        }

        public void getEventDetails(Event e)
        {
            Console.WriteLine("Event Name: " + e.EventName);
        }
    }
}
