using invyoc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace invyoc.Extensions;

public static class PrimitiveTypeExtensions
{
    public static decimal ToFormat(this decimal value)
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
                Text = e.ToString() + $" ({GetEnumDescription(e)})"
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
            result = result[..maxLength];

        // Ensure it's not empty
        if (string.IsNullOrWhiteSpace(result))
            return "invoice_";

        result = result.Replace(" ", "_");

        return result;
    }

    public static string ToDateStr(DateTime dateTime)
    {
        return dateTime.ToString("dd-MMM-yyyy");
    }

    public static string ToCurrency(this decimal value)
    {
        return string.Format(new CultureInfo("en-IN", false), "{0:N2}", Convert.ToDouble(value));
    }

    public static void AppendJsonObjectToFile(string filePath, SavedInvoiceData newObject)
    {
        List<SavedInvoiceData> dataList = [];

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {
                try
                {
                    dataList = JsonSerializer.Deserialize<List<SavedInvoiceData>>(jsonContent);
                }
                catch
                {
                    // fallback in case of invalid format
                    dataList = [];
                }
            }
            else dataList = [];
        }
        else
        {
            var jsonFilePath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(jsonFilePath) && !Directory.Exists(jsonFilePath))
                Directory.CreateDirectory(jsonFilePath);

            dataList = [];
        }

        newObject.Id = dataList.Count + 1;
        newObject.Timestamp = DateTime.UtcNow;


        // Add new data at the top
        dataList.Insert(0, newObject);

        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        string updatedJson = JsonSerializer.Serialize(dataList, options);
        File.WriteAllText(filePath, updatedJson);
    }

    public static List<SavedInvoiceData> GetAllContentFromJsonFile(string filePath)
    {
        List<SavedInvoiceData> dataList = [];

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {
                try
                {
                    dataList = JsonSerializer.Deserialize<List<SavedInvoiceData>>(jsonContent);
                }
                catch
                {
                    // fallback in case of invalid format
                    dataList = [];
                }
            }
            else dataList = [];
        }

        return dataList ?? [];
    }
}