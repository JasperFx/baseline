using System;

namespace Baseline.Testing
{
    public static class DisposableExtensions
    {
        public static void SafeDispose(this IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // That's right, swallow that exception
            }
        }

    }
}