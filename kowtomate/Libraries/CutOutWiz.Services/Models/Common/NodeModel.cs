using System;
using System.Collections.Generic;
using System.Text;

namespace CutOutWiz.Core
{
    public class NodeModel
    {
        public string key { get; set; }
        public string title { get; set; } 
        public bool folder { get; set; }
        public bool lazy { get; set; } = true;

        public NodeModel(string key, string title, bool folder)
        {
            this.key = key;
            this.title = title;
            this.folder = folder;
        }
        
    }
}
