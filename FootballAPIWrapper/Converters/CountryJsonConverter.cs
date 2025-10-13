using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using FootballAPIWrapper.Models;

namespace FootballAPIWrapper.Converters
{
    /// <summary>
    /// Custom JSON converter for Country objects that handles both string and object formats.
    /// The API sometimes returns country as a simple string (e.g., "Argentina") 
    /// and sometimes as an object with name, code, and flag properties.
    /// </summary>
    public class CountryJsonConverter : JsonConverter<Country>
    {
        public override Country ReadJson(JsonReader reader, Type objectType, Country existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.String)
            {
                // Handle simple string format (e.g., "Argentina")
                var countryName = reader.Value.ToString();
                return new Country
                {
                    Name = countryName,
                    Code = string.Empty,
                    Flag = string.Empty
                };
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                // Handle object format with name, code, flag properties
                var jsonObject = JObject.Load(reader);
                
                return new Country
                {
                    Name = jsonObject["name"]?.ToString() ?? string.Empty,
                    Code = jsonObject["code"]?.ToString() ?? string.Empty,
                    Flag = jsonObject["flag"]?.ToString() ?? string.Empty
                };
            }

            throw new JsonSerializationException($"Unexpected token type {reader.TokenType} when deserializing Country");
        }

        public override void WriteJson(JsonWriter writer, Country value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // Always write as object format for consistency
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteValue(value.Name);
            writer.WritePropertyName("code");
            writer.WriteValue(value.Code);
            writer.WritePropertyName("flag");
            writer.WriteValue(value.Flag);
            writer.WriteEndObject();
        }
    }
}