using Microsoft.AspNetCore.Components.Web;

namespace TestThing2.Classes
{
    public class DragEventArgsMore : IHasArgs
    {

        public string event_id = "some data";
        public object args { set; get; }
        public void SetEventArgs(object val) => args = val;
        public DragEventArgs GetEventArgs() => (DragEventArgs)this.args;
    }
}