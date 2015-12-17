using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Baseline
{
	public static class Platform
	{
        static Platform()
        {
            var name = GetUname();
            IsWindows = name == string.Empty;
            IsDarwin = string.Equals(GetUname(), "Darwin", StringComparison.Ordinal);
            IsLinux = (!IsWindows && !IsDarwin);
        }

        public static bool IsWindows { get; }

        public static bool IsDarwin { get; }

        public static bool IsLinux { get; }

        [DllImport("libc")]
        static extern int uname(StringBuilder buf);

        private static string GetUname()
        {
            var buffer = new StringBuilder(8192);
            try
            {
                if (uname(buffer) == 0)
                {
                    return buffer.ToString();
                }
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}

