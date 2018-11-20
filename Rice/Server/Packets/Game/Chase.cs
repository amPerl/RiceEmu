using Rice.Server.Core;
using Rice.Server.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rice.Server.Packets.Game
{

    //180: ChaseBegin
    //183: ChaseEnd
    //185: ChaseProgress
    //187: ChaseHit
    //189: ChaseRequest

    public static class Chase
    {
        [RicePacket(189, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void ChaseRequest(RicePacket packet)
        {
            bool now = packet.Reader.ReadByte() > 0;
            var position = Vector4.Deserialize(packet.Reader);
            Log.WriteLine($"ChaseRequest: Now - {now} | Pos {position}");
        }

        [RicePacket(180, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void ChaseBegin(RicePacket packet)
        {
            int chaseType = packet.Reader.ReadInt32();
            var startPos = Vector4.Deserialize(packet.Reader);
            int firstHuvLevel = packet.Reader.ReadInt32();
            int firstHuvId = packet.Reader.ReadInt32();
            int huvNum = packet.Reader.ReadInt32();

            //Log.WriteLine(
            //    $"ChaseBegin:" +
            //    $"\n\tChaseType: {chaseType}" +
            //    $"\n\tStartPos: {startPos}" +
            //    $"\n\tFirst HUV Lv{firstHuvLevel} ({firstHuvId}) x{huvNum}"
            //);

            var activeQuest = packet.Sender.Player.ActiveCharacter.ActiveQuest;

            var ack = new RicePacket(181); //todo
            ack.Writer.Write(startPos); // struct XiVec4	m_StartPos;		 // this+0x2
            ack.Writer.Write((byte)1); // char	m_Accepted;		 // this+0x12
            ack.Writer.Write(180); // long	m_FindTimeout;		 // this+0x13
            ack.Writer.Write(180); // long	m_ArrestTimeout;		 // this+0x17
            ack.Writer.Write((ushort)huvNum); // unsigned short	m_huvSize;		 // this+0x1B
            for (int i = 0; i < huvNum; i++)
            {
                ack.Writer.Write(packet.Sender.Player.ActiveCharacter.CarSerial); //unsigned short m_Serial;         // this+0x0
                ack.Writer.Write((ushort)(20397 + i)); //unsigned short m_CarSort;        // this+0x2
                if ((activeQuest?.QuestInfo?.HuvLevel ?? 0) <= 0)
                {
                    ack.Writer.Write(firstHuvLevel); //int m_huvLevel;      // this+0x4
                    ack.Writer.Write(firstHuvId); //int m_huvId;         // this+0x8
                }
                else
                {
                    var info = activeQuest.QuestInfo;
                    Log.WriteLine($"Player has active quest {info.Title}, using huv info ({info.HuvLevel}, {info.HuvID})");
                    ack.Writer.Write(info.HuvLevel); //int m_huvLevel;      // this+0x4
                    ack.Writer.Write(info.HuvID); //int m_huvId;         // this+0x8
                }
                ack.Writer.Write(140f); //float m_Speed;       // this+0xC
                ack.Writer.Write(300f); //float m_MaxSpeed;        // this+0x10
                ack.Writer.Write(240f); //float m_NosAccel;        // this+0x14
                ack.Writer.Write(3f); //float m_NosTime;         // this+0x18
                ack.Writer.Write(3f); //float m_NosRefreshRate;      // this+0x1C
                ack.Writer.Write(43f); //float m_Durability;      // this+0x20
                ack.Writer.Write(2f); //float m_FrontPlayerAvoidanceRate;        // this+0x24
                ack.Writer.Write(2f); //float m_FrontTrafficAvoidanceRate;       // this+0x28
                ack.Writer.Write(1f); //float m_RearPlayerAvoidanceRate;		 // this+0x2C
            }
            packet.Sender.Send(ack);
        }

        [RicePacket(185, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void ChaseProgress(RicePacket packet)
        {
            ushort serial = packet.Reader.ReadUInt16();
            ushort targetSerial = packet.Reader.ReadUInt16();
            ushort targetCarSort = packet.Reader.ReadUInt16();
            int time = packet.Reader.ReadInt32();
            int state = packet.Reader.ReadInt32();
            string[] states = "WAIT RUN ARRESTING FAILTOCATCH ENDED".Split(' ');

            //Log.WriteLine(
            //    $"ChaseProgress:" +
            //    $"\n\tSerial: {serial}" +
            //    $"\n\tTargetSerial: {targetSerial}" +
            //    $"\n\tTargetSort: {targetCarSort}" +
            //    $"\n\tTime: {time}" +
            //    $"\n\tState: {states[state]} ({state})"
            //);

            var ack = new RicePacket(185);
            ack.Writer.Write(serial);
            ack.Writer.Write(targetSerial);
            ack.Writer.Write(targetCarSort);
            ack.Writer.Write(time);
            ack.Writer.Write(state);
            packet.Sender.Send(ack);
        }

        [RicePacket(187, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void ChaseHit(RicePacket packet)
        {
            ushort serial = packet.Reader.ReadUInt16();
            ushort targetSerial = packet.Reader.ReadUInt16();
            ushort targetCarSort = packet.Reader.ReadUInt16();
            float damage = packet.Reader.ReadSingle();
            int time = packet.Reader.ReadInt32();
            uint flags = packet.Reader.ReadUInt32();
            float resultLife = packet.Reader.ReadSingle();
            string name = packet.Reader.ReadUnicodeStatic(10);
            ushort what = packet.Reader.ReadUInt16();
            ushort what2 = packet.Reader.ReadUInt16();
            //Log.WriteLine(
            //    $"ChaseHit:" +
            //    $"\n\tSerial: {serial}" +
            //    $"\n\tTargetSerial: {targetSerial}" +
            //    $"\n\tTargetSort: {targetCarSort}" +
            //    $"\n\tDamage: {damage}" +
            //    $"\n\tTime: {time}" +
            //    $"\n\tFlags: {flags}" +
            //    $"\n\tResultLife: {resultLife}" +
            //    $"\n\tName: {name}" +
            //    $"\n\tWhat: {what}" +
            //    $"\n\tWhat2: {what2}"
            //);
        }

        [RicePacket(183, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void ChaseEnd(RicePacket packet)
        {
            ushort serial = packet.Reader.ReadUInt16();
            ushort carSort = packet.Reader.ReadUInt16();
            uint type = packet.Reader.ReadUInt32();
            float life = packet.Reader.ReadSingle();
            uint result = packet.Reader.ReadUInt32();
            var pos = Vector4.Deserialize(packet.Reader);
            var vel = Vector4.Deserialize(packet.Reader);
            int time = packet.Reader.ReadInt32();
            Log.WriteLine(
                $"ChaseEnd:" +
                $"\n\tBuffer: {BitConverter.ToString(packet.Buffer)}" +
                $"\n\tSerial: {serial}" +
                $"\n\tCarSort: {carSort}" +
                $"\n\tType: {type}" +
                $"\n\tLife: {life}" +
                $"\n\tResult: {result}" +
                $"\n\tPosition: {pos}" +
                $"\n\tVelocity: {vel}" +
                $"\n\tTime: {time}"
            );
            
            var ack = new RicePacket(184); // ChaseResult
            ack.Writer.Write(result);
            ack.Writer.Write(time);
            ack.Writer.Write((ushort)1); // unitSize - player count?
            ack.Writer.Write(new byte[4 * 4 + 4 * 4]); // int selfdaily[4]; int teamdaily[4];

            ack.Writer.WriteUnicodeStatic(packet.Sender.Player.ActiveCharacter.Name, 0x20);
            ack.Writer.Write(serial);
            ack.Writer.Write(carSort);
            ack.Writer.Write(0); // deltaHuvMoney
            ack.Writer.Write(0); // deltaBonusMoney
            ack.Writer.Write(0L); // money
            ack.Writer.Write(0); // deltaHuvExp
            ack.Writer.Write(0); // deltaBonusExp
            ack.Writer.Write(packet.Sender.Player.ActiveCharacter.GetExpInfo());
            ack.Writer.Write(packet.Sender.Player.ActiveCharacter.Level);
            ack.Writer.Write(1f); // point?

            ack.Writer.Write(0); // rewardItemCount
            ack.Writer.Write(0); // reward 1
            ack.Writer.Write(0); // reward 2
            packet.Sender.Send(ack);
        }
    }
}
