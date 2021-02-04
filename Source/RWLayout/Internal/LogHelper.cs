using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    public enum MessageType
    {
        Verbose,
        Message,
        Warning,
        Error,
    }

    public static class LogHelper
    {
        public static void Log(this string str, MessageType type = MessageType.Verbose)
        {
            str = $"[RWL] {str}";
            switch (type)
            {
                case MessageType.Verbose:
                    if (RWLayoutInfo.VerboseLogging)
                    {
                        Verse.Log.Message(str);
                    }
                    return;
                case MessageType.Message:
                    Verse.Log.Message(str);
                    return;
                case MessageType.Warning:
                    Verse.Log.Warning(str);
                    return;
                case MessageType.Error:
                    Verse.Log.Error(str);
                    return;
            }
        }


        public static void LogException(string message, Exception e)
        {
            BuildExceptionMessage(message, e).Log(MessageType.Error);
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
