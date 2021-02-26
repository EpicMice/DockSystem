using System;
using System.Collections.Generic;

namespace TestThing2.Classes
{
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
}