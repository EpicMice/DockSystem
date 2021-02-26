﻿using Microsoft.AspNetCore.Components.Web;

namespace TestThing2.Classes
{
    public class MouseEventData : EventData
    {
        public MouseEventArgs MouseArgs { get { return (MouseEventArgs) this.GetEventArgs<MouseEventArgsMore>().args; } }
    }
}