using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Checkers
{
    public class MovementConverter : JsonConverter<Movement>
    {
        public override Movement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("start", out JsonElement startElement) ||
                !root.TryGetProperty("delta", out JsonElement deltaElement))
            {
                throw new JsonException("Invalid Movement JSON structure.");
            }

            var start = ParseCoordinates(startElement);
            var delta = ParseCoordinates(deltaElement);

            return new Movement(start, delta);
        }

        private static (int X, int Y) ParseCoordinates(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object ||
                !element.TryGetProperty("x", out JsonElement xElement) ||
                !element.TryGetProperty("y", out JsonElement yElement) ||
                xElement.ValueKind != JsonValueKind.Number ||
                yElement.ValueKind != JsonValueKind.Number)
            {
                throw new JsonException("Invalid coordinates in Movement JSON.");
            }

            int x = xElement.GetInt32();
            int y = yElement.GetInt32();

            return (x, y);
        }

        public override void Write(Utf8JsonWriter writer, Movement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteCoordinateObject(writer, "start", value.Start);
            WriteCoordinateObject(writer, "delta", value.Delta);
            WriteCoordinateObject(writer, "end", value.End);

            writer.WriteEndObject();
        }

        private static void WriteCoordinateObject(Utf8JsonWriter writer, String propertyName, (int X, int Y) coord)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteStartObject();
            writer.WriteNumber("x", coord.X);
            writer.WriteNumber("y", coord.Y);
            writer.WriteEndObject();
        }
    }
}
