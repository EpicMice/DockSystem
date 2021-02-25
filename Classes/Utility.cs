using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestThing2.Classes{


    public class StyleUpdate
    {
        public Dictionary<string, Action<(int,int)>> Style_Map = new Dictionary<string, Action<(int, int)>>();

        public void AddStyleCallback(string id, Action<(int, int)> a)
        {
            this.Style_Map.Add(id, a);
        }

        public void UpdateStyle(string id, (int, int) ab)
        {
            Style_Map[id](ab);
        }
    }
    public class EventData
    {

    }

    public class MouseEventData : EventData
    {
        public MouseEventArgsMore args;
        public string Target;
        public string Source;
        public MouseEventArgs MouseArgs { get { return args.self_args; } }
    }

    public class DragEventData : EventData
    {
        public DragEventArgsMore args;
        public string Target;
        public string Source;
        public DragEventArgs MouseArgs { get { return args.self_args; } }
    }

    public class MouseEventArgsMore
    {
        public MouseEventArgs self_args;

        public string event_id = "some data";
    }
    public class DragEventArgsMore
    {
        public DragEventArgs self_args;

        public string event_id = "some data";
    }

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

    public class MouseEventTrack
    {
        public int ScreenX;
        public int ScreenY;

        public int ClientX;
        public int ClientY;

        public Dictionary<int, bool> ButtonStates = new Dictionary<int, bool>();

        public void UpdateMouse(MouseEventArgs MEA)
        {

        }
    }

    public class EventHandler
    {

        public MouseEventTrack mouse_tracking = new MouseEventTrack();

        public const string OnMouseDown = nameof(OnMouseDown);
        public const string OnMouseUp = nameof(OnMouseUp);
        public const string OnMouseOver = nameof(OnMouseOver);

        public const string OnDragStart = nameof(OnDragStart);
        public const string OnDragOver = nameof(OnDragOver);
        public const string OnDragEnd = nameof(OnDragEnd);
        public const string OnDrop = nameof(OnDrop);

        public List<string> PipelineEventNames = new List<string> {
            OnMouseDown,
            OnMouseUp,
            OnMouseOver,
        };

        public const int STARTING = 0;
        public const int PROPAGATING = 1;
        public const int BUBBLE_ENDING = 2;

        public Dictionary<string, Action<string, object, int>> PipelineEvents = new Dictionary<string, Action<string, object, int>>
        {
        };

        public EventHandler()
        {
            PipelineEvents[OnMouseDown] = RunMouseDown;
            PipelineEvents[OnMouseUp] = (a, b, state) => {
                switch (state)
                {
                    case STARTING:
                        Console.WriteLine(a+" : ON MOUSE UP");
                        break;
                    case PROPAGATING:
                        Console.WriteLine(a + " : ON MOUSE UP");
                        break;
                    case BUBBLE_ENDING:
                        break;
                }
            };
        }

        Dictionary<string, string> EventStash = new Dictionary<string, string>();

        //source = ID of the event source (The first element that began the event bubbling.
        //MEA = the Mouse Event Data
        //state = the STARTING, RUNNING, or STOPPING state of the function 
        public void RunMouseDown(string source, object MEA, int state)
        {

            var event_data = MEA as MouseEventArgs;

            switch (state)
            {
                case STARTING:

                    this.mouse_tracking.ButtonStates.TryAdd((int)event_data.Button, true);
                    break;
                case PROPAGATING:
                    break;
                case BUBBLE_ENDING:
                    return;
            }

            if(source == null)
            {
                source = (MEA as MouseEventData).Source;
                Console.WriteLine("SOURCE: " + source);
            }

            EventStash.TryAdd(nameof(RunMouseDown), source);
            EventStash[nameof(RunMouseDown)] = source;

            Console.WriteLine(MEA.GetType());
            //this.mouse_tracking.UpdateMouse(MEA as MouseEventArgs);
        }

        //The first time the function received event_args, it will be MouseEventArgs, KeyEventArgs, or DragEventArgs.
        //The second time the function is passed the object, it is EventData
        public EventData GetEventMoreID(string id, object event_args)
        {

            if (event_args.GetType() == typeof(MouseEventArgs))
            {
                return new MouseEventData { Target = id, Source = id, args = new MouseEventArgsMore { event_id = id, self_args = event_args as MouseEventArgs } };
            }

            if (event_args.GetType() == typeof(MouseEventData))
            {
                var hold = event_args as MouseEventData;
                return new MouseEventData { Target = id, Source = hold.Source, args = hold.args };
            }

            if (event_args.GetType() == typeof(DragEventArgs))
            {
                return new DragEventData { Target = id, Source = id,  args = new DragEventArgsMore { event_id = id, self_args = event_args as DragEventArgs } };
            }

            if (event_args.GetType() == typeof(DragEventData))
            {
                var hold = event_args as DragEventData;
                return new DragEventData { Target = id, Source = hold.Source, args = hold.args };
            }
                
            throw new Exception("event_args not cast for "+event_args.GetType().Name);
        }

        public void RegisterEvent(EventActionGroup eag)
        {

            var hold = eag.Events.Keys.ToArray();

            foreach (string event_name in hold)
            {
                eag.Events[event_name] = (event_args) =>
                {

                    if(this.PipelineEvents.TryGetValue(event_name, out var run_event))
                    {
                        if (event_args.GetType().IsSubclassOf(typeof(EventData)))
                        {
                            run_event(null, event_args, PROPAGATING);
                        }
                        else
                        {
                            run_event(eag.ID, event_args, STARTING);
                            //start of bubble
                        }
                    }
                 
                    var source_args = this.GetEventMoreID(eag.ID, event_args);

                    eag.FiredEvents[event_name](
                        source_args
                    );

                    if (eag.Parent != null)
                    {
                        eag.Parent.Events[event_name](source_args);
                    }
                    else
                    {
                        //end of the bubble.
                        if (run_event != null)
                        {
                            run_event(eag.ID, event_args, BUBBLE_ENDING);
                        }
                    }

                };
            }
        }
    }

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