using LibellusLibrary.JSON;
using System.Text.Json.Serialization;

namespace LibellusLibrary.Event.Types
{
	internal class PmdData_SLight : PmdDataType, ITypeCreator
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonInclude]
		public List<Pmd_SLightDef> SLights { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public PmdDataType? CreateType(uint version)
		{
			return version switch
			{
				9 or 10 or 11 or 12 => new PmdData_SLight(),
				_ => new PmdData_RawData(),
			};
		}

		public PmdDataType? ReadType(BinaryReader reader, uint version, List<PmdTypeID> typeIDs, PmdTypeFactory typeFactory)
		{
			long OriginalPos = reader.BaseStream.Position;

			reader.BaseStream.Position = OriginalPos + 0x8;
			uint count = reader.ReadUInt32();

			reader.BaseStream.Position = OriginalPos + 0xC;
			reader.BaseStream.Position = (long)reader.ReadUInt32();

			SLights = new();
			for (int i = 0; i < count; i++)
			{
				Pmd_SLightDef currentSLight = new();
				currentSLight.ReadSLight(reader);
				SLights.Add(currentSLight);
			}

			reader.BaseStream.Position = OriginalPos;
			return this;
		}
		
		internal override void SaveData(PmdBuilder builder, BinaryWriter writer)
		{
			foreach(Pmd_SLightDef sli in SLights)
			{
				sli.WriteSLight(writer);
			}
		}
		internal override int GetCount() => SLights.Count;
		internal override int GetSize() => 0x30;
	}

	internal class Pmd_SLightDef
	{
		[JsonPropertyOrder(-100)]
		public float DirectionRed { get; set; }
		[JsonPropertyOrder(-99)]
		public float DirectionGreen { get; set; }
		[JsonPropertyOrder(-98)]
		public float DirectionBlue { get; set; }
		[JsonPropertyOrder(-97)]
		public float Field0C { get; set; } // Unknown; alpha?

		[JsonPropertyOrder(-96)]
		public float DirectionYaw { get; set; } // editor name: DIR Y; limited to 0-359 (whole numbers)
		[JsonPropertyOrder(-95)]
		public float DirectionPitch { get; set; } // editor name: DIR X; limited to 0-359 (whole numbers)
		
		[JsonPropertyOrder(-94)]
		public float Field18 { get; set; }
		[JsonPropertyOrder(-93)]
		public float Field1C { get; set; }

		[JsonPropertyOrder(-92)]
		public float AmbientRed { get; set; }
		[JsonPropertyOrder(-91)]
		public float AmbientGreen { get; set; }
		[JsonPropertyOrder(-90)]
		public float AmbientBlue { get; set; }
		[JsonPropertyOrder(-89)]
		public float Field2C { get; set; } // Unknown; alpha?

		public void ReadSLight(BinaryReader reader)
		{
			DirectionRed = reader.ReadSingle();
			DirectionGreen = reader.ReadSingle();
			DirectionBlue = reader.ReadSingle();
			Field0C = reader.ReadSingle();
			DirectionYaw = reader.ReadSingle();
			DirectionPitch = reader.ReadSingle();
			Field18 = reader.ReadSingle();
			Field1C = reader.ReadSingle();
			AmbientRed = reader.ReadSingle();
			AmbientGreen = reader.ReadSingle();
			AmbientBlue = reader.ReadSingle();
			Field2C = reader.ReadUInt32();
		}

		public void WriteSLight(BinaryWriter writer)
		{
			writer.Write(DirectionRed);
			writer.Write(DirectionGreen);
			writer.Write(DirectionBlue);
			writer.Write(Field0C);
			writer.Write(DirectionYaw);
			writer.Write(DirectionPitch);
			writer.Write(Field18);
			writer.Write(Field1C);
			writer.Write(AmbientRed);
			writer.Write(AmbientGreen);
			writer.Write(AmbientBlue);
			writer.Write(Field2C);
		}
	}
}
