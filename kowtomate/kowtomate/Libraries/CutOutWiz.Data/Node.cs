using System;
using System.Collections.Generic;
using System.Text;

namespace CutOutWiz.Data
{
    public class Node
    {
        public string key { get; set; }
        public string title { get; set; } 
        public bool folder { get; set; }
        public bool lazy { get; set; } = true;

        public Node(string key, string title, bool folder)
        {
            this.key = key;
            this.title = title;
            this.folder = folder;
        }
        
    }
}
