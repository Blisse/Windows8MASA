using System;
using System.Collections.Generic;
using System.Text;

namespace MASA.Common.Utilities
{
    public static class DateTimeOffsetUtility
    {
        public static DateTime DateTimeFromUnixTimeStamp(long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }
    }
}
