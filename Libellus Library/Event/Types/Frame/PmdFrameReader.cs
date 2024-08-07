﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibellusLibrary.Event.Types.Frame
{
	internal class PmdFrameReader : JsonConverter<List<PmdTargetType>>
	{
		public override List<PmdTargetType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			List<PmdTargetType> frames = new();

			reader.Read();
			List<PmdTargetType> abstractTypes = new();

			Utf8JsonReader abstractReader = reader;

			while (abstractReader.TokenType != JsonTokenType.EndArray)
			{
				PmdTargetType abstractType = JsonSerializer.Deserialize<PmdTargetType>(ref abstractReader, options)!;
				abstractTypes.Add(abstractType);
				abstractReader.Read();
			}

			foreach (PmdTargetType abstractType in abstractTypes)
			{
				Type trueDataType = PmdFrameFactory.GetFrameType(abstractType.TargetType).GetType();
				frames.Add((PmdTargetType)JsonSerializer.Deserialize(ref reader, trueDataType, options)!);
				reader.Read();
			}
			//reader.Read();
			return frames;
		}

		public override void Write(Utf8JsonWriter writer, List<PmdTargetType> value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (PmdTargetType data in value)
			{
				writer.WriteRawValue(JsonSerializer.Serialize<object>(data, options));
			}
			writer.WriteEndArray();

		}
	}
}
