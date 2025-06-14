using System.Text.Json.Serialization;
using LibellusLibrary.Event.Types.Object;

namespace LibellusLibrary.Event.Types
{
	internal class PmdData_ObjectTable : PmdDataType, ITypeCreator
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonConverter(typeof(PmdObjectReader))]
		public List<PmdObjectType> Objects { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public PmdDataType? CreateType(uint version)
		{
			return version switch
			{
				9 => new PmdData_DDSObjectTable(),
				10 or 11 or 12 => new PmdData_ObjectTable(),
				_ => new PmdData_RawData()
			};
		}

		public PmdDataType? ReadType(BinaryReader reader, uint version, List<PmdTypeID> typeIDs, PmdTypeFactory typeFactory)
		{
			// If v9/DDS PMD, return appropriate class for reading
			// TODO: Better way of handling this
			if (version == 9)
			{
				PmdData_DDSObjectTable ddsObjTbl = new();
				ddsObjTbl.ReadType(reader, version, typeIDs, typeFactory);
				return ddsObjTbl;
			}

			long OriginalPos = reader.BaseStream.Position;

			reader.BaseStream.Position = OriginalPos + 0x4;
			int size = reader.ReadInt32();
			uint count = reader.ReadUInt32();
			reader.BaseStream.Position = (long)reader.ReadUInt32();

			if (version != 10 && version != 11 && version != 12)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Version {version} ObjectTable is not supported! Reading data to Hex array...");
				Console.ResetColor();
				PmdData_RawData unsupportedObjectTable = new();
				for (int i = 0; i < count; i++)
				{
					unsupportedObjectTable.Data.Add(reader.ReadBytes(size));
				}
				reader.BaseStream.Position = OriginalPos;
				return unsupportedObjectTable;
			}

			Objects = PmdObjectFactory.ReadDataTypes(reader, count);
			reader.BaseStream.Position = OriginalPos;
			return this;
		}

		internal override void SaveData(PmdBuilder builder, BinaryWriter writer)
		{
			foreach(PmdObjectType obj in Objects)
			{
				obj.WriteObject(writer);
			}
		}
		internal override int GetCount() => Objects.Count;
		internal override int GetSize() => 0x14;
	}

	public class PmdData_DDSObjectTable : PmdDataType
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonConverter(typeof(DDSObjectReader))]
		public List<PmdObjectType> Objects { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public PmdDataType? ReadType(BinaryReader reader, uint version, List<PmdTypeID> typeIDs, PmdTypeFactory typeFactory)
		{
			long OriginalPos = reader.BaseStream.Position;

			reader.BaseStream.Position = OriginalPos + 0x8;
			uint count = reader.ReadUInt32();
			reader.BaseStream.Position = (long)reader.ReadUInt32();

			// Read DDS Objects
			Objects = new List<PmdObjectType>();
			for (int i = 0; i < count; i++)
			{
				DDSObject_Unknown ddsObj = new();
				ddsObj.ReadObject(reader);
				Objects.Add(ddsObj);
			}

			reader.BaseStream.Position = OriginalPos;
			return this;
		}

		internal override void SaveData(PmdBuilder builder, BinaryWriter writer)
		{
			foreach (PmdObjectType obj in Objects)
			{
				obj.WriteObject(writer);
			}
		}
		internal override int GetCount() => Objects.Count;
		internal override int GetSize() => 0x8;
	}
}
