﻿namespace LibellusLibrary.Event.Types.Frame
{
	public class PmdFrameFactory
	{
		public List<PmdTargetType> ReadDataTypes(BinaryReader reader, uint typeTableCount, uint version)
		{
			List<PmdTargetType> frames = new();

			for (int i = 0; i < typeTableCount; i++)
			{
				long start = reader.BaseStream.Position;
				PmdTargetTypeID curTargetType = (PmdTargetTypeID)reader.ReadUInt16();
				PmdTargetType dataType = version switch
				{
					4 => GetSMT3FrameType(curTargetType),
					9 => GetDDSFrameType(curTargetType),
					10 or 11 or 12 => GetP3FrameType(curTargetType),
					_ => throw new NotImplementedException()
				};
				if (dataType is ITargetVarying varyingDataType) { dataType = varyingDataType.GetVariant(reader); }
				reader.BaseStream.Position = start;
				dataType.ReadFrame(reader);
				frames.Add(dataType);
			}
			return frames;
		}

		public static PmdTargetType GetSMT3FrameType(PmdTargetTypeID Type) => Type switch
		{
			PmdTargetTypeID.QUAKE => new SMT3Target_Quake(),
			_ => new SMT3Target_Unknown()
		};

		public static PmdTargetType GetDDSFrameType(PmdTargetTypeID Type) => Type switch
		{
			PmdTargetTypeID.UNIT => new DDSTarget_Unit(),
			PmdTargetTypeID.MESSAGE => new DDSTarget_Message(),
			PmdTargetTypeID.QUAKE => new DDSTarget_Quake(),
			PmdTargetTypeID.SLIGHT => new DDSTarget_Slight(),
			PmdTargetTypeID.RAIN => new DDSTarget_Rain(),
			PmdTargetTypeID.MOVIE => new DDSTarget_Movie(),
			_ => new DDSTarget_Unknown()
		};

		// TODO: Rename classes to fit P3Target_X convention
		public static PmdTargetType GetP3FrameType(PmdTargetTypeID Type) => Type switch
		{
			PmdTargetTypeID.UNIT => new P3Target_Unit(),
			PmdTargetTypeID.MESSAGE => new P3Target_Message(),
			PmdTargetTypeID.SE => new P3Target_Se(),
			PmdTargetTypeID.FADE => new P3Target_Fade(),
			PmdTargetTypeID.QUAKE => new P3Target_Quake(),
			PmdTargetTypeID.SLIGHT => new P3Target_Slight(),
			PmdTargetTypeID.BGM => new P3Target_BGM(),
			PmdTargetTypeID.PADACT => new P3Target_Padact(),
			PmdTargetTypeID.MOVIE => new P3Target_Movie(),
			PmdTargetTypeID.CTLCAM => new P3Target_Ctlcam(),
			PmdTargetTypeID.WAIT => new P3Target_Wait(),
			PmdTargetTypeID.B_UP => new P3Target_Bustup(),
			PmdTargetTypeID.CUTIN => new P3Target_Cutin(),
			PmdTargetTypeID.EVENT_EFFECT => new P3Target_EventEffect(),
			PmdTargetTypeID.JUMP => new P3Target_Jump(),
			PmdTargetTypeID.KEYFREE => new P3Target_Keyfree(),
			PmdTargetTypeID.RANDOMJUMP => new P3Target_RandomJump(),
			PmdTargetTypeID.CUSTOMEVENT => new P3Target_CustomEvent(),
			PmdTargetTypeID.CONDJUMP => new P3Target_CondJump(),
			PmdTargetTypeID.COND_ON => new P3Target_CondOn(),
			PmdTargetTypeID.COMULVJUMP => new P3Target_ComuLvJump(),
			PmdTargetTypeID.COUNTJUMP => new P3Target_CountJump(),
			PmdTargetTypeID.HOLYJUMP => new P3Target_HolyJump(),
			PmdTargetTypeID.FIELDOBJ => new P3Target_FieldObj(),
			PmdTargetTypeID.PACKMODEL => new P3Target_Unit(),
			PmdTargetTypeID.FIELDEFF => new P3Target_FieldEff(),
			PmdTargetTypeID.SPUSE => new P3Target_SpuSe(),
			PmdTargetTypeID.SCRIPT => new P4Target_Script(),
			PmdTargetTypeID.FOG => new P4Target_Fog(),
			PmdTargetTypeID.FLDNOISE => new P4Target_FldNoise(),
			PmdTargetTypeID.CAMERA_STATE => new P4Target_CameraState(),
			_ => new P3Target_Unknown()
		};

		// Names + ID's taken from P4G; earlier games have different names in their binaries
		public enum PmdTargetTypeID
		{
			STAGE = 0,
			UNIT = 1,
			CAMERA = 2,
			EFFECT = 3,
			MESSAGE = 4,
			SE = 5,
			FADE = 6,
			QUAKE = 7,
			BLUR = 8,
			LIGHT = 9,
			SLIGHT = 10,
			SFOG = 11,
			SKY = 12,
			BLUR2 = 13,
			MBLUR = 14,
			DBLUR = 15,
			FILTER = 16,
			MFILTER = 17,
			BED = 18,
			BGM = 19,
			MG1 = 20,
			MG2 = 21,
			FB = 22,
			RBLUR = 23,
			TMX = 24,
			RAIN = 25,
			EPL = 26,
			HBLUR = 27,
			PADACT = 28,
			MOVIE = 29,
			TIMEI = 30,
			RENDERTEX = 31,
			BISTA = 32,
			CTLCAM = 33,
			WAIT = 34,
			B_UP = 35,
			CUTIN = 36,
			EVENT_EFFECT = 37,
			JUMP = 38,
			KEYFREE = 39,
			RANDOMJUMP = 40,
			CUSTOMEVENT = 41,
			CONDJUMP = 42,
			COND_ON = 43,
			COMULVJUMP = 44,
			COUNTJUMP = 45,
			HOLYJUMP = 46,
			FIELDOBJ = 47,
			PACKMODEL = 48,
			FIELDEFF = 49,
			SPUSE = 50,
			SCRIPT = 51,
			BLURFILTER = 52,
			FOG = 53,
			ENV = 54,
			FLDSKY = 55,
			FLDNOISE = 56,
			CAMERA_STATE = 57
		}
	}
}
