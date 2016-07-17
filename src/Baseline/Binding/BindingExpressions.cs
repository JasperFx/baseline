using System.Reflection;

namespace Baseline.Binding
{
    internal class BindingExpressions
    {
        internal static MethodInfo DataSourceGet = typeof(IDataSource).GetMethod(nameof(IDataSource.Get));
        internal static MethodInfo DataSourceGetChild = typeof(IDataSource).GetMethod(nameof(IDataSource.GetChild));
        internal static MethodInfo DataSourceHas = typeof(IDataSource).GetMethod(nameof(IDataSource.Has));
        internal static MethodInfo DataSourceHasChild = typeof(IDataSource).GetMethod(nameof(IDataSource.HasChild));
    }
}