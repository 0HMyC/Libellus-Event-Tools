using static LibellusLibrary.Event.Types.Frame.PmdFrameFactory;

namespace LibellusLibrary.Event.Types.Object
{
	public class PmdObjectFactory
	{
		public static List<PmdObjectType> ReadDataTypes(BinaryReader reader, uint typeTableCount)
		{
			List<PmdObjectType> objects = new();

			for (int i = 0; i < typeTableCount; i++)
			{
				long start = reader.BaseStream.Position;
				PmdObjectType dataType = GetObjectType((PmdTargetTypeID)reader.ReadByte());
				reader.BaseStream.Position = start;
				dataType.ReadObject(reader);
				objects.Add(dataType);
			}
			return objects;
		}

		public static PmdObjectType GetObjectType(PmdTargetTypeID Type) => Type switch
		{
			PmdTargetTypeID.SLIGHT => new PmdObject_SLight(),
			PmdTargetTypeID.PACKMODEL => new PmdObject_PackModel(),
			_ => new PmdObject_Unknown()
		};
	}
}
