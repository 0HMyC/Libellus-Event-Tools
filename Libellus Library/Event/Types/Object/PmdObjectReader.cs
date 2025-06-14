using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibellusLibrary.Event.Types.Object
{
	internal class PmdObjectReader : JsonConverter<List<PmdObjectType>>
	{
		public override List<PmdObjectType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			List<PmdObjectType> objects = new();

			reader.Read();
			List<PmdObjectType> abstractTypes = ReadAbstractObjects(reader, options);

			foreach (PmdObjectType abstractType in abstractTypes)
			{
				Type trueDataType = PmdObjectFactory.GetObjectType(abstractType.ObjectID).GetType();
				objects.Add((PmdObjectType)JsonSerializer.Deserialize(ref reader, trueDataType, options)!);
				reader.Read();
			}
			return objects;
		}

		public override void Write(Utf8JsonWriter writer, List<PmdObjectType> value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (PmdObjectType data in value)
			{
				writer.WriteRawValue(JsonSerializer.Serialize<object>(data, options));
			}
			writer.WriteEndArray();
		}

		public static List<PmdObjectType> ReadAbstractObjects(Utf8JsonReader abstractReader, JsonSerializerOptions options)
		{
			List<PmdObjectType> abstractTypes = new();
			while (abstractReader.TokenType != JsonTokenType.EndArray)
			{
				PmdObjectType abstractType = JsonSerializer.Deserialize<PmdObjectType>(ref abstractReader, options)!;
				abstractTypes.Add(abstractType);
				abstractReader.Read();
			}
			return abstractTypes;
		}
	}

	internal class DDSObjectReader : PmdObjectReader
	{
		public override List<PmdObjectType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			List<PmdObjectType> objects = new();

			reader.Read();
			List<PmdObjectType> abstractTypes = ReadAbstractObjects(reader, options);

			foreach (PmdObjectType abstractType in abstractTypes)
			{
				// TODO: Better than this, ala the frametarget readers
				Type trueDataType = new DDSObject_Unknown().GetType();
				objects.Add((PmdObjectType)JsonSerializer.Deserialize(ref reader, trueDataType, options)!);
				reader.Read();
			}
			return objects;
		}
	}
}
