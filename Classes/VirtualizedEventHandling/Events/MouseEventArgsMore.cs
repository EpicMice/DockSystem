using Microsoft.AspNetCore.Components.Web;

namespace TestThing2.Classes
{
    public class MouseEventArgsMore : IHasArgs
    {
        public string event_id = "some data";

        public object args { set; get; }
        public void SetEventArgs(object val) => args = val;
        public T GetEventArgs<T>() => (T)this.args;
        public MouseEventArgs GetEventArgs() => (MouseEventArgs)this.args;
    }
}