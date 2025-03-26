namespace Doubloon.Tests.Currencies;

using Doubloon.Currencies;
using Xunit;

public class EURTests
{
    [Fact]
    public void EURFormatsCorrectly()
    {
        Assert.Equal("10.25€", new EUR().ToDisplayFormat(10.25M));
    }
}