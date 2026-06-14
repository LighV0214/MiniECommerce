using System.Globalization;
using System.Reflection;

namespace MiniECommerce.Api.Common.Mapping;

public static class CommandRequestMapper
{
    public static TCommand Map<TCommand>(object request)
    {
        var requestType = request.GetType();
        var commandType = typeof(TCommand);
        var requestProperties = requestType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var constructor in commandType
                     .GetConstructors()
                     .OrderByDescending(constructor => constructor.GetParameters().Length))
        {
            var parameters = constructor.GetParameters();
            var arguments = new object?[parameters.Length];
            var canUseConstructor = true;

            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                
                if (!requestProperties.TryGetValue(parameter.Name ?? string.Empty, out var property))
                {
                    canUseConstructor = false;
                    break;
                }

                var value = property.GetValue(request);
                arguments[index] = ConvertValue(value, property.PropertyType, parameter.ParameterType);
            }

            if (canUseConstructor)
            {
                return (TCommand)constructor.Invoke(arguments);
            }
        }

        throw new InvalidOperationException(
            $"Cannot map request '{requestType.Name}' to command '{commandType.Name}'. " +
            "Ensure request properties match command constructor parameters by name.");
    }

    private static object? ConvertValue(object? value, Type sourceType, Type destinationType)
    {
        var targetType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

        if (value is null)
        {
            return destinationType.IsValueType && Nullable.GetUnderlyingType(destinationType) is null
                ? Activator.CreateInstance(destinationType)
                : null;
        }

        if (targetType.IsAssignableFrom(sourceType) || targetType.IsInstanceOfType(value))
        {
            return value;
        }

        if (targetType.IsEnum)
        {
            return value is string text
                ? Enum.Parse(targetType, text, ignoreCase: true)
                : Enum.ToObject(targetType, value);
        }

        if (targetType == typeof(Guid))
        {
            return value is Guid ? value : Guid.Parse(value.ToString()!);
        }

        return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }
}
