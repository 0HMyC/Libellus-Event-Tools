using LibellusLibrary.JSON;
using System.Text.Json.Serialization;
using static LibellusLibrary.Event.Types.Frame.PmdFrameFactory;

namespace LibellusLibrary.Event.Types.Object
{
	public class PmdObjectType
	{
		[JsonPropertyOrder(-100)]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PmdTargetTypeID ObjectID { get; set; }

		[JsonPropertyOrder(-99)]
		public byte SlotOrID_Field01 { get; set; }
		[JsonPropertyOrder(-98)]
		public short NameIndex { get; set; } // NameIndex definitely true for v9, unsure about v10+ but probably same?
		[JsonPropertyOrder(-97)]
		public short Field04 { get; set; }

		public void ReadObject(BinaryReader reader)
		{
			ObjectID = (PmdTargetTypeID)reader.ReadByte();
			SlotOrID_Field01 = reader.ReadByte();
			NameIndex = reader.ReadInt16();
			Field04 = reader.ReadInt16();
			ReadData(reader);
		}

		public void WriteObject(BinaryWriter writer)
		{
			writer.Write((byte)ObjectID);
			writer.Write(SlotOrID_Field01);
			writer.Write(NameIndex);
			writer.Write(Field04);
			WriteData(writer);
		}
		protected virtual void ReadData(BinaryReader reader) => throw new InvalidOperationException();
		protected virtual void WriteData(BinaryWriter writer) => throw new InvalidOperationException();
	}

	internal class PmdObject_Unknown : PmdObjectType
	{
		[JsonPropertyOrder(-96)]
		[JsonConverter(typeof(ByteArrayToHexArray))]
		public byte[] Data { get; set; } = Array.Empty<byte>();
		[JsonPropertyOrder(-95)]
		public uint Field10 { get; set; }
		protected override void ReadData(BinaryReader reader)
		{
			Data = reader.ReadBytes(0xA);
			Field10 = reader.ReadUInt32();
		}

		protected override void WriteData(BinaryWriter writer)
		{
			writer.Write(Data);
			writer.Write(Field10);
		}
	}

	internal class DDSObject_Unknown : PmdObjectType
	{
		[JsonPropertyOrder(-96)]
		public byte Field06 { get; set; }
		[JsonPropertyOrder(-95)]
		public byte Field07 { get; set; }
		protected override void ReadData(BinaryReader reader)
		{
			Field06 = reader.ReadByte();
			Field07 = reader.ReadByte();
		}

		protected override void WriteData(BinaryWriter writer)
		{
			writer.Write(Field06);
			writer.Write(Field07);
		}
	}
}
