using Microsoft.AspNetCore.Components.Web;

namespace TestThing2.Classes
{
    public class DragEventData : EventData
    {
        
        public DragEventArgs MouseArgs { get { return (DragEventArgs) this.GetEventArgs<DragEventArgsMore>().args; } }
    }
}