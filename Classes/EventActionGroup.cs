using System;
using System.Collections.Generic;
using System.Linq;
using static TestThing2.Classes.EventNames;

namespace TestThing2.Classes
{

    public enum EventNames
    {
        OnMouseMove,
        OnMouseDown,
        OnMouseUp,
        OnMouseOver,

        OnDragStart,
        OnDragEnd,
        OnDragOver,
        OnDrop,

        OnKeyDown,
        OnKeyUp,
        OnKeyPress,

        OnWheel,
        OnScroll,
        None,
    }

    public class EventActionGroup
    {

        public static Action<object> DefaultAction = (a) => { };

        public EventActionGroup Parent { get; set; }

        private static int ID_Counter = 0;
        public string ID = "" + EventActionGroup.ID_Counter++;

        public static List<EventNames> KeyEventNames = new List<EventNames>
        {
            OnKeyDown,
            OnKeyUp,
            OnKeyPress
        };

        public static List<EventNames> MouseEventNames = new List<EventNames> {
            OnMouseMove,
            OnMouseDown,
            OnMouseOver,
            OnMouseUp
        };

        public static List<EventNames> DragEventNames = new List<EventNames> {
            OnDragStart,
            OnDragEnd,
            OnDragOver,
            OnDrop
        };

        public static Action<T> GetDefaultActions<T>() where T: class
        {
            return DefaultAction;
        }

        public static IEnumerable<EventNames> AllEventNames => Enum.GetValues(typeof(EventNames)).Cast<EventNames>();

        public static Dictionary<EventNames, Action<T>> MakeEventDictionary<T>() where T: class => AllEventNames
            .Select(name => new KeyValuePair<EventNames, Action<T>>(name, DefaultAction))
            .ToDictionary(a => a.Key, a => a.Value);

        public Dictionary<EventNames, Action<object>> Events = MakeEventDictionary<object>();

        public Dictionary<EventNames, Action<object>> FiredEvents = MakeEventDictionary<object>();

    }
}