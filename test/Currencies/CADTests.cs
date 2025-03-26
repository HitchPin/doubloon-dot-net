namespace Doubloon.Tests.Currencies;

using Doubloon.Currencies;
using Xunit;

public class CADTests
{
    [Fact]
    public void CADFormatsCorrectly()
    {
        Assert.Equal("$10.25", new CAD().ToDisplayFormat(10.25M));
    }
}