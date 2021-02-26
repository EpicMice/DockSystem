using Microsoft.AspNetCore.Components.Web;

namespace TestThing2.Classes
{
    public class MouseEventData : EventData
    {
        public MouseEventArgsMore args;
        public string Target;
        public string Source;
        public MouseEventArgs MouseArgs { get { return args.self_args; } }
    }
}