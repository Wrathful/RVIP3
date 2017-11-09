using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVIP3Classes
{
    [Serializable]
    public class Detail
    {
        public string name;
        public int installation_time;
        public bool have;
        public bool indeliver = false;

        public Detail(string name, int installation_time, bool have)
        {
            this.name = name;
            this.installation_time = installation_time;
            this.have = have;
        }
    }
}
