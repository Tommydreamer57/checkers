using System.Text.Json;
using System.Text.Json.Serialization;

namespace Checkers
{
    public class PieceArrayConverter : JsonConverter<Piece[,]>
    {
        public override Piece[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Deserialization of Piece[,] is not implemented.");
        }

        public override void Write(Utf8JsonWriter writer, Piece[,]? value, JsonSerializerOptions options)
        {
            // throw new NotImplementedException("NOT IMPLEMENTED EXCEPTION");

            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();

            for (int i = 0; i < value.GetLength(0); i++)
            {
                writer.WriteStartArray();

                for (int j = 0; j < value.GetLength(1); j++)
                {
                    var piece = value[i, j];
                    JsonSerializer.Serialize(writer, piece, options);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }
    }
}