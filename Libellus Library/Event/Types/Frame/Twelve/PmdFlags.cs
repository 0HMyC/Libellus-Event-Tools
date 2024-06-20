using System.Text.Json.Serialization;

// Original file used spaces as tabs; retain that for better diffs
namespace LibellusLibrary.Event.Types.Frame.Twelve
{
    public class PmdFlags
    {
        [JsonPropertyOrder(-100)]
        public ushort FlagNumber { get; set; }

        [JsonPropertyOrder(-99)]
        public ushort CompareValue { get; set; }

        [JsonPropertyOrder(-98)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitFlagType FlagType { get; set; }

        [JsonPropertyOrder(-97)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitGFlagType GlobalFlagType { get; set; }

        internal void ReadData(BinaryReader reader)
        {
            FlagNumber = reader.ReadUInt16();
            CompareValue = reader.ReadUInt16();
            FlagType = (UnitFlagType)reader.ReadUInt16();
            GlobalFlagType = (UnitGFlagType)reader.ReadUInt16();
        }

        internal void WriteData(BinaryWriter writer)
        {
            writer.Write(FlagNumber);
            writer.Write(CompareValue);
            writer.Write((ushort)FlagType);
            writer.Write((ushort)GlobalFlagType);
        }

        public enum UnitFlagType : short
        {
            DISABLE = 0,
            LOCAL = 1,
            GLOBAL = 2,
        }

        // GFlagType == GlobalFlagType
        public enum UnitGFlagType : short
        {
            EVT = 0,
            COMMU = 1,
            SYS = 2,
        }
    }
}
