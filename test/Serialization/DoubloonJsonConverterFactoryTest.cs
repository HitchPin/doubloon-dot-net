namespace Doubloon.Tests.Serialization;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Doubloon.Currencies;
using Xunit;

public class DoubloonJsonConverterFactoryTest
{
    [Fact]
    public void SerializesToJson()
    {
        var x = new Doubloon<USD>("10.00");
        var c = JsonSerializer.Serialize(x);
        var obj = (JsonObject)JsonNode.Parse(c)!;
        Assert.Equal("WyIxMC4wMCIsIlVTRCJd", obj["canonical"]!.GetValue<string>());
        Assert.Equal("$10.00", obj["display_only"]!.GetValue<string>());
    }

    [Fact]
    public void RoundTrips()
    {
        var x = new Doubloon<USD>("10.00");
        var c = JsonSerializer.Serialize(x);
        var d = JsonSerializer.Deserialize<Doubloon<USD>>(c);
        Assert.Equal(x, d);
    }

    [Fact]
    public void ThrowsOnNonObject()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Doubloon<USD>>("[\"asdf\"]"));
    }
    [Fact]
    public void ThrowsOnMissingCanonical()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Doubloon<USD>>(
            """
            {
              "display_only": "$1.00"
            }
            """));
    }
}