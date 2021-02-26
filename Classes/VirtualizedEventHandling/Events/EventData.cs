namespace TestThing2.Classes
{

    public class EventData
    {
        public string Target { get; set; }
        public string Source { get; set; }

        public object args { get; set; }

        public void SetEventArgs(object val) => args = val;
        public T GetEventArgs<T>() => (T)this.args;
    }
}