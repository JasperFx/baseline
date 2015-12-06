using System;

namespace Baseline
{
	public static class Platform
	{
		public static bool IsLinux()
		{
			var pf = Environment.OSVersion.Platform;
			return pf == PlatformID.Unix;
		}

		public static bool IsMacOSX()
		{
			var pf = Environment.OSVersion.Platform;
			return pf == PlatformID.MacOSX;
		}

		public static bool IsUnix()
		{
			return IsLinux () || IsMacOSX ();
		}


	}
}

