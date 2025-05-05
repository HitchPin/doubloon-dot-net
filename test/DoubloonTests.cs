namespace Doubloon.Tests;

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Doubloon.Currencies;
using Microsoft.IO;
using Xunit;

public class DoubloonTests
{
    [Fact]
    public void CreatesAndExtractsIdenticalValues()
    {
        var d1 = new Doubloon<USD>("0.1");
        Assert.Equal("0.1", d1.AsScalarString());
    }
    [Fact]
    public void AddsCorrectly()
    {
        var d1 = new Doubloon<USD>("0.1");
        var d2 = new Doubloon<USD>("0.2");
        var d3 = d1 + d2;
        Assert.Equal("0.3", d3.AsScalarString());
    }
    [Fact]
    public void SubtractsCorrectly()
    {
        var d1 = new Doubloon<USD>("13.30");
        var d2 = new Doubloon<USD>("5.20");
        var d3 = d1 - d2;
        Assert.Equal("8.10", d3.AsScalarString());
    }
    [Fact]
    public void MultipliesCorrectlyByInt()
    {
        var d1 = new Doubloon<USD>("13.30");
        var d3 = d1 * 3;
        Assert.Equal("39.90", d3.AsScalarString());
    }
    [Fact]
    public void MultipliesCorrectlyByDecimal()
    {
        var d1 = new Doubloon<USD>("13.30");
        var d3 = d1 * 2.14M;
        Assert.Equal("Doubloon<USD>(28.4620)", d3.ToString());
        Assert.Equal("28.46", d3.AsScalarString());
    }
    [Fact]
    public void DividesCorrectlyByInt()
    {
        var d1 = new Doubloon<USD>("13.30");
        var d3 = d1 / 4;
        Assert.Equal("Doubloon<USD>(3.325)", d3.ToString());
        Assert.Equal(3.32M, d3.AsScalarDecimal());
        Assert.Equal("3.32", d3.AsScalarString());
    }
    [Fact]
    public void ComparesEqualityCorrectly()
    {
        var d1 = new Doubloon<USD>("13.30");
        var d2 = new Doubloon<USD>("13.30");
        Assert.True(d1 == d2);
        Assert.True(d1.Equals(d2));
        
        var e1 = new Doubloon<USD>("13.30");
        var e2 = new Doubloon<USD>("13.20");
        Assert.True(e1 != e2);
        Assert.False(e1.Equals(e2));
    }
    [Fact]
    public void CompareeValueCorrectly()
    {
        var x = new Doubloon<USD>("13.30");
        var lt = new Doubloon<USD>("13.20");
        var eq = new Doubloon<USD>("13.30");
        var gt = new Doubloon<USD>("13.40");
        
        Assert.False(x < eq);
        Assert.True(x <= eq);
        Assert.True(x == eq);
        Assert.True(x >= eq);
        Assert.False(x > eq);
        
        Assert.True(x < gt);
        Assert.True(x <= gt);
        Assert.False(x == gt);
        Assert.False(x >= gt);
        Assert.False(x > gt);        
        
        Assert.False(x < lt);
        Assert.False(x <= lt);
        Assert.False(x == lt);
        Assert.True(x >= lt);
        Assert.True(x > lt);
    }

    [Fact]
    public void SerializesToCorrectJsonValue()
    {
        var d = new Doubloon<USD>("2.25");
        var c = d.ToJson();
        Assert.Equal("WyJVU0QiLCIyLjI1Il0=", c["canonical"]!.GetValue<string>());
        Assert.Equal("$2.25", c["display_only"]!.GetValue<string>());
    }
    
    [Fact]
    public void DeserializesCorrectDataFromJsonValue()
    {
        var json = Doubloon<USD>.Parse("WyJVU0QiLCIyLjI1Il0=");
        Assert.Equal(new Doubloon<USD>("2.25"), json);
    }

    [Fact]
    public void ProducesJsonString()
    {
        var jsonStr = new Doubloon<USD>("2.25").ToJsonString();
        var node = JsonNode.Parse(jsonStr);
        Assert.Equal("$2.25", node!["display_only"]!.GetValue<string>());
        Assert.Equal("WyJVU0QiLCIyLjI1Il0=", node!["canonical"]!.GetValue<string>());
    }

    [Fact]
    public void ThrowsIfCanonicalHasTooManyStringsInArray()
    {
        var s = new JsonArray()
        {
            "10.25",
            "USD",
            "three is too many"
        };
        var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(s.ToJsonString()));
        Assert.Throws<JsonException>(() => Doubloon<USD>.Parse(b64));
    }
    
    [Fact]
    public void ThrowsIfCurrencyDoesNotMatchT()
    {
        var s = new JsonArray()
        {
            "10.25",
            "CAD",
        };
        var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(s.ToJsonString()));
        Assert.Throws<ArgumentException>(() => Doubloon<USD>.Parse(b64));
    }
    [Fact]
    public void ThrowsIfJsonObjectMissingCanonical()
    {
        var s = new JsonObject()
        {
            ["hi"] = "there"
        };
        Assert.Throws<ArgumentException>(() => Doubloon<USD>.Parse(s.ToJsonString()));
    }
    [Fact]
    public void ThrowsIfJsonMalformed()
    {
        var json = """
                   {
                      "display_only": "there"
                   ]
                   """;
        Assert.ThrowsAny<Exception>(() => Doubloon<USD>.Parse(json));
    }
    [Fact]
    public void ThrowsOnInvalidJsonType()
    {
        var json = """
                   []
                   """;
        Assert.ThrowsAny<Exception>(() => Doubloon<USD>.Parse(json));
    }
    [Fact]
    public void HashCodeSameForEqualObjects()
    {
        var x = new Doubloon<USD>("2.25");
        var y = new Doubloon<USD>("2.25");
        Assert.Equal(x, y);
        Assert.Equal(x.GetHashCode(), y.GetHashCode());
        var z = new Doubloon<CAD>("2.25");
        Assert.NotEqual((object)x, z);
        Assert.NotEqual(x.GetHashCode(), z.GetHashCode());
        var zz = new Doubloon<CAD>("2.26");
        Assert.NotEqual((object)z, zz);
        Assert.NotEqual(z.GetHashCode(), zz.GetHashCode());
    }

    [Fact]
    public void StreamManagerVendsMemoryStreams()
    {
        var mm = GetUnsafeField(new Doubloon<USD>());
        using var ms = mm.GetStream();
        Assert.IsAssignableFrom<MemoryStream>(ms);
    }
    
    [Fact]
    public void LooksUpCurrencyByName()
    {
        var c = CurrencyRegistry.ByName("USD");
        Assert.Equal(typeof(USD), c);
    }

    [Fact]
    public void CurrentPropertyReflectsCurrency()
    {
        var d = new Doubloon<EUR>();
        Assert.IsType<EUR>(d.Currency);
    }
    
    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "manager")]
    public static extern ref RecyclableMemoryStreamManager GetUnsafeField<T>(Doubloon<T> unsafeExample) where T : ICurrency, new();
}
