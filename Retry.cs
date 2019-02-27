
namespace CapCajaLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    internal static class Retry
    {
        public static bool DoWithRetry(Action action, short maxIntento = 4, TimeSpan? sleepPeriod = null)
        {
            if (maxIntento <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxIntento));
            if (sleepPeriod == null)
                sleepPeriod = new TimeSpan(0, 0, 0, 5);
            while (true)
            {
                try
                {
                    action();
                    return true;
                }
                catch (Exception)
                {
                    if (--maxIntento == 0)
                        throw;
                    Thread.Sleep(Convert.ToInt32(sleepPeriod.Value.TotalMilliseconds));
                }
            }
        }
    }
}
