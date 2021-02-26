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

        public Dictionary<string, EventActionGroup> RegisteredComponents = new Dictionary<string, EventActionGroup>();

        public Dictionary<EventNames, EventTrack> EventTrackMap = MouseEventNames
            .Select(name => new KeyValuePair<EventNames, EventTrack>(name, new MouseEventTrack()))
            .Concat(DragEventNames.Select(name=> new KeyValuePair<EventNames, EventTrack>(name, new DragEventTrack())))
            .ToDictionary(a => a.Key, a => a.Value);

        public Dictionary<int, bool> MouseButtonStates = new Dictionary<int, bool>();

        public List<int> TempButtonStates = new List<int>();

        public Dictionary<int, bool> MouseActiveButtonDrag = new Dictionary<int, bool>();

        public bool GetMouseButtonState(int button)
        {
            MouseButtonStates.TryAdd(button, false);
            return MouseButtonStates[button];
        }

        public void SetMouseButtonState(int button, bool down) {
            MouseButtonStates.TryAdd(button, down);
            MouseButtonStates[button] = down;
        }

        public bool GetDragButtonState(int button)
        {
            MouseActiveButtonDrag.TryAdd(button, false);
            return MouseActiveButtonDrag[button];
        }

        //Returns if the button was not already dragging (down)        
        //Returns if the button is dragging, but was not already being tracked.
        public bool SetDragButtonState(int button, bool down)
        {

            MouseActiveButtonDrag.TryAdd(button, down);
            MouseActiveButtonDrag[button] = down;

            //just so I can call this method in the where clause.
            return true;
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

        public Dictionary<EventNames, (EventNames, string, object, Action)> RegisterPipelineParamMap = 
            new Dictionary<EventNames, (EventNames, string, object, Action)>();


        public EventHandler()
        {
            PipelineEvents[OnMouseDown] = this.RegisterMousePipelineEvent(OnMouseDown, (id, eventArgs, state) => {
                if (state == STARTING)
                {
                    this.SetMouseButtonState((int)((MouseEventArgs)TempEventArgs).Button, true);
                }

                //if(state == BUBBLE_ENDING)                
                //drag start should start on mousemove events, not mousedown events.                
            });

            PipelineEvents[OnMouseUp] = this.RegisterMousePipelineEvent(OnMouseUp, (id, eventArgs, state) => {
                if (state == STARTING)
                {
                    this.SetMouseButtonState((int)((MouseEventArgs)TempEventArgs).Button, false);
                    return;
                }          
                
                if(state == BUBBLE_ENDING)
                {
                    Console.WriteLine(TempEventArgs.GetType());

                    if (TempEventArgs as IHasArgs != null)
                    {
                        TempEventArgs = (TempEventArgs as IHasArgs).args;

                        if (TempEventArgs as IHasArgs != null)
                        {
                            TempEventArgs = (TempEventArgs as IHasArgs).args;
                        }
                    }
                    
                    Console.WriteLine(TempEventArgs.GetType());

                    if (this.GetDragButtonState((int)((MouseEventArgs)TempEventArgs).Button))
                    {
                        this.SetDragButtonState((int)((MouseEventArgs)TempEventArgs).Button, false);

                        this.RegisteredComponents[id].Events[OnDragEnd](new DragEventArgs { 
                            //Fill out the eventargs for the spec.
                        });

                        Console.WriteLine("WHAT: "+id);
                    }
                }
            });

            //never output to console on mousemove events.
            //unless it's a one-off message.
            PipelineEvents[OnMouseMove] = this.RegisterMousePipelineEvent(OnMouseMove, (id, eventArgs, state) => {
                if (state == BUBBLE_ENDING)
                {
                    //Console.WriteLine("MOUSE UP");

                    this.TempButtonStates = this.MouseButtonStates
                    .Where(b => b.Value && !this.GetDragButtonState(b.Key) && this.SetDragButtonState(b.Key, true))
                    .ToList()
                    .ConvertAll(e => e.Key)
                    .ToList();

                    if (this.TempButtonStates.Count > 0)
                    {
                        Console.WriteLine(this.TempButtonStates.Count+": "+ id);

                        this.RegisteredComponents[id].Events[OnDragStart](new DragEventArgs
                        {
                            //Fill out the eventargs for the spec.
                        });
                        }
                }
            });

            PipelineEvents[OnDragStart] = this.RegisterDragPipelineEvent(OnDragStart, (id, eventArgs, state) => {
                if (state == STARTING)
                {
                    Console.WriteLine("starting drag: " + id);
                }
            });

            PipelineEvents[OnDragEnd] = this.RegisterDragPipelineEvent(OnDragEnd, (id, eventArgs, state) => {
                if (state == STARTING)
                {
                    Console.WriteLine("ending drag: " + id);
                }
            });

            //implement the OnDragStart function next
        }

        Dictionary<EventNames, string> EventStash = new Dictionary<EventNames, string>();

        public Action<string, object, int> RegisterMousePipelineEvent(EventNames event_name, Action<string, EventArgs, int> pipelineEvent)//don't ask.
        {
            return this._RegisterEventPipeline<MouseEventArgs, MouseEventTrack, MouseEventData>(event_name, pipelineEvent);
        }

        public Action<string, object, int> RegisterDragPipelineEvent(EventNames event_name, Action<string, EventArgs, int> pipelineEvent)//don't ask.
        {
            return this._RegisterEventPipeline<DragEventArgs, DragEventTrack, DragEventData>(event_name, pipelineEvent);
        }

        private Action<string, object, int> _RegisterEventPipeline<EventArgType, EventTrackType, EventDataType>
            (EventNames event_name, Action<string, EventArgs, int> pipelineEvent) 
            where EventTrackType : IGetEventArgs where EventDataType : EventData => (source, event_args, state) =>
        {

            this.RegisterPipelineParamMap[event_name] = (event_name, source, event_args, () =>
            {
                pipelineEvent(source, (EventArgs) event_args, state);
            }
            );

            this.RegisterPipelineEvent<EventArgType, EventTrackType, EventDataType>(event_name);
        };

        public object TempEventArgs { get; set; }
        public object TempEventTrack { get; set; }

        //funcName = name of the function
        //source = ID of the event source (The first element that began the event bubbling.
        //mouseEventArgs = the raw Mouse Event Data
        public void RegisterPipelineEvent<EventArgType,EventArgTrack,EventArgData>(EventNames event_name) where EventArgTrack:IGetEventArgs where EventArgData: EventData
        {

            this.TempEventArgs = this.RegisterPipelineParamMap[event_name].Item3;
            ((EventArgTrack) (this.TempEventTrack = this.EventTrackMap[event_name])).SetEventArgs(this.TempEventArgs);

            this.RegisterPipelineParamMap[event_name].Item4();
             
            if (this.RegisterPipelineParamMap[event_name].Item2 == null)
            {
                this.RegisterPipelineParamMap[event_name] = (event_name, ((EventArgData)this.RegisterPipelineParamMap[event_name].Item3).Source, this.RegisterPipelineParamMap[event_name].Item3, this.RegisterPipelineParamMap[event_name].Item4);
                Console.WriteLine("SOURCE: " + this.RegisterPipelineParamMap[event_name].Item2);
            }

            EventStash.TryAdd(this.RegisterPipelineParamMap[event_name].Item1, this.RegisterPipelineParamMap[event_name].Item2);
            EventStash[this.RegisterPipelineParamMap[event_name].Item1] = this.RegisterPipelineParamMap[event_name].Item2;
        }

        //The first time the function received event_args, it will be MouseEventArgs, KeyEventArgs, or DragEventArgs.
        //The second time the function is passed the object, it is EventData
        public EventData GetEventMoreID(string id, object event_args)
        {

            if (event_args.GetType() == typeof(MouseEventArgs))
            {
                return new MouseEventData { Target = id, Source = id, args = new MouseEventArgsMore { event_id = id, args = event_args as MouseEventArgs } };
            }

            if (event_args.GetType() == typeof(MouseEventData))
            {
                var hold = event_args as MouseEventData;
                return new MouseEventData { Target = id, Source = hold.Source, args = hold.args };
            }

            if (event_args.GetType() == typeof(DragEventArgs))
            {
                return new DragEventData { Target = id, Source = id,  args = new DragEventArgsMore { event_id = id, args = event_args as DragEventArgs } };
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
            this.RegisteredComponents.Add(eag.ID, eag);

            foreach (EventNames event_name in eag.Events.Keys.ToArray())
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
                    
                    //perhaps put this in a task.
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