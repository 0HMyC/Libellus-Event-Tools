﻿using LibellusLibrary.JSON;
using System.Text.Json.Serialization;
using static LibellusLibrary.Event.Types.Frame.PmdFrameFactory;

namespace LibellusLibrary.Event.Types.Frame
{
	public class PmdTargetType
	{
		[JsonPropertyOrder(-100)]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PmdTargetTypeID TargetType { get; set; }
		[JsonPropertyOrder(-99)]
		public ushort StartFrame { get; set; }
		[JsonPropertyOrder(-98)]
		public ushort Length { get; set; }
		[JsonPropertyOrder(-97)]
		public short NameIndex { get; set; } // P3 SLIGHT replaces this with a (u)int called HOKAN; TODO: figure out how to get a P3Target_Slight made...

		public virtual void ReadFrame(BinaryReader reader) => throw new NotImplementedException();
		public virtual void WriteFrame(BinaryWriter writer) => throw new NotImplementedException();
		protected virtual void ReadData(BinaryReader reader) => throw new InvalidOperationException();
		protected virtual void WriteData(BinaryWriter writer) => throw new InvalidOperationException();
	}

	// Some SMT3 framefuncs don't use the Length param as the length it seems? TODO: Figure that out
	public class SMT3TargetType : PmdTargetType
	{
		public override void ReadFrame(BinaryReader reader)
		{
			TargetType = (PmdTargetTypeID)reader.ReadUInt16();
			StartFrame = reader.ReadUInt16();
			Length = reader.ReadUInt16();
			NameIndex = reader.ReadInt16();
			ReadData(reader);
		}
		public override void WriteFrame(BinaryWriter writer)
		{
			writer.Write((ushort)TargetType);
			writer.Write(StartFrame);
			writer.Write(Length);
			writer.Write(NameIndex);
			WriteData(writer);
		}
	}

	public class DDSTargetType : PmdTargetType
	{
		[JsonPropertyOrder(-96)]
		public uint Field08 { get; set; } // Unknown what this is/does ATM
		public override void ReadFrame(BinaryReader reader)
		{
			TargetType = (PmdTargetTypeID)reader.ReadUInt16();
			StartFrame = reader.ReadUInt16();
			Length = reader.ReadUInt16();
			NameIndex = reader.ReadInt16();
			Field08 = reader.ReadUInt32();
			ReadData(reader);
		}
		public override void WriteFrame(BinaryWriter writer)
		{
			writer.Write((ushort)TargetType);
			writer.Write(StartFrame);
			writer.Write(Length);
			writer.Write(NameIndex);
			writer.Write(Field08);
			WriteData(writer);
		}
	}

	public class P3TargetType : PmdTargetType
	{
		[JsonPropertyOrder(-96)]
		public byte ResourceID { get; set; }
		[JsonPropertyOrder(-95)]
		public byte ResourceType { get; set; }
		[JsonPropertyOrder(-94)]
		public short Field0A { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonPropertyOrder(-93)]
		public PmdFlags Flags { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public override void ReadFrame(BinaryReader reader)
		{
			TargetType = (PmdTargetTypeID)reader.ReadUInt16();
			StartFrame = reader.ReadUInt16();
			Length = reader.ReadUInt16();
			NameIndex = reader.ReadInt16();
			ResourceID = reader.ReadByte();
			ResourceType = reader.ReadByte();
			Field0A = reader.ReadInt16();
			Flags = new PmdFlags();
			Flags.ReadData(reader);
			ReadData(reader);
		}
		public override void WriteFrame(BinaryWriter writer)
		{
			writer.Write((ushort)TargetType);
			writer.Write(StartFrame);
			writer.Write(Length);
			writer.Write(NameIndex);
			writer.Write(ResourceID);
			writer.Write(ResourceType);
			writer.Write(Field0A);
			Flags.WriteData(writer);
			WriteData(writer);
		}
	}

	internal class SMT3Target_Unknown : SMT3TargetType
	{
		[JsonPropertyOrder(-96)]
		[JsonConverter(typeof(ByteArrayToHexArray))]
		public byte[] Data { get; set; } = Array.Empty<byte>();

		protected override void ReadData(BinaryReader reader)
		{
			Data = reader.ReadBytes(8);
		}

		protected override void WriteData(BinaryWriter writer)
		{
			writer.Write(Data);
		}
	}

	internal class DDSTarget_Unknown : DDSTargetType
	{
		[JsonPropertyOrder(-95)]
		[JsonConverter(typeof(ByteArrayToHexArray))]
		public byte[] Data { get; set; } = Array.Empty<byte>();

		protected override void ReadData(BinaryReader reader)
		{
			Data = reader.ReadBytes(0x20);
		}

		protected override void WriteData(BinaryWriter writer)
		{
			writer.Write(Data);
		}
	}

	internal class P3Target_Unknown : P3TargetType
	{
		[JsonPropertyOrder(-92)]
		[JsonConverter(typeof(ByteArrayToHexArray))]
		public byte[] Data { get; set; } = Array.Empty<byte>();

		protected override void ReadData(BinaryReader reader)
		{
			Data = reader.ReadBytes(0x28);
		}

		protected override void WriteData(BinaryWriter writer)
		{
			writer.Write(Data);
		}
	}
}
