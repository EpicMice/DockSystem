using System;

namespace TestThing2.Classes
{

    public interface IHasArgs
    {
        public object args { set; get; }
        public void SetEventArgs(object val) => args = val;
        public T GetEventArgs<T>() => (T)this.args;
    }

    public class EventData : EventArgs, IHasArgs
    {
        public string Target { get; set; }
        public string Source { get; set; }
        public object args { set; get; }
        public void SetEventArgs(object val) => args = val;
        public T GetEventArgs<T>() => (T)this.args;
    }
}