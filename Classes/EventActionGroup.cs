using System;
using System.Collections.Generic;

namespace TestThing2.Classes
{
    public class EventActionGroup
    {

        public static Action DefaultAction = () => { };

        public EventActionGroup Parent { get; set; }

        private static int ID_Counter = 0;
        public string ID = "" + EventActionGroup.ID_Counter++;

        public const string OnMouseMove = nameof(OnMouseMove);
        public const string OnMouseDown = nameof(OnMouseDown);
        public const string OnMouseUp = nameof(OnMouseUp);
        public const string OnMouseOver = nameof(OnMouseOver);

        public const string OnDragStart = nameof(OnDragStart);
        public const string OnDragEnd = nameof(OnDragEnd);
        public const string OnDragOver = nameof(OnDragOver);
        public const string OnDrop = nameof(OnDrop);

        public List<string> MouseEventNames = new List<string> {
            OnMouseMove,
            OnMouseDown,
            OnMouseOver,
            OnMouseUp
        };

        public List<string> DragEventNames = new List<string> {
            OnDragStart,
            OnDragEnd,
            OnDragOver,
            OnDrop
        };

        public Dictionary<string, Action<object>> Events = new Dictionary<string, Action<object>> {
            { OnMouseMove,  (a)=>{ } },
            { OnMouseDown,  (a)=>{ } },
            { OnMouseUp,    (a)=>{ } },
            { OnMouseOver,  (a)=>{ } },

            { OnDragStart,  (a)=>{ } },
            { OnDragEnd,    (a)=>{ } },
            { OnDragOver,   (a)=>{ } },
            { OnDrop,       (a)=>{ } },
        };

        public Dictionary<string, Action<EventData>> FiredEvents = new Dictionary<string, Action<EventData>> {
            { OnMouseMove,  (a)=>{ DefaultAction(); } },
            { OnMouseDown,  (a)=>{ DefaultAction(); } },
            { OnMouseUp,    (a)=>{ DefaultAction(); } },
            { OnMouseOver,  (a)=>{ DefaultAction(); } },

            { OnDragStart,  (a)=>{ DefaultAction(); } },
            { OnDragEnd,    (a)=>{ DefaultAction(); } },
            { OnDragOver,   (a)=>{ DefaultAction(); } },
            { OnDrop,       (a)=>{ DefaultAction(); } },
        };
    }
}