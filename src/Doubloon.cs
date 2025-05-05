namespace Doubloon
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using Microsoft.IO;
    using Serialization;

    [JsonConverter(typeof(DoubloonJsonConverterFactory))]
    public readonly struct Doubloon<T> where T : ICurrency, new()
    {
        private static readonly RecyclableMemoryStreamManager manager = new RecyclableMemoryStreamManager();
        
        private readonly T currency;
        private readonly decimal value;
        public Doubloon() : this(CurrencyRegistry.RegisterOrGetRegistrant<T>(), Decimal.Zero)
        {
        }
        public Doubloon(decimal d) : this(CurrencyRegistry.RegisterOrGetRegistrant<T>(), CurrencyRegistry.RegisterOrGetRegistrant<T>().AsSafeDecimal(d))
        {
        }
        public Doubloon(string d) : this(CurrencyRegistry.RegisterOrGetRegistrant<T>(), CurrencyRegistry.RegisterOrGetRegistrant<T>().AsSafeDecimal(d))
        {
        }
        private Doubloon(T instance, decimal d)
        {
            this.currency = instance;
            this.value = d;
        }

        public T Currency => currency;
        
        public static Doubloon<T> operator +(Doubloon<T> d1, Doubloon<T> d2)
        {
            return new Doubloon<T>(d1.currency, d1.value + d2.value);
        }
        public static Doubloon<T> operator -(Doubloon<T> d1, Doubloon<T> d2)
        {
            return new Doubloon<T>(d1.currency, d1.value - d2.value);
        }
        public static Doubloon<T> operator *(Doubloon<T> d1, int scalar)
        {
            return new Doubloon<T>(d1.currency, d1.value * scalar);
        }
        public static Doubloon<T> operator *(Doubloon<T> d1, decimal scalar)
        {
            return new Doubloon<T>(d1.currency, d1.value * scalar);
        }
        public static Doubloon<T> operator /(Doubloon<T> d1, int scalar)
        {
            return new Doubloon<T>(d1.currency, d1.value / scalar);
        }
        public static bool operator >(Doubloon<T> d1, Doubloon<T> d2)
        {
            return d1.value > d2.value;
        }
        public static bool operator <(Doubloon<T> d1, Doubloon<T> d2)
        {
            return d1.value < d2.value;
        }
        public static bool operator >=(Doubloon<T> d1, Doubloon<T> d2)
        {
            return d1.value >= d2.value;
        }
        public static bool operator <=(Doubloon<T> d1, Doubloon<T> d2)
        {
            return d1.value <= d2.value;
        }
        public static bool operator ==(Doubloon<T> d1, Doubloon<T> d2)
        {
            return d1.value == d2.value;
        }
        public static bool operator !=(Doubloon<T> d1, Doubloon<T> d2)
        {
            return d1.value != d2.value;
        }

        public override int GetHashCode()
        {
            var str = this.currency.Name + "|" + this.value.ToString();
            return str.GetHashCode();
        }
        

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is not Doubloon<T> doubloon) return false;
            return doubloon.value == this.value;
        }

        public override string ToString()
        {
            return "Doubloon<" + this.currency.Name + ">(" + this.value + ")";
        }

        public decimal AsScalarDecimal()
        {
            return currency.Quantize(this.value);
        }
        public string AsScalarString()
        {
            return currency.Quantize(this.value).ToString();
        }

        public string ToJsonString()
        {
            string canonBase64;
            using (var ms = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(ms, new JsonWriterOptions() { Indented = false});
                writer.WriteStartArray();
                writer.WriteStringValue(this.currency.Name);
                writer.WriteStringValue(this.AsScalarString());
                writer.WriteEndArray();
                writer.Flush();
                writer.Dispose();
                ms.Position = 0;
                canonBase64 = Convert.ToBase64String(ms.ToArray());
            }

            using (var ms = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(ms, new JsonWriterOptions() { Indented = false });
                writer.WriteStartObject();
                writer.WriteString("canonical", canonBase64);
                writer.WriteString("display_only", this.currency.ToDisplayFormat(this.value));
                writer.WriteEndObject();
                writer.Flush();
                writer.Dispose();
                ms.Position = 0;
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        
        public JsonNode ToJson()
        {
            var canon = ToCanonicalString();
            var obj = new JsonObject();
            obj.Add("canonical", canon);
            obj.Add("display_only", this.currency.ToDisplayFormat(this.value));
            return obj;
        }

        internal string ToCanonicalString()
        {
            using (var ms = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(ms, new JsonWriterOptions() { Indented = false});
                writer.WriteStartArray();
                writer.WriteStringValue(this.currency.Name);
                writer.WriteStringValue(this.AsScalarString());
                writer.WriteEndArray();
                writer.Flush();
                writer.Dispose();
                ms.Position = 0; 
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        internal string ToDisplay()
        {
            return this.currency.ToDisplayFormat(this.value);
        }
        internal static Doubloon<T> FromCanonicalString(string canon)
        {
            var b64 = canon;
            var bytes = Convert.FromBase64String(b64);
            ReadOnlySpan<byte> span = bytes.AsSpan();
            var r2 = new Utf8JsonReader(span, new JsonReaderOptions() { AllowMultipleValues = false, CommentHandling = JsonCommentHandling.Disallow});
            r2.Read();
            if (r2.TokenType != JsonTokenType.StartArray) throw new JsonException("Invalid Doubloon JSON. Expected array.");
            r2.Read();
            string currencyName = r2.GetString();
            r2.Read();
            string value = r2.GetString();
            r2.Read();
            if (r2.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException("Expected only two strings.");
            }

            var jt = new T();
            if (jt.Name != currencyName)
            {
                throw new ArgumentException($"JSON string '{jt.Name}' does not match currency '{currencyName}'.");
            }

            return new Doubloon<T>(decimal.Parse(value));
        }

        public static Doubloon<T> Parse(string json)
        {
            if (json.TrimStart().StartsWith("{"))
            {
                var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
                reader.Read();
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Read();
                    while (reader.TokenType != JsonTokenType.EndObject)
                    {
                        string propName = reader.GetString();
                        reader.Read();
                        if (propName == "canonical")
                        {
                            return FromCanonicalString(reader.GetString());
                        }

                        reader.Read();
                    }

                }
                
                throw new ArgumentException("Input was an object but no matching doubloon found.");
            }
            else if (char.IsLetterOrDigit(json[0]))
            {
                return FromCanonicalString(json);
            }

            throw new ArgumentException("Unknown input format.");
        }
    }
}