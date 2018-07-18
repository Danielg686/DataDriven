using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace OmegaRace
{
    public class NetworkMessage
    {
        DataMessage_Type data_type;
        DataMessage data;

        public NetworkMessage()
        {

        }

        public void Set_Data(DataMessage _data)
        {
            data = _data;
            data_type = data.type;
        }

        public DataMessage GetData()
        {
            return data;
        }

        public byte[] Serialize()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((int)data_type);

            data.Serialize(ref writer);

            return stream.ToArray();
        }

        public static NetworkMessage Deserialize(byte[] bytes)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(bytes));
            NetworkMessage output = new NetworkMessage();
            output.data_type = (DataMessage_Type)reader.ReadInt32();

            switch (output.data_type)
            {
                case DataMessage_Type.GAME_STATE:
                    output.data = GameStateMessage.Deserialize(ref reader);
                    break;
                case DataMessage_Type.PLAYER_UPDATE:
                    output.data = PlayerUpdateMessage.Deserialize(ref reader);
                    break;
                case DataMessage_Type.MISSLE_MINE:
                    output.data = MissileMineMessage.Deserialize(ref reader);
                    break;
                case DataMessage_Type.ROTATION:
                    output.data = RotationMessage.Deserialize(ref reader);
                    break;
                case DataMessage_Type.FENCE_HIT:
                    output.data = FenceHitMessage.Deserialize(ref reader);
                    break;
                case DataMessage_Type.MISSILEUPDATE:
                    output.data = MissileUpdateMessage.Deserialize(ref reader);
                    break;
                case DataMessage_Type.GAME_OVER:
                    output.data = GAMEOVERMESSAGE.Deserialize(ref reader);
                    break;
                default:
                    Debug.Assert(false, "INVALID DATA TYPE");
                    break;

            }

            return output;
        }

    }



    /*
    [Serializable]
    public struct ClientMessage
    {
        public int horzVal;
        public int vertVal;

        public byte[] Serialize()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(this.horzVal);
            writer.Write(this.vertVal);
            return stream.ToArray();
        }

        public static ClientMessage Deserialize(byte[] bytes)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(bytes));
            ClientMessage output = new ClientMessage();
            output.horzVal = reader.ReadInt32();
            output.vertVal = reader.ReadInt32();
            return output;
        }
    }

    [Serializable]
    public class ServerMessage
    {
        
        public float p1_x;
        public float p1_y;
        public float p2_x;
        public float p2_y;

        public byte[] Serialize()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(this.p1_x);
            writer.Write(this.p1_y);
            writer.Write(this.p2_x);
            writer.Write(this.p2_y);
            return stream.ToArray();
        }

        public static ServerMessage Deserialize(byte[] bytes)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(bytes));
            ServerMessage output = new ServerMessage();
            output.p1_x = reader.ReadSingle();
            output.p1_y = reader.ReadSingle();
            output.p2_x = reader.ReadSingle();
            output.p2_y = reader.ReadSingle();
            return output;
        }
    }
    */
}
