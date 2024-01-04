using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCube
{
    public static class Extensions
    {
        /// <summary>
        /// Similar to <see cref="GetReadableTime(TimeSpan)"/>.
        /// </summary>
        /// <param name="timeSpan"><see cref="TimeSpan"/></param>
        /// <returns>formatted text</returns>
        public static string ToReadableString(this TimeSpan span)
        {
            //return string.Format("{0}{1}{2}{3}",
            //    span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s") : string.Empty,
            //    span.Duration().Hours > 0 ? string.Format("{0:0} hr{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s") : string.Empty,
            //    span.Duration().Minutes > 0 ? string.Format("{0:0} min{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s") : string.Empty,
            //    span.Duration().Seconds > 0 ? string.Format("{0:0} sec{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s") : string.Empty);

            var parts = new StringBuilder();
            if (span.Days > 0)
                parts.Append($"{span.Days} day{(span.Days == 1 ? string.Empty : "s")} ");
            if (span.Hours > 0)
                parts.Append($"{span.Hours} hour{(span.Hours == 1 ? string.Empty : "s")} ");
            if (span.Minutes > 0)
                parts.Append($"{span.Minutes} minute{(span.Minutes == 1 ? string.Empty : "s")} ");
            if (span.Seconds > 0)
                parts.Append($"{span.Seconds} second{(span.Seconds == 1 ? string.Empty : "s")} ");
            if (span.Milliseconds > 0)
                parts.Append($"{span.Milliseconds} millisecond{(span.Milliseconds == 1 ? string.Empty : "s")} ");

            return parts.ToString().Trim();
        }
    }
}
