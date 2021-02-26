using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static TestThing2.Classes.EventActionGroup;
using static TestThing2.Classes.EventNames;

namespace TestThing2.Classes
{

    public class EventHandler
    {

        public Dictionary<EventNames, EventTrack> EventTrackMap = MouseEventNames
            .Select(name => new KeyValuePair<EventNames, EventTrack>(name, new MouseEventTrack()))
            .ToDictionary(a => a.Key, a => a.Value);

        public Dictionary<int, bool> MouseButtonStates = new Dictionary<int, bool>();

        public bool GetMouseButtonState(int button)
        {
            MouseButtonStates.TryAdd(button, false);
            return MouseButtonStates[button];
        }

        public void SetMouseButtonState(int button, bool down){
            MouseButtonStates.TryAdd(button, down);
            MouseButtonStates[button] = down;
        }

        public List<EventNames> PipelineEventNames = new List<EventNames> {
            OnMouseDown,
            OnMouseUp,
            OnMouseOver,
        };

        public const int STARTING = 0;
        public const int PROPAGATING = 1;
        public const int BUBBLE_ENDING = 2;

        public Dictionary<EventNames, Action<string, object, int>> PipelineEvents = new Dictionary<EventNames, Action<string, object, int>>
        {

        };

        public (EventNames, string, object, Action) RegisterPipelineParams { get; set; } = (EventNames.None, null, null, null);

        public EventHandler()
        {
            PipelineEvents[OnMouseDown] = this.RegisterMousePipelineEvent(state => {
                if (state == STARTING)
                {
                    this.SetMouseButtonState((int)((MouseEventArgs)TempEventArgs).Button, true);
                    Console.WriteLine("MOUSE DOWN");
                    Console.WriteLine("STARTING");
                }

                //if(state == BUBBLE_ENDING)                
                //drag start should start on mousemove events, not mousedown events.                
            });

            PipelineEvents[OnMouseUp] = this.RegisterMousePipelineEvent(state => {
                if (state == STARTING)
                {
                    this.SetMouseButtonState((int)((MouseEventArgs)TempEventArgs).Button, false);
                    Console.WriteLine("MOUSE UP");
                    Console.WriteLine("STARTING");
                }
            });

            //never output to console on mousemove events.
            //unless it's a one-off message.
            PipelineEvents[OnMouseMove] = this.RegisterMousePipelineEvent(state => {
                if (state == BUBBLE_ENDING)
                {
                    //Console.WriteLine("MOUSE UP");
                    //Console.WriteLine("STARTING");
                    if (this.GetMouseButtonState((int)((MouseEventArgs)TempEventArgs).Button))
                    {

                    }
                }
            });
        }

        Dictionary<EventNames, string> EventStash = new Dictionary<EventNames, string>();

        public Action<string, object,int> RegisterMousePipelineEvent(Action<int> piplineEvent) => (source, mouseEventArgs, state) =>
        {
            if (state == BUBBLE_ENDING) return;

            RegisterPipelineParams = (OnMouseDown, source, mouseEventArgs, () =>
            {
                piplineEvent(state);
            }
            );

            this.RegisterPipelineEvent<MouseEventArgs, MouseEventTrack, MouseEventData>();
        };
        

        public object TempEventArgs { get; set; }
        public object TempEventTrack { get; set; }


        //funcName = name of the function
        //source = ID of the event source (The first element that began the event bubbling.
        //mouseEventArgs = the raw Mouse Event Data
        public void RegisterPipelineEvent<EventArgType,EventArgTrack,EventArgData>() where EventArgTrack:IGetEventArgs where EventArgData: EventData
        {

            this.TempEventArgs = RegisterPipelineParams.Item3;

            ((EventArgTrack) (this.TempEventTrack = this.EventTrackMap[OnMouseDown])).SetEventArgs(this.TempEventArgs);

            RegisterPipelineParams.Item4();
             
            if (RegisterPipelineParams.Item2 == null)
            {
                RegisterPipelineParams = (RegisterPipelineParams.Item1, ((EventArgData)RegisterPipelineParams.Item3).Source, RegisterPipelineParams.Item3, RegisterPipelineParams.Item4);
                Console.WriteLine("SOURCE: " + RegisterPipelineParams.Item2);
            }

            EventStash.TryAdd(RegisterPipelineParams.Item1, RegisterPipelineParams.Item2);
            EventStash[RegisterPipelineParams.Item1] = RegisterPipelineParams.Item2;

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

            foreach (EventNames event_name in hold)
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
}