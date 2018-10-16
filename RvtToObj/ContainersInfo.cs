using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RvtToObj
{
    public class ContainersInfo
    {
        public List<Color> color { get; set; }
        public List<double> transparencys { get; set; }
        public List<int> Shiniess { get; set; }
        public List<string> name { get; set; }
    }
}
