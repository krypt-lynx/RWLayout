using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    class LogHelper
    {
        public static void LogException(string message, Exception e)
        {
            Log.Error(BuildExceptionMessage(message, e));
        }

        public static string BuildExceptionMessage(string message, Exception e)
        {
            var result = $"{message}: {e.GetType().Name}: {e.Message}\n";
            if (e.InnerException != null)
            {
                result += BuildExceptionMessage("Inner Exception", e.InnerException);
            }
            return result;
        }
    }
}
