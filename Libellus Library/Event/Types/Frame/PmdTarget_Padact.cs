using LibellusLibrary.JSON;
using System.Text.Json.Serialization;

namespace LibellusLibrary.Event.Types.Frame
{
	internal class PmdTarget_Padact : PmdTargetType
	{
		[JsonPropertyOrder(-92)]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PadactEnum PadactMode { get; set; }

		[JsonPropertyOrder(-91)]
		public byte Field15 { get; set; } // (as in which PADACT entry is it listed under in editor) id? Uncertain

		[JsonPropertyOrder(-90)]
		public ushort Field16 { get; set; }

		[JsonPropertyOrder(-90)]
		public short ZIKAN { get; set; } // Mnemonic = L; limited to 0-3000 in editor

		[JsonPropertyOrder(-90)]
		public short TUYOSA { get; set; } // Mnemonic = P; limited to 0-255 in editor (why not use a byte atlus???)

		[JsonPropertyOrder(-90)]
		public short ON_ZIKAN { get; set; } // Mnemonic = ON; limited to 0-1000 in editor

		[JsonPropertyOrder(-90)]
		public short OFF_ZIKAN { get; set; } // Mnemonic = OFF; limited to 0-1000 in editor

		[JsonPropertyOrder(-90)]
		[JsonConverter(typeof(ByteArrayToHexArray))]
		public byte[] Data { get; set; } = Array.Empty<byte>();

		internal enum PadactEnum : byte
		{
			START = 0,
			STOP = 1
		}

		protected override void ReadData(BinaryReader reader)
		{
			PadactMode = (PadactEnum)reader.ReadByte();
			Field15 = reader.ReadByte();
			Field16 = reader.ReadUInt16();
			ZIKAN = reader.ReadInt16();
			TUYOSA = reader.ReadInt16();
			ON_ZIKAN = reader.ReadInt16();
			OFF_ZIKAN = reader.ReadInt16();
			Data = reader.ReadBytes(28);
		}

		protected override void WriteData(BinaryWriter writer)
		{
			writer.Write((byte)PadactMode);
			writer.Write(Field15);
			writer.Write(Field16);
			writer.Write(ZIKAN);
			writer.Write(TUYOSA);
			writer.Write(ON_ZIKAN);
			writer.Write(OFF_ZIKAN);
			writer.Write(Data);
		}
	}
}
