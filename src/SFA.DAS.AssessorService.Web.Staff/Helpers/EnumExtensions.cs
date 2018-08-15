using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var memberInfo = enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First();

        var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

        return (displayAttribute != null ? displayAttribute.GetName() : memberInfo.Name );
    }
}

