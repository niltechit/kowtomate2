using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core
{
    public class KeyValuePairCustomModel
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public KeyValuePairCustomModel(string name, string value)
        {
            Name = name;    
            Value = value;
        }
    }
}
