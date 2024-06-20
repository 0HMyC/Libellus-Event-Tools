using LibellusLibrary.Event.Types.Frame;
using System.Text.Json.Serialization;

namespace LibellusLibrary.Event.Types
{
	internal class PmdData_FrameTable : PmdDataType, ITypeCreator
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonConverter(typeof(PmdFrameReader))]
		public List<PmdTargetType> Frames { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public PmdDataType? CreateType(uint version)
		{
			return version switch
			{
				// 9 => new Ver9_FrameTable(),
				12 => new PmdData_FrameTable(),
				_ => new PmdData_RawData()
			};
		}

		public PmdDataType? ReadType(BinaryReader reader, uint version, List<PmdTypeID> typeIDs, PmdTypeFactory typeFactory)
		{
			long OriginalPos = reader.BaseStream.Position;

			reader.BaseStream.Position += 0x8;
			uint count = reader.ReadUInt32();
			reader.BaseStream.Position = (long)reader.ReadUInt32();

			PmdFrameFactory factory = new();
			Frames = factory.ReadDataTypes(reader, count, version);

			reader.BaseStream.Position = OriginalPos;
			return this;
		}

		internal override void SaveData(PmdBuilder builder, BinaryWriter writer)
		{
			foreach(PmdTargetType frame in Frames)
			{
				frame.WriteFrame(writer);
			}
		}
		internal override int GetCount() => Frames.Count;
		internal override int GetSize() => 0x3C;

	}
	/*internal class Ver9_FrameTable : PmdDataType, ITypeCreator
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonConverter(typeof(PmdFrameReader))]
		public List<PmdTargetType> Frames { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public PmdDataType? CreateType(uint version) => throw new NotImplementedException();

		public PmdDataType? ReadType(BinaryReader reader, uint version, List<PmdTypeID> typeIDs, PmdTypeFactory typeFactory)
		{
			long OriginalPos = reader.BaseStream.Position;

			reader.BaseStream.Position += 0x8;
			uint count = reader.ReadUInt32();
			reader.BaseStream.Position = (long)reader.ReadUInt32();

			// TODO: PmdFrameFactory needs to be versionable for ver9 support
			// PmdFrameFactory factory = new();
			// Frames = factory.ReadDataTypes(reader, count);

			reader.BaseStream.Position = OriginalPos;
			return this;
		}
		internal override int GetCount() => Frames.Count;
		internal override int GetSize() => 0x2C;
	}*/
}
