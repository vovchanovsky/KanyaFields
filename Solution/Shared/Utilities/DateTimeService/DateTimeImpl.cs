using System;

namespace Utilities.DateTimeService
{
    public class DateTimeImpl : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
