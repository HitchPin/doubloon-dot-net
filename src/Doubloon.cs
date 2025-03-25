namespace Doubloon
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Microsoft.IO;

    public readonly struct Doubloon<T> where T : ICurrency, new()
    {
        private static readonly RecyclableMemoryStreamManager manager = new RecyclableMemoryStreamManager();
        
        private readonly T currency;
        private readonly decimal value;
        public Doubloon(decimal d) : this(TypeCache.GetInstance<T>(), TypeCache.GetInstance<T>().AsSafeDecimal(d))
        {
        }
        private Doubloon(T instance, decimal d)
        {
            this.currency = instance;
            this.value = d;
        }
        
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
                var writer = new Utf8JsonWriter(ms, new JsonWriterOptions() { Indented = false, NewLine = ""});
                writer.WriteStartArray();
                writer.WriteStringValue(this.AsScalarString());
                writer.WriteStringValue(this.currency.Name);
                writer.WriteEndArray();
                writer.Flush();
                writer.Dispose();
                ms.Position = 0;
                canonBase64 = Convert.ToBase64String(ms.ToArray());
            }

            using (var ms = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(ms, new JsonWriterOptions() { Indented = false, NewLine = ""});
                writer.WriteStartObject();
                writer.WriteString("canonical", canonBase64);
                writer.WriteString("formatted", this.currency.ToDisplayFormat(this.value));
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
            obj.Add("formatted", this.currency.ToDisplayFormat(this.value));
            return obj;
        }

        private string ToCanonicalString()
        {
            using (var ms = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(ms, new JsonWriterOptions() { Indented = false, NewLine = ""});
                writer.WriteStartArray();
                writer.WriteStringValue(this.AsScalarString());
                writer.WriteStringValue(this.currency.Name);
                writer.WriteEndArray();
                writer.Flush();
                writer.Dispose();
                ms.Position = 0; 
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static Doubloon<JT> Parse<JT>(string json) where JT : ICurrency, new()
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Invalid Doubloon JSON. Expected object.");
            }

            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                string prop = reader.GetString();
                if (prop == "canonical")
                {
                    reader.Read();
                    var b64 = reader.GetString();
                    var bytes = Convert.FromBase64String(b64);
                    ReadOnlySpan<byte> span = bytes.AsSpan();
                    var r2 = new Utf8JsonReader(span, new JsonReaderOptions() { AllowMultipleValues = false, CommentHandling = JsonCommentHandling.Disallow});
                    r2.Read();
                    if (r2.TokenType != JsonTokenType.StartArray) throw new JsonException("Invalid Doubloon JSON. Expected array.");
                    reader.Read();
                    string currencyName = reader.GetString();
                    reader.Read();
                    string value = reader.GetString();
                    reader.Read();
                    if (r2.TokenType != JsonTokenType.EndArray)
                    {
                        throw new JsonException("Expected only two strings.");
                    }

                    var jt = new JT();
                    if (jt.Name != currencyName)
                    {
                        throw new ArgumentException($"JSON string '{jt.Name}' does not match currency '{currencyName}'.");
                    }

                    return new Doubloon<JT>(decimal.Parse(value));
                }
            }
            throw new JsonException("Invalid Doubloon JSON.");
        }
    }
}