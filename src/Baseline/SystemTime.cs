using System;

namespace Baseline
{
    public class SystemTime : ISystemTime
    {
        private readonly IClock _clock;
        private readonly ITimeZoneContext _context;

        public SystemTime(IClock clock, ITimeZoneContext context)
        {
            _clock = clock;
            _context = context;
        }

        public LocalTime LocalTime()
        {
            return new LocalTime(_clock.UtcNow(), _context.GetTimeZone());
        }

        public DateTime UtcNow()
        {
            return _clock.UtcNow();
        }

        public static SystemTime Default()
        {
            return new SystemTime(new Clock(), new MachineTimeZoneContext());
        }

        public static SystemTime AtLocalTime(DateTime now)
        {
            return new SystemTime(new Clock().LocalNow(now), new MachineTimeZoneContext());
        }

        public static SystemTime AtLocalTime(TimeSpan timeOfDay)
        {
            return AtLocalTime(DateTime.Today.Add(timeOfDay));
        }
    }
}