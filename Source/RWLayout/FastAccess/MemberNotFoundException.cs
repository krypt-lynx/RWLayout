using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    class MemberNotFoundException : Exception
    {
        public MemberNotFoundException() : base() { }
        public MemberNotFoundException(string message) : base(message) { }
        public MemberNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        public MemberNotFoundException(string member, Type type) : base($"Method \"{member}\" is not found in type \"{type.FullName}\"") { }
    }
}
