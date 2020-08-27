using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.Alpha1
{
    public partial class CElement
    {
        public string Name = null;

        public static int nextId = 0;
        public string NamePrefix()
        {
            if (Name == null)
            {
                return $"{GetType().Name}_{id}";
            } 
            else
            {
                return $"{GetType().Name}_{Name}_{id}";
            }
        }

    }
}
