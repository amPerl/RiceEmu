using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rice.Server.Core;
using Rice.Server.Structures;

namespace Rice.Server.Packets.Game
{
    public static class Quest
    {
        [RicePacket(272, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void MyQuestList(RicePacket packet)
        {
            var quests = Rice.Game.Quest.Retrieve(packet.Sender.Player.ActiveCharacter.CID);

            var ack = new RicePacket(273);
            ack.Writer.Write(quests.Count);
            foreach (var quest in quests)
            {
                ack.Writer.Write(quest.QID);
                ack.Writer.Write(quest.State);
                ack.Writer.Write(quest.Progress);
                ack.Writer.Write(quest.FailCount);
            }
            packet.Sender.Send(ack);
        }

        [RicePacket(262, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void QuestStart(RicePacket packet)
        {
            var questId = packet.Reader.ReadInt32();

            var quest = Rice.Game.Quest.Retrieve(packet.Sender.Player.ActiveCharacter.CID, questId);
            quest = quest ?? Rice.Game.Quest.CreateEntry(packet.Sender.Player.ActiveCharacter.CID, questId);

            packet.Sender.Player.ActiveCharacter.Quest = quest;

            var ack = new RicePacket(263);
            ack.Writer.Write(quest.QID);
            ack.Writer.Write((uint) quest.FailCount);
            packet.Sender.Send(ack);
            // TODO: verify if player is eligible
        }

        [RicePacket(264, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void QuestComplete(RicePacket packet)
        {
            var questId = packet.Reader.ReadInt32();
            var totalTime = packet.Reader.ReadSingle();

            var quest = packet.Sender.Player.ActiveCharacter.Quest;
            if (quest == null || quest.QID != questId)
            {
                packet.Sender.Error("something's fucky");
                packet.Sender.Kill("Active quest and quest complete mismatch.");
                return;
            }

            quest.SetState(1);

            var ack = new RicePacket(265);
            ack.Writer.Write(questId);
            packet.Sender.Send(ack);
            // TODO: verify quest completion
        }

        [RicePacket(266, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void QuestReward(RicePacket packet)
        {
            var questId = packet.Reader.ReadInt32();

            var quest = packet.Sender.Player.ActiveCharacter.Quest;
            if (quest == null || quest.QID != questId)
            {
                packet.Sender.Error("something's fucky");
                packet.Sender.Kill("Active quest and quest reward mismatch.");
                return;
            }

            quest.SetState(2);
            var rewards = quest.QuestInfo.GetRewards();
            packet.Sender.Player.ActiveCharacter.Quest = null;

            int expGotten = quest.QuestInfo.Experience;
            int moneyGotten = quest.QuestInfo.Mito;

            packet.Sender.Player.ActiveCharacter.GrantExperience(expGotten);
            packet.Sender.Player.ActiveCharacter.GrantMito((ulong)moneyGotten);

            var ack = new RicePacket(267);
            ack.Writer.Write(questId);
            ack.Writer.Write(expGotten);
            ack.Writer.Write(moneyGotten);
            ack.Writer.Write(packet.Sender.Player.ActiveCharacter.GetExpInfo());
            ack.Writer.Write(packet.Sender.Player.ActiveCharacter.Level);
            ack.Writer.Write((ushort) rewards.Length); // reward item num
            Console.WriteLine("{0}", ack.Writer.BaseStream.Position);
            for (int i = 0; i < 3; i++)
            {
                // jesus christ i need to clean up this horrible hack
                for (; i < rewards.Length; i++)
                {
                    var itemID = rewards[i];
                    int tblIdx = Structures.Resources.ItemTable.Items.FindIndex(itm => itm.ID == itemID);
                    if (tblIdx == -1)
                    {
                        ack.Writer.Write(0);
                        continue;
                    }
                    Rice.Game.Item resultItem;
                    packet.Sender.Player.ActiveCharacter.GrantItem(itemID, 1, out resultItem);
                    ack.Writer.Write((uint) tblIdx);
                }
                if (i < 3)
                    ack.Writer.Write(0);
            }
            packet.Sender.Send(ack);
            // TODO: use real quest data, update player info
        }

        [RicePacket(268, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void QuestFail(RicePacket packet)
        {
            var questId = packet.Reader.ReadInt32();

            var quest = packet.Sender.Player.ActiveCharacter.Quest;
            if (quest == null || quest.QID != questId)
            {
                packet.Sender.Error("something's fucky");
                packet.Sender.Kill("Active quest and quest fail mismatch.");
                return;
            }

            quest.Fail();
            packet.Sender.Player.ActiveCharacter.Quest = null;

            var ack = new RicePacket(269);
            ack.Writer.Write(questId);
            packet.Sender.Send(ack);
        }

        [RicePacket(270, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void QuestGiveUp(RicePacket packet)
        {
            var questId = packet.Reader.ReadInt32();

            var quest = packet.Sender.Player.ActiveCharacter.Quest;
            if (quest == null || quest.QID != questId)
            {
                packet.Sender.Error("something's fucky");
                packet.Sender.Kill("Active quest and quest abandon mismatch.");
                return;
            }

            quest.Fail(true);
            packet.Sender.Player.ActiveCharacter.Quest = null;

            var ack = new RicePacket(271);
            ack.Writer.Write(questId);
            packet.Sender.Send(ack);
        }

        [RicePacket(274, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void QuestGoalPlace(RicePacket packet)
        {
            var questId = packet.Reader.ReadInt32();

            var quest = packet.Sender.Player.ActiveCharacter.Quest;
            if (quest == null || quest.QID != questId)
            {
                packet.Sender.Error("something's fucky");
                packet.Sender.Kill("Active quest and quest progress mismatch.");
                return;
            }

            quest.IncrementProgress();
        }
    }
}