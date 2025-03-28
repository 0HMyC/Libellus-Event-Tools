using LibellusLibrary.JSON;
using System.Text.Json.Serialization;

namespace LibellusLibrary.Event.Types.Frame
{
	internal class P3Target_Jump : P3TargetType
	{
		[JsonPropertyOrder(-92)]
		public short ToFrame { get; set; }

		[JsonPropertyOrder(-91)]
		[JsonConverter(typeof(ByteArrayToHexArray))]
		public byte[] Data { get; set; } = Array.Empty<byte>();

		protected override void ReadData(BinaryReader reader)
		{
			ToFrame = reader.ReadInt16();
			Data = reader.ReadBytes(38);
		}

		protected override void WriteData(BinaryWriter writer)
		{
			writer.Write(ToFrame);
			writer.Write(Data);
		}
	}
}
