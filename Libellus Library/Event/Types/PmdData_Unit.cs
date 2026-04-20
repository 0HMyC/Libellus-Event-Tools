using System.Text.Json.Serialization;
using LibellusLibrary.Utils;
using LibellusLibrary.Utils.IO;

namespace LibellusLibrary.Event.Types
{
	internal class PmdData_Unit : PmdDataType, ITypeCreator, IExternalFile, IReferenceType
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonInclude]
		public List<Pmd_UnitDef> Units { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonIgnore]
		public long storeFileOverride = -1;

		public PmdDataType? CreateType(uint version)
		{
			return new PmdData_Unit();
		}

		public async Task LoadExternalFile(string directory)
		{
			foreach(Pmd_UnitDef unit in Units)
			{
				unit.UnitData = await File.ReadAllBytesAsync(Path.Combine(directory, unit.FileName));
			}
		}

		public async Task SaveExternalFile(string directory)
		{
			foreach (Pmd_UnitDef unit in Units)
			{
				await File.WriteAllBytesAsync(Path.Combine(directory, unit.FileName), unit.UnitData);
			}
		}

		// Get total size of all Unit files
		public int GetTotalFileSize()
		{
			int size = 0;
			foreach (Pmd_UnitDef unit in Units)
			{
				// Only align file data to 0x10 if the file is not already aligned
				int modSize = unit.UnitData.Length % 0x10;
				size += modSize == 0 ? unit.UnitData.Length : (unit.UnitData.Length - modSize) + 0x10;
			}
			return size;
		}

		public PmdDataType? ReadType(BinaryReader reader, uint version, List<PmdTypeID> typeIDs, PmdTypeFactory typeFactory)
		{
			Units ??= new();
			long OriginalPos = reader.BaseStream.Position;

			reader.BaseStream.Position = OriginalPos + 0x8;
			uint count = reader.ReadUInt32();
			reader.BaseStream.Position = (long)reader.ReadUInt32();
			long unitStart = reader.FTell();
			for(int i = 0; i < count; i++)
			{
				reader.FSeek(unitStart + i * 0x20);
				Pmd_UnitDef unit = new();
				unit.ReadUnit(reader, typeFactory);
				Units.Add(unit);
			}

			reader.BaseStream.Position = OriginalPos;
			return this;

		}

		internal override void SaveData(PmdBuilder builder, BinaryWriter writer)
		{
			long start = writer.FTell();
			long dataOffset = storeFileOverride == -1 ? start + 0x20 * Units.Count : storeFileOverride;

			for(int i = 0; i < Units.Count; i++)
			{
				writer.FSeek(start + i * 0x20);
				Units[i].WriteUnit(writer, dataOffset);
				dataOffset += Units[i].UnitData.Length;
				// Align dataOffset to 0x10 increment as is needed for RMD files
				long alignedOffset = dataOffset - (dataOffset % 0x10) + 0x10;
				writer.Write(new byte[alignedOffset - dataOffset]);
				dataOffset = alignedOffset;
			}
			// if we overrode the offset to write RMD's to, seek to the end of the Unit headers
			// so that the other tables/files can be stored into the PMD correctly still
			if (storeFileOverride != -1)
			{
				writer.FSeek(start + Units.Count * 0x20);
			}
		}

		public void SetReferences(PmdBuilder pmdBuilder)
		{
			foreach(Pmd_UnitDef unit in Units)
			{
				unit.SetReferences(pmdBuilder);
			}
		}

		internal override int GetCount() => Units.Count;
		internal override int GetSize() => 0x20;
	}
	internal class Pmd_UnitDef: IReferenceType
	{
		public string FileName { get; set; } = string.Empty;
		public byte[] UnitData = Array.Empty<byte>();

		public int MajorID { get; set; }
		public int MinorID { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public ObjectType ModelType { get; set; }

		public int NameIndex;

		public void ReadUnit(BinaryReader reader, PmdTypeFactory typeFactory)
		{
			FileName = typeFactory.GetNameTable(reader)[reader.ReadInt32()];
			reader.ReadInt32(); // FileIndex, should be the same as above
			MajorID = reader.ReadInt32();
			MinorID = reader.ReadInt32();
			int offset = reader.ReadInt32();
			int size = reader.ReadInt32();
			ModelType = (ObjectType)reader.ReadInt32();
			reader.ReadInt32(); // reserve
			long originalpos = reader.BaseStream.Position;
			reader.FSeek(offset);
			UnitData = reader.ReadBytes(size);
			reader.BaseStream.Position = originalpos;
		}
		public void WriteUnit(BinaryWriter writer, long unitDataOffset)
		{
			writer.Write(NameIndex);
			writer.Write(NameIndex);
			writer.Write(MajorID);
			writer.Write(MinorID);
			writer.Write((int)unitDataOffset);
			writer.Write(UnitData.Length);
			writer.Write((int)ModelType);
			writer.Write(0); // Reserve
			writer.FSeek(unitDataOffset);
			writer.Write(UnitData);
		}

		public void SetReferences(PmdBuilder pmdBuilder)
		{
			byte[] temp = Text.StringtoASCII8(FileName);
			Array.Resize(ref temp, 32);
			NameIndex = pmdBuilder.AddReference(PmdTypeID.Name, temp);
		}

		public enum ObjectType
		{
			Attachment = 1,
			Field_Object = 2
		}
	}
}
