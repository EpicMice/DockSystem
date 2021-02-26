using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;

namespace TestThing2.Classes
{

    public class MouseEventTrack : EventTrack
    {

        public int ScreenX;
        public int ScreenY;

        public int ClientX;
        public int ClientY;

        //public Dictionary<int, bool> ButtonStates = new Dictionary<int, bool>();

        public void UpdateMouse(MouseEventArgs MEA)
        {

        }
    }
    public class DragEventTrack : EventTrack
    {

        public int ScreenX;
        public int ScreenY;

        public int ClientX;
        public int ClientY;

        //public Dictionary<int, bool> ButtonStates = new Dictionary<int, bool>();

        public void UpdateMouse(MouseEventArgs MEA)
        {

        }
    }
}