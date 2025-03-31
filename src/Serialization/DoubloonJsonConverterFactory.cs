namespace Doubloon.Serialization;

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DoubloonJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Doubloon<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var tCurrency = typeToConvert.GetGenericArguments().First();
        var converter = typeof(DoubloonJsonConverter<>).MakeGenericType(tCurrency);
        return (JsonConverter)Activator.CreateInstance(converter);
    }

    public class DoubloonJsonConverter<TCurrency> : JsonConverter<Doubloon<TCurrency>>
        where TCurrency : ICurrency, new()
    {
        public override Doubloon<TCurrency> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected object start");
            }

            string? canonical = null;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var prop = reader.GetString();
                reader.Read();
                var val = reader.GetString();
                if (prop == "canonical")
                {
                    canonical = val;
                }
            }

            if (canonical == null)
            {
                throw new JsonException("No canonical value set.");
            }
            
            return Doubloon<TCurrency>.FromCanonicalString(canonical);
        }

        public override void Write(Utf8JsonWriter writer, Doubloon<TCurrency> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("canonical", value.ToCanonicalString());
            writer.WriteString("display_only", value.ToDisplay());
            writer.WriteEndObject();
        }
    }
}