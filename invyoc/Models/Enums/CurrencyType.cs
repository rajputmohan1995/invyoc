using System.ComponentModel;

namespace invyoc.Models.Enums
{
    public enum CurrencyType
    {
        [Description("$")]
        USD,

        [Description("₹")]
        INR,

        [Description("€")]
        EUR,

        [Description("£")]
        GBP
    }
}
