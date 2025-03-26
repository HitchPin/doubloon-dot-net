namespace Doubloon.Tests.Currencies;

using Doubloon.Currencies;
using Xunit;

public class CVETests
{
    [Fact]
    public void CVEFormatsCorrectly()
    {
        Assert.Equal("10$25", new CVE().ToDisplayFormat(10.25M));
    }
}