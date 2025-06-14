using LibellusLibrary.Utils;

namespace LibellusLibrary.Event.Types
{
	internal class PmdData_Message : PmdDataType, ITypeCreator, IExternalFile, IReferenceType
	{
		public string FileName { get; set; } = string.Empty;

		public byte[] MessageData = Array.Empty<byte>();

		public PmdDataType? CreateType(uint version)
		{
			return new PmdData_Message();
		}

		public PmdDataType? ReadType(BinaryReader reader, uint version, List<PmdTypeID> typeIDs, PmdTypeFactory typeFactory)
		{
			long OriginalPos = reader.BaseStream.Position;

			reader.BaseStream.Position += 0x4;
			int size = reader.ReadInt32();
			uint count = reader.ReadUInt32();
			if (count == 0)
			{
				reader.BaseStream.Position = OriginalPos;
				return this;
			}
			reader.BaseStream.Position = (long)reader.ReadUInt32();

			MessageData = reader.ReadBytes(size);
			foreach (string name in typeFactory.GetNameTable(reader))
			{
				// TODO: Maybe use a var instead of calling ToLower twice here?
				// Not super important but it might maybe be more performant/resource efficient lol
				if (name.ToLower().EndsWith(".msg"))
				{
					// Check if original filename is uppercase via extension,
					// using uppercase .BMD extensions if it is for accuracy
					FileName = string.Concat(name.AsSpan(0, name.LastIndexOf('.')), name.EndsWith(".MSG") ? ".BMD" : ".bmd");
					break;
				}
				else if (name.ToLower().EndsWith(".bmd"))
				{
					FileName = name;
					break;
				}
			}
			if (FileName == string.Empty)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Could not locate BMD filename! Saving as Message.bmd!");
				Console.ResetColor();
				FileName = "Message.bmd";
			}

			reader.BaseStream.Position = OriginalPos;
			return this;
		}

		public async Task SaveExternalFile(string directory)
		{
			if (FileName == string.Empty)
			{
				return;
			}
			await File.WriteAllBytesAsync(Path.Combine(directory, FileName), MessageData);
		}

		public async Task LoadExternalFile(string directory)
		{
			if (FileName == string.Empty)
			{
				return;
			}
			MessageData = await File.ReadAllBytesAsync(Path.Combine(directory, FileName));
		}

		public int GetTotalFileSize() => GetSize();

		internal override void SaveData(PmdBuilder builder, BinaryWriter writer)
		{
			writer.Write(MessageData);
			return;
		}
		
		internal override int GetSize() => MessageData.Length;
		internal override int GetCount() => FileName == string.Empty ? 0 : 1;

		public void SetReferences(PmdBuilder pmdBuilder)
		{
			if (GetCount() == 0)
			{
				return;
			}
			if (FileName.ToLower().EndsWith(".bmd"))
			{
				// Check if original filename is uppercase via extension,
				// using uppercase .BMD extensions if it is for accuracy
				FileName = string.Concat(FileName.AsSpan(0, FileName.LastIndexOf('.')), FileName.EndsWith(".BMD") ? ".MSG" : ".msg");
			}
			byte[] temp = Text.StringtoASCII8(FileName);
			Array.Resize(ref temp, 32);
			pmdBuilder.AddReference(PmdTypeID.Name, temp);
		}
	}
}
