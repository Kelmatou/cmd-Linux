using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public class Genius_data
    {
        public string command_usage;
        public string data;

        public Genius_data(string command_usage, string data)
        {
            this.command_usage = command_usage;
            this.data = data;
        }
    }
}
