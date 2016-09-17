using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public class Link
    {
        public string name;
        public string target;

        public Link(string name, string target)
        {
            this.name = '~' + name;
            this.target = target;
        }
    }
}
