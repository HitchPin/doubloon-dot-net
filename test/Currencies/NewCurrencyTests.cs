namespace Doubloon.Tests.Currencies;

using Doubloon.Currencies;
using Xunit;

public class NewCurrencyTests
{
    [Fact]
    public void LoadsNewCurrencyFromJson()
    {
        var c = new Doubloon<HYPE>(30.119M);
        c += new Doubloon<HYPE>(14.231M);
        Assert.Equal("44.350", c.AsScalarString());
        var asJson = c.ToJsonString();
        var data = Doubloon<HYPE>.Parse(asJson);
        Assert.Equal(data, c);
    }
    [RegisteredCurrency]
    public class HYPE : QuantizedCurrency
    {
        public HYPE() : base("~", 3)
        {
        }

        public override string ToDisplayFormat(decimal d) =>
            d.ToString("~#,##0.00");
    }
}