using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace TestThing2.Classes
{
    public class MouseEventTrack
    {
        public int ScreenX;
        public int ScreenY;

        public int ClientX;
        public int ClientY;

        public Dictionary<int, bool> ButtonStates = new Dictionary<int, bool>();

        public void UpdateMouse(MouseEventArgs MEA)
        {

        }
    }
}