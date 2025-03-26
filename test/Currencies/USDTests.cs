namespace Doubloon.Tests.Currencies;

using global::Doubloon.Currencies;
using Xunit;

public class USDTests
{
    [Fact]
    public void VerifiesDecimalPlaces()
    {
        Assert.Throws<ImplicitRoundingForbiddenException>(() => new USD().AsSafeDecimal(12.345M));
        Assert.Throws<ImplicitRoundingForbiddenException>(() => new USD().AsSafeDecimal("12.345"));
    }
    [Fact]
    public void FormatsAppropriately()
    {
        var usd = 12.34M;
        var dollars = new USD().ToDisplayFormat(usd);
        Assert.Equal("$12.34", dollars);
    }
}