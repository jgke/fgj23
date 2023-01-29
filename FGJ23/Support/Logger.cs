using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Support
{
    public static class Logger
    {
        private static bool loggingSetup = false;
        public static void SetupLogger()
        {
            if (!loggingSetup)
            {
                loggingSetup = true;
                Log.Logger = new LoggerConfiguration()
                    .Destructure.With(new LoggableDestructuringPolicy())
                    .WriteTo.Console()
                    .WriteTo.Debug()
                    .CreateLogger();

            }
        }
    }
}
