using System.Reflection;

namespace TourManagementApi.Extensions
{
    public static class ObjectMapperExtensions
    {
        public static void MapWithFallback<TSource, TTarget>(TSource source, TTarget target)
        {
            if (source == null || target == null)
                return;

            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);

            foreach (var sProp in sourceProps)
            {
                if (!targetProps.TryGetValue(sProp.Name, out var tProp))
                    continue;

                var sValue = sProp.GetValue(source);
                if (sValue == null)
                    continue;

                if (sProp.PropertyType == typeof(string) && string.IsNullOrWhiteSpace((string)sValue))
                    continue;

                try
                {
                    var targetType = Nullable.GetUnderlyingType(tProp.PropertyType) ?? tProp.PropertyType;

                    var convertedValue = sValue.GetType() == targetType
                        ? sValue
                        : Convert.ChangeType(sValue, targetType);

                    tProp.SetValue(target, convertedValue);
                }
                catch
                {
                    // tipi uyuşmuyorsa geç (örneğin string → DateTime başarısızsa)
                    continue;
                }
            }
        }
    }
}
