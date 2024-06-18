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
		public short NameIndex { get; set; }
		[JsonPropertyOrder(-96)]
		public byte FBNResourceID { get; set; }
		[JsonPropertyOrder(-95)]
		public byte Field09 { get; set; }
		[JsonPropertyOrder(-94)]
		public short Field0A { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonPropertyOrder(-93)]
		public PmdFlags Flags { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public void ReadFrame(BinaryReader reader)
		{
			TargetType = (PmdTargetTypeID)reader.ReadUInt16();
			StartFrame = reader.ReadUInt16();
			Length = reader.ReadUInt16();
			NameIndex = reader.ReadInt16();
			FBNResourceID = reader.ReadByte();
			Field09 = reader.ReadByte();
			Field0A = reader.ReadInt16();
			Flags = new PmdFlags();
			Flags.ReadData(reader);
			ReadData(reader);
		}
		public void WriteFrame(BinaryWriter writer)
		{
			writer.Write((ushort)TargetType);
			writer.Write(StartFrame);
			writer.Write(Length);
			writer.Write(NameIndex);
			writer.Write(FBNResourceID);
			writer.Write(Field09);
			writer.Write(Field0A);
			Flags.WriteData(writer);
			WriteData(writer);
		}
		protected virtual void ReadData(BinaryReader reader) => throw new InvalidOperationException();
		protected virtual void WriteData(BinaryWriter writer) => throw new InvalidOperationException();
	}

	internal class PmdTarget_Unknown : PmdTargetType
	{
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
