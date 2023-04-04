using System;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public static class SystemTime
    {
        /// <summary> Normally this is a pass-through to DateTime.Now, but it can be overridden with SetDateTime( .. ) for testing or debugging.
        /// </summary>
        private static Func<DateTime> utcNow = () => DateTime.UtcNow;

        public static Func<DateTime> UtcNow
        {
            get { return utcNow; }
            set { utcNow = value; }
        }
    }
}