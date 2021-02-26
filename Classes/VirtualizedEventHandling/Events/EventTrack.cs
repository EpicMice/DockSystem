namespace TestThing2.Classes
{
    public interface IGetEventArgs
    {
        public T GetEventArgs<T>();
        public void SetEventArgs(object val);
    }

    public class EventTrack : IGetEventArgs
    {
        private object EventArgs { get; set; }

        public void SetEventArgs(object val) => EventArgs = val;
        public T GetEventArgs<T> () => (T) this.EventArgs;
    }
}