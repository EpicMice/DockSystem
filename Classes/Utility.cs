using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestThing2.Classes{

    public class StyleUpdate
    {
        public Dictionary<string, Action<(int,int)>> Style_Map = new Dictionary<string, Action<(int, int)>>();

        public void AddStyleCallback(string id, Action<(int, int)> a)
        {
            this.Style_Map.Add(id, a);
        }

        public void UpdateStyle(string id, (int, int) ab)
        {
            Style_Map[id](ab);
        }
    }

    public class EventActionGroup
    {
        private static int ID_Counter = 0;
        public string ID = ""+EventActionGroup.ID_Counter++;

        public Action OnMouseDown = () => { };
        public Action OnMouseUp = () => { };
        public Action OnDragStart = () => { };
        public Action OnDragEnd = () => { };
        public Action OnMouseMove = () => { };
    }

    public class InputService {

        public Dictionary<string, EventActionGroup> RegisteredElements = new Dictionary<string, EventActionGroup>();

        /*
         
        RegisteredElements is for storing EventActionGroups with an associated string (id)


         
        */

        public int TrackDragX { get; set; } = 0;
        public int TrackDragY { get; set; } = 0;

        public string TrackString = null;

        public string StartDrag(int x, int y)
        {
            this.TrackDragX = x;
            this.TrackDragY = y;
            return this.TrackString = Guid.NewGuid().ToString().Replace("-", "_");
        }

        public (bool, int, int) GetDragDifference(string id, int x, int y)
        {
            (bool, int, int) hold = (id == this.TrackString, x - TrackDragX, y - TrackDragY);
            TrackDragX = 0;
            TrackDragY = 0;
            return hold;
        }

        public bool LMB_Down = false;
        public bool RMB_Down = false;

        public int ScreenX { get; set; } = 0;
        public int ScreenY { get; set; } = 0;
        public int ClientX { get; set; } = 0;
        public int ClientY { get; set; } = 0;

        public (int, int) CheckButtonState(MouseEventArgs e)
        {
            int LMB_State = 0, RMB_State = 0;

            if (LMB_Down != (LMB_Down = (1 & e.Buttons) != 0))
            {
                if (LMB_Down)
                {
                    //button was just pressed
                    LMB_State = 1;
                }
                if (!LMB_Down)
                {
                    //button was just released
                    LMB_State = -1;
                }
            }

            if (RMB_Down != (RMB_Down = (2 & e.Buttons) != 0))
            {
                if (RMB_Down)
                {
                    //button was just pressed
                    RMB_State = 1;
                }
                if (!RMB_Down)
                {
                    //button was just released
                    RMB_State = -1;
                }
            }

            return (LMB_State, RMB_State);
        }

        public Dictionary<string, List<string>> MouseDownEventOrder = new Dictionary<string, List<string>>()
        {
            { nameof(RMB_Down), new List<string>() },
            { nameof(LMB_Down), new List<string>() }
        };

        public List<string> GetMouseDownItems(int button)
        {
            switch (button)
            {
                case 0: return MouseDownEventOrder[nameof(LMB_Down)];
                case 1: return MouseDownEventOrder[nameof(RMB_Down)];
                default: return null;
            }
        }

        public void OnMouseDown(MouseEventArgs e, string source_element)
        {
            var states = this.CheckButtonState(e);

            if (LMB_Down)
            {
                MouseDownEventOrder[nameof(LMB_Down)].Add(source_element);
                Console.WriteLine("Adding LMB");
            }

            if (RMB_Down)
            {
                MouseDownEventOrder[nameof(RMB_Down)].Add(source_element);
                Console.WriteLine("Adding RMB");
            }
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            var states = this.CheckButtonState(e);
            if (!LMB_Down)
            {
                MouseDownEventOrder[nameof(LMB_Down)].ForEach((a) => { RegisteredElements[a].OnMouseUp(); });
                MouseDownEventOrder[nameof(LMB_Down)].Clear();
            }
            if (!RMB_Down)
            {
                MouseDownEventOrder[nameof(RMB_Down)].ForEach((a) => { RegisteredElements[a].OnMouseUp(); });
                MouseDownEventOrder[nameof(RMB_Down)].Clear();
            }
        }

        public void OnMouseOver(MouseEventArgs e)
        {
            this.ScreenX = (int)e.ScreenX;
            this.ScreenY = (int)e.ScreenY;
            this.ScreenX = (int)e.ClientX;
            this.ScreenY = (int)e.ClientY;
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            this.ScreenX = (int)e.ScreenX;
            this.ScreenY = (int)e.ScreenY;
            this.ScreenX = (int)e.ClientX;
            this.ScreenY = (int)e.ClientY;

            if (LMB_Down)
            {
                //this.MouseDownEventOrder[nameof(LMB_Down)].Where(a=>a.)
            }

        }

        public void OnClick(MouseEventArgs e)
        {
            this.ScreenX = (int)e.ScreenX;
            this.ScreenY = (int)e.ScreenY;
            this.ScreenX = (int)e.ClientX;
            this.ScreenY = (int)e.ClientY;
        }

    }
}