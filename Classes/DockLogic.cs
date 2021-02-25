using System;
using System.Collections.Generic;
using System.Linq;

namespace TestThing2.Classes
{   

    public enum DockContentType
    {
        DIV
    }

    public class DockContent
    {
        public string ID { get; set; }

        public DockContentType DCT = DockContentType.DIV;

        public bool Active = false;

        public void SetActive(bool b)
        {
            this.Active = b;
        }

        public void OnTabDragStart()
        {
            Console.WriteLine("Dragging tab: " + ID);
        }
        public void OnTabDragEnd()
        {
            Console.WriteLine("Ending drag: " + ID);
        }

        //tab element reference?        
        //content element reference?

    }

    public class DockInstance
    {
        public string Dock_ID;
        public Dictionary<string, DockContent> Content_Map = new Dictionary<string, DockContent>();
        
        public void AddTabWithContent(DockContent dcontent)
        {
            Content_Map.Add(dcontent.ID, dcontent);
            if(Content_Map.Count == 1)
            {
                dcontent.Active = true;
            }
        }

        private void PerformSetActive(string id, bool make_active)
        {
            var item = Content_Map.Values.FirstOrDefault(e => e.Active);
            if(item != null)
            {
                if(item.ID != id)
                {
                    item.SetActive(false);
                    Content_Map[id].SetActive(true);
                }
            }
        }

        public void SetActive(string id, bool make_active)
        {
            this.PerformSetActive(id, make_active);
        }

        public void SetActive(DockContent dcontent, bool make_active)
        {
            this.PerformSetActive(dcontent.ID, make_active);
        }

        private void PerformRemoveTabAndContent(string id)
        {
            Content_Map.Remove(id);
            if(Content_Map.Count == 1)
            {
                Content_Map.Values.FirstOrDefault().SetActive(true);
            }
        }

        public void RemoveTabAndContent(string id)
        {
            this.PerformRemoveTabAndContent(id);
        }

        public void RemoveTabAndContent(DockContent dcontent)
        {
            this.PerformRemoveTabAndContent(dcontent.ID);
        }

    }

    public class DockData
    {

        private int id = 0;
        public string MakeID()
        {
            return "id_"+(id++);
        }

        public Dictionary<string, DockInstance> Dock_Map = new Dictionary<string, DockInstance>();

        public Dictionary<string, DockContent> Dock_Tabs = new Dictionary<string, DockContent>();

        public DockData()
        {
            //load config for docks

            //sample config data

            var dock1 = CreateDock();
            var tab1 = CreateTabWithContent();
            var tab2 = CreateTabWithContent();

            dock1.AddTabWithContent(tab1);
            dock1.AddTabWithContent(tab2);

            dock1 = CreateDock();
            tab1 = CreateTabWithContent();
            tab2 = CreateTabWithContent();

            dock1.AddTabWithContent(tab1);
            dock1.AddTabWithContent(tab2);
        }

        public DockContent CreateTabWithContent()
        {
            DockContent dcontent = new DockContent();
            dcontent.ID = MakeID();

            return dcontent;
        }


        public DockInstance CreateDock()
        {
            DockInstance inst = new DockInstance();

            inst.Dock_ID = MakeID();
            this.Dock_Map.Add(inst.Dock_ID, inst);

            return inst;
        }
    }
}