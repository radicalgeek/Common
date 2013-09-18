using System;

namespace RadicalGeek.Common.Time
{
    public static class Clock
    {
        private static DateTime startTime = DateTime.Now;
        private static DateTime initTime = DateTime.Now;

        public static readonly DateTime MinDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Creates a new DateTime with the given year, month & day. If the year given is less than 100, it attempts to determine whether it's a 20th or 21st century date by assuming that the date will only be in the past.
        /// </summary>
        /// <returns></returns>
        public static DateTime NewDate(int year, int month, int day, DateTime? ceilingDate = null)
        {
            if (!ceilingDate.HasValue)
                ceilingDate = Now;
            if (year < 100)
            {
                if (year <= (ceilingDate.Value.Year - 2000))
                    year += 2000;
                else
                    year += 1900;
            }
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Returns True if Clock is currently Frozen.
        /// </summary>
        public static bool IsFrozen { get; private set; }

        /// <summary>
        /// Freezes the clock at the current time.
        /// </summary>
        public static void Freeze()
        {
            IsFrozen = true;
            startTime += (DateTime.Now - initTime);
        }

        /// <summary>
        /// Resumes chronological operation of the Clock
        /// </summary>
        public static void Unfreeze()
        {
            IsFrozen = false;
            initTime = DateTime.Now;
        }

        /// <summary>
        /// Advances the Clock by the given TimeSpan.
        /// </summary>
        /// <param name="timeSpan"></param>
        public static void Advance(TimeSpan timeSpan)
        {
            startTime += timeSpan;
        }

        /// <summary>
        /// Retards the Clock by the given TimeSpan.
        /// </summary>
        /// <param name="timeSpan"></param>
        public static void Retard(TimeSpan timeSpan)
        {
            startTime -= timeSpan;
        }

        /// <summary>
        /// Gets the time now as understood by the Clock. When Clock is Frozen, Now will return the same value until Clock is UnFrozen.
        /// </summary>
        public static DateTime Now
        {
            get
            {
                TimeSpan timeDifference = (IsFrozen ? TimeSpan.Zero : (DateTime.Now - initTime));
                return startTime + timeDifference;
            }
        }

        /// <summary>
        /// Sets the time of the  If you check Clock again in 1 second and Clock is not frozen, the time will be 1 second later than what you set the clock to.
        /// </summary>
        /// <param name="time">The date/time to set the clock to.</param>
        public static void SetTime(DateTime time)
        {
            startTime = time;
            initTime = DateTime.Now;
        }

        /// <summary>
        /// Gets the Date as understood by the Clock
        /// </summary>
        public static DateTime Today { get { return Now.Date; } }

        /// <summary>
        /// Gets the Date Tomorrow as understood by the Clock
        /// </summary>
        public static DateTime Tomorrow { get { return Now.Date.AddDays(1); } }

        /// <summary>
        /// Gets the Date Yesterday as understood by the Clock
        /// </summary>
        public static DateTime Yesterday { get { return Now.Date.AddDays(-1); } }

        /// <summary>
        /// Returns the Clock to a Normal state (i.e. current real date/time)
        /// </summary>
        public static void Reset()
        {
            startTime = DateTime.Now;
            initTime = DateTime.Now;
            IsFrozen = false;
        }
    }
}
