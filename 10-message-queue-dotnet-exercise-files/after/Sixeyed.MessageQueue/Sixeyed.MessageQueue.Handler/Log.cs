
using log4net;
using log4net.Config;

namespace Sixeyed.MessageQueue.Handler
{
    public static class Log
    {
        private static ILog _Log;

        static Log()
        {
            XmlConfigurator.Configure();
            _Log = LogManager.GetLogger("Sixeyed.MessageQueue.Handler");
        }

        public static void WriteLine(string format, params object[] args)
        {
            _Log.Debug(string.Format(format, args));
        }
    }
}
