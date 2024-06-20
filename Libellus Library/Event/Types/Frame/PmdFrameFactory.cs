namespace LibellusLibrary.Event.Types.Frame
{
	public class PmdFrameFactory
	{
		public List<PmdTargetType> ReadDataTypes(BinaryReader reader, uint typeTableCount, uint version)
		{
			List<PmdTargetType> frames = new();

			for (int i = 0; i < typeTableCount; i++)
			{
				long start = reader.BaseStream.Position;
				PmdTargetType dataType = GetFrameType((PmdTargetTypeID)reader.ReadUInt16());
				reader.BaseStream.Position = start;
				dataType.ReadFrame(reader);
				frames.Add(dataType);
			}
			return frames;
		}

		public static PmdTargetType GetFrameType(PmdTargetTypeID Type) => Type switch
		{
			PmdTargetTypeID.UNIT => new PmdTarget_Unit(),
			PmdTargetTypeID.MESSAGE => new PmdTarget_Message(),
			PmdTargetTypeID.FADE => new PmdTarget_Fade(),
			PmdTargetTypeID.QUAKE => new PmdTarget_Quake(),
			PmdTargetTypeID.BGM => new PmdTarget_Bgm(),
			PmdTargetTypeID.PADACT => new PmdTarget_Padact(),
			PmdTargetTypeID.CTLCAM => new PmdTarget_Ctlcam(),
			PmdTargetTypeID.CUTIN => new PmdTarget_Cutin(),
			PmdTargetTypeID.JUMP => new PmdTarget_Jump(),
			PmdTargetTypeID.CUSTOMEVENT => new PmdTarget_CustomEvent(),
			PmdTargetTypeID.SCRIPT => new PmdTarget_Script(),
			PmdTargetTypeID.FOG => new PmdTarget_Fog(),
			_ => new PmdTarget_Unknown()
		};

		// Names + ID's taken from P4G; earlier games have different strings in their binaries
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
			// RAIN = 25, TODO: Confirm this? Seemingly not present in "used" P4G Event Editor strings?
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
