using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FootballAPIWrapper.Converters
{
    public class ErrorsJsonConverter : JsonConverter<List<string>>
    {
        public override List<string> ReadJson(JsonReader reader, Type objectType, List<string>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            
            if (token.Type == JTokenType.Array)
            {
                // Normal case: errors is an array
                return token.ToObject<List<string>>() ?? new List<string>();
            }
            else if (token.Type == JTokenType.Object)
            {
                // Edge case: errors is an empty object, return empty list
                return new List<string>();
            }
            else if (token.Type == JTokenType.Null)
            {
                // Null case: return empty list
                return new List<string>();
            }
            
            // Fallback: return empty list
            return new List<string>();
        }

        public override void WriteJson(JsonWriter writer, List<string>? value, JsonSerializer serializer)
        {
            // Always write as an array
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}