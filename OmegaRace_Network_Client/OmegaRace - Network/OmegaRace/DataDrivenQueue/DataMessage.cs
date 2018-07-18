using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace OmegaRace
{
    public enum DataMessage_Type
    {
        GAME_STATE = 0,
        PLAYER_UPDATE,
        PLAYER_INPUT,
        COLLISION,
        FENCE_HIT,
        SOUND,
        MISSLE_MINE,
        ROTATION,
        MISSILEUPDATE,
        GAME_OVER
    }
    public enum SEND_TYPE
    {
        LOCAL = 0,
        NETWORKED
    }

    [Serializable]
    public abstract class DataMessage
    {
        public DataMessage_Type type;
        public SEND_TYPE sendType;

        public DataMessage()
        {

        }

        public DataMessage(DataMessage_Type t, SEND_TYPE _sendType)
        {
            type = t;
            sendType = _sendType;
        }

        public abstract void Serialize(ref BinaryWriter writer);

    }

    [Serializable]
    public class PlayerInputMessage : DataMessage
    {
        public int playerID;
        public int horzInput;
        public int vertInput;
        public bool fireMissile;
        public bool layMine;

        public PlayerInputMessage()
        {
            type = DataMessage_Type.PLAYER_INPUT;
        }

        public PlayerInputMessage(SEND_TYPE _sendType, int _playerID, int _horzInput, int _vertInput, bool _fireMissile, bool _layMine)
            : base(DataMessage_Type.PLAYER_INPUT, _sendType)
        {
            playerID = _playerID;
            horzInput = _horzInput;
            vertInput = _vertInput;
            fireMissile = _fireMissile;
            layMine = _layMine;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(this.playerID);
            writer.Write(this.horzInput);
            writer.Write(this.vertInput);
            writer.Write(this.fireMissile);
            writer.Write(this.layMine);
        }

        public static PlayerInputMessage Deserialize(ref BinaryReader reader)
        {
            PlayerInputMessage output = new PlayerInputMessage();
            output.playerID = reader.ReadInt32();
            output.horzInput = reader.ReadInt32();
            output.vertInput = reader.ReadInt32();
            output.fireMissile = reader.ReadBoolean();
            output.layMine = reader.ReadBoolean();
            return output;
        }

    }

    [Serializable]
    public class GameStateMessage : DataMessage
    {
        public GAME_STATE state;

        public GameStateMessage()
        {
            type = DataMessage_Type.GAME_STATE;
        }

        public GameStateMessage(SEND_TYPE _sendType, GAME_STATE _state)
            : base(DataMessage_Type.GAME_STATE, _sendType)
        {
            state = _state;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write((int)this.state);
        }

        public static GameStateMessage Deserialize(ref BinaryReader reader)
        {
            GameStateMessage output = new GameStateMessage();
            output.state = (GAME_STATE)reader.ReadInt32();
            return output;
        }

    }


    [Serializable]
    public class PlayerUpdateMessage : DataMessage
    {
        public int playerID;
        public float x;
        public float y;
        

        public PlayerUpdateMessage()
        {
            type = DataMessage_Type.PLAYER_UPDATE;
            sendType = SEND_TYPE.NETWORKED;
        }

        public PlayerUpdateMessage(int _playerID, float _x, float _y)
            : base(DataMessage_Type.PLAYER_UPDATE, SEND_TYPE.NETWORKED)
        {
            playerID = _playerID;
            x = _x;
            y = _y;
            
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(this.playerID);
            writer.Write(this.x);
            writer.Write(this.y);
        }

        public static PlayerUpdateMessage Deserialize(ref BinaryReader reader)
        {
            PlayerUpdateMessage output = new PlayerUpdateMessage();
            output.playerID = reader.ReadInt32();
            output.x = reader.ReadSingle();
            output.y = reader.ReadSingle();
            return output;
        }
    }

    [Serializable]
    public class FenceHitMessage : DataMessage
    {

        public int fence_id;

        public FenceHitMessage()
            : base(DataMessage_Type.FENCE_HIT, SEND_TYPE.NETWORKED)
        {

        }

        public FenceHitMessage(int _id)
            : base(DataMessage_Type.FENCE_HIT, SEND_TYPE.NETWORKED)
        {
            fence_id = _id;
        }


        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(fence_id);
        }

        public static FenceHitMessage Deserialize(ref BinaryReader reader)
        {
            FenceHitMessage output = new FenceHitMessage();
            output.fence_id = reader.ReadInt32();
            return output;
        }

    }

    [Serializable]
    public class CollisionMessage : DataMessage
    {
        public bool Collision;

        public CollisionMessage()
        {
            type = DataMessage_Type.COLLISION;
        }

        public CollisionMessage(bool _Collision)
            : base(DataMessage_Type.COLLISION, SEND_TYPE.NETWORKED)
        {
            Collision = _Collision;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(this.Collision);
        }

        public static CollisionMessage Deserialize(ref BinaryReader reader)
        {
            CollisionMessage output = new CollisionMessage();
            output.Collision = reader.ReadBoolean();
            return output;
        }
    }

    [Serializable]
    public class MissileMineMessage : DataMessage
    {
        public int PlayerID;
        public bool pMissile;
        public bool Mine;
        public float direction_x;
        public float direction_y;

        public MissileMineMessage()
        {
            type = DataMessage_Type.MISSLE_MINE;
            sendType = SEND_TYPE.NETWORKED;
        }

        public MissileMineMessage(int _pID, bool _missile, bool _mine, float _direction_x, float _direction_y)
            : base(DataMessage_Type.MISSLE_MINE, SEND_TYPE.NETWORKED)
        {
            PlayerID = _pID;
            pMissile = _missile;
            Mine = _mine;
            direction_x = _direction_x;
            direction_y = _direction_y;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(this.PlayerID);
            writer.Write(this.pMissile);
            writer.Write(this.Mine);
            writer.Write(this.direction_x);
            writer.Write(this.direction_y);
        }

        public static MissileMineMessage Deserialize(ref BinaryReader reader)
        {
            MissileMineMessage output = new MissileMineMessage();
            output.PlayerID = reader.ReadInt32();
            output.pMissile = reader.ReadBoolean();
            output.Mine = reader.ReadBoolean();
            output.direction_x = reader.ReadSingle();
            output.direction_y = reader.ReadSingle();
            return output;
        }

    }

    [Serializable]

    public class RotationMessage : DataMessage
    {
        public int playerID;
        public float input_Ang;


        public RotationMessage()
        {
            type = DataMessage_Type.ROTATION;
            sendType = SEND_TYPE.NETWORKED;
        }
        
        public RotationMessage(int _playerID, float _input_Ang)
            :base(DataMessage_Type.ROTATION, SEND_TYPE.NETWORKED)
        {
            playerID = _playerID;
            input_Ang = _input_Ang;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(this.playerID);
            writer.Write(this.input_Ang);
        }

        public static RotationMessage Deserialize(ref BinaryReader reader)
        {
            RotationMessage output = new RotationMessage();
            output.playerID = reader.ReadInt32();
            output.input_Ang = reader.ReadSingle();
            return output;
        }
    }

    [Serializable]

    public class MissileUpdateMessage : DataMessage
    {
        public int MissileID;
        public float x_pos;
        public float y_pos;

        public MissileUpdateMessage()
        {
            type = DataMessage_Type.MISSILEUPDATE;
            sendType = SEND_TYPE.NETWORKED;
        }

        public MissileUpdateMessage(int ID, float _x_pos, float _y_pos)
            :base(DataMessage_Type.MISSILEUPDATE, SEND_TYPE.NETWORKED)
        {
            MissileID = ID;
            x_pos = _x_pos;
            y_pos = _y_pos;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(this.MissileID);
            writer.Write(this.x_pos);
            writer.Write(this.y_pos);
        }

        public static MissileUpdateMessage Deserialize(ref BinaryReader reader)
        {
            MissileUpdateMessage output = new MissileUpdateMessage();
            output.MissileID = reader.ReadInt32();
            output.x_pos = reader.ReadSingle();
            output.y_pos = reader.ReadSingle();
            return output;
        }
    }

    [Serializable]

    public class GAMEOVERMESSAGE : DataMessage
    {
        public int playerID;

        public GAMEOVERMESSAGE()
        {
            type = DataMessage_Type.GAME_OVER;
            sendType = SEND_TYPE.NETWORKED;
        }

        public GAMEOVERMESSAGE(int ID)
            :base(DataMessage_Type.GAME_OVER, SEND_TYPE.NETWORKED)
        {
            playerID = ID;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(this.playerID);
        }

        public static GAMEOVERMESSAGE Deserialize(ref BinaryReader reader)
        {
            GAMEOVERMESSAGE output = new GAMEOVERMESSAGE();
            output.playerID = reader.ReadInt32();
            return output;
        }
    }
}
