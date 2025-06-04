using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace invyoc.Extensions;

public static class PrimitiveTypeExtensions
{
    public static decimal ToFormat(this decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    public static double ToFormat(this double value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    public static List<SelectListItem> ToSelectList<TEnum>() where TEnum : Enum
    {
        return [.. Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new SelectListItem
            {
                Value = GetEnumDescription(e),
                Text = GetEnumDescription(e) + " " + e.ToString()
            })];
    }

    public static string GetEnumDescription<TEnum>(TEnum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() as DescriptionAttribute;
        return attr?.Description ?? value.ToString();
    }

    public static string MakeValidFileName(string name, string replacement = "_", int maxLength = 255)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "invoice_";

        // Get invalid characters and create a regex pattern
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string invalidReStr = $"[{invalidChars}]+";

        // Replace invalid characters with the replacement
        string result = Regex.Replace(name, invalidReStr, replacement);

        // Trim to max length
        if (result.Length > maxLength)
            result = result.Substring(0, maxLength);

        // Ensure it's not empty
        if (string.IsNullOrWhiteSpace(result))
            return "invoice_";

        return result;
    }
}
