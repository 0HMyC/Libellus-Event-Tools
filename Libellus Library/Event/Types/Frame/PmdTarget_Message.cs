using LibellusLibrary.JSON;
using System.Text.Json.Serialization;

// Original file used spaces as tabs; retain that for better diffs
namespace LibellusLibrary.Event.Types.Frame
{
    internal class DDSTarget_Message : DDSTargetType
    {
        [JsonPropertyOrder(-95)]
        public byte MessageIndex { get; set; }

        [JsonPropertyOrder(-94)]
        public byte SetBranch { get; set; }

        [JsonPropertyOrder(-93)]
        [JsonConverter(typeof(ByteArrayToHexArray))]
        public byte[] Data { get; set; } = Array.Empty<byte>();

        protected override void ReadData(BinaryReader reader)
        {
            MessageIndex = reader.ReadByte();
            SetBranch = reader.ReadByte();
            Data = reader.ReadBytes(0x1E);
        }

        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(MessageIndex);
            writer.Write(SetBranch);
            writer.Write(Data);
        }
    }

    internal class P3Target_Message : P3TargetType
    {
        [JsonPropertyOrder(-92)]
        public byte MessageIndex { get; set; }

        [JsonPropertyOrder(-91)]
        public byte SetLocalFlag { get; set; }
        // Values range from 0x00 (NOTUSE) to 0xA0 (RESERVE), incrementing by 0x10
        // TODO: Turn values below into array?
        // 0x00 (NOTUSE)
        // 0x10 (0)
        // 0x20 (1)
        // 0x30 (2)
        // 0x40 (3)
        // 0x50 (4)
        // 0x60 (5)
        // 0x70 (HOUKAGO SASOI); Roughly means "After School Invite"
        // 0x80 (KYUJITU SASOI); Probably some kind of Invite given SASOI, unsure of KYUJITU meaning
        // 0x90 (BASHO2 IKU?)
        // 0xA0 (RESERVE)

        [JsonPropertyOrder(-90)]
        public ushort Field16 { get; set; }

        [JsonPropertyOrder(-89)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageModeEnum MessageMode { get; set; }

        [JsonPropertyOrder(-88)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessModeEnum MessageAccessMode { get; set; }

        [JsonPropertyOrder(-87)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DisplayModeEnum MessageDisplayMode { get; set; }

        [JsonPropertyOrder(-86)]
        [JsonConverter(typeof(ByteArrayToHexArray))]
        public byte[] Data { get; set; } = Array.Empty<byte>();

        internal enum MessageModeEnum : byte
        {
            STOP = 0,
            NO_STOP = 1,
        }

        internal enum AccessModeEnum : byte
        {
            DIRECT = 0,
            REF0 = 1,
            REF1 = 2,
            REF2 = 3,
            REF3 = 4,
            REF4 = 5,
            JOUTYUU = 6,
        }

        internal enum DisplayModeEnum : byte
        {
            NORMAL = 0,
            TUTORIAL = 1,
        }

        protected override void ReadData(BinaryReader reader)
        {
            MessageIndex = reader.ReadByte();
            SetLocalFlag = reader.ReadByte();
            Field16 = reader.ReadUInt16();
            MessageMode = (MessageModeEnum)reader.ReadByte();
            MessageAccessMode = (AccessModeEnum)reader.ReadByte();
            MessageDisplayMode = (DisplayModeEnum)reader.ReadByte();
            Data = reader.ReadBytes(33);
        }

        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(MessageIndex);
            writer.Write(SetLocalFlag);
            writer.Write(Field16);
            writer.Write((byte)MessageMode);
            writer.Write((byte)MessageAccessMode);
            writer.Write((byte)MessageDisplayMode);
            writer.Write(Data);
        }
    }
}
