using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAKURA.CORE
{
    public class UserType : EntityBase
    {
        public bool State { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
