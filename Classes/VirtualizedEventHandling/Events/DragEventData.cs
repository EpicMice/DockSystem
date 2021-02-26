using Microsoft.AspNetCore.Components.Web;

namespace TestThing2.Classes
{
    public class DragEventData : EventData
    {
        public DragEventArgsMore args;
        public string Target;
        public string Source;
        public DragEventArgs MouseArgs { get { return args.self_args; } }
    }
}