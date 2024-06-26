﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibellusLibrary.JSON
{
	class ByteArrayToHexArray : JsonConverter<byte[]>
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(byte[]);
		}

		public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.String)
			{
				string? hex = reader.GetString();
				if (!string.IsNullOrEmpty(hex))
				{
					hex = hex.Replace(" ", string.Empty);
					return Enumerable.Range(0, hex.Length)
						 .Where(x => x % 2 == 0)
						 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
						 .ToArray();
				}
			}
			return Enumerable.Empty<byte>().ToArray();
		}

		public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
		{
			string? @string = BitConverter.ToString(value).Replace("-", " ");
			writer.WriteStringValue(@string);
		}
	}
	class ListByteArrayToHexArray : JsonConverter<List<byte[]>>
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(List<byte[]>);
		}

		public override List<byte[]>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			List<byte[]> result = new();
			reader.Read();
			while (reader.TokenType != JsonTokenType.EndArray)
			{
				string? hex = reader.GetString();
				if (!string.IsNullOrEmpty(hex))
				{
					hex = hex.Replace(" ", string.Empty);
					result.Add(Enumerable.Range(0, hex.Length)
						 .Where(x => x % 2 == 0)
						 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
						 .ToArray());
				}
				reader.Read();

			}
			return result;
		}
		public override void Write(Utf8JsonWriter writer, List<byte[]> value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			ByteArrayToHexArray converter = new();
			foreach (byte[] item in value)
			{
				converter.Write(writer, item, options);
			}
			writer.WriteEndArray();
		}
	}
}