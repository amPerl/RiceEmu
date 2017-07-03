using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Rice.Server;
using Rice.Server.Structures;
using Rice.Server.Structures.Resources;
using Models = Rice.Server.Database.Models;

namespace Rice.Game
{
    public class Quest
    {
        public ulong CID;
        public int QID;
        public int State;
        public int Progress;
        public ushort FailCount;

        public QuestTableEntry QuestInfo;

        private Quest(int qid)
        {
            QID = qid;
            QuestInfo = QuestTable.Quests.FirstOrDefault(q => q.TableIndex == QID);
        }

        private Quest(Models.QuestState questState)
        {
            QID = questState.QID;
            QuestInfo = QuestTable.Quests.FirstOrDefault(q => q.TableIndex == QID);

            CID = (ulong) questState.CID;

            State = questState.State;
            Progress = questState.Progress;
            FailCount = (ushort) questState.FailCount;
        }

        public static List<Quest> Retrieve(ulong cid)
        {
            using (var rc = Database.GetContext())
            {
                var quests = rc.QuestStates.Where(q => q.CID == (long) cid).ToList();
                return quests.Select(q => new Quest(q)).ToList();
            }
        }

        public static Quest Retrieve(ulong cid, int qid)
        {
            using (var rc = Database.GetContext())
            {
                var questState = rc.QuestStates.SingleOrDefault(q => q.CID == (long)cid && q.QID == qid);
                return questState != null ? new Quest(questState) : null;
            }
        }

        public static Quest CreateEntry(ulong cid, int qid)
        {
            using (var rc = Database.GetContext())
            {
                var character = rc.Characters.Find((long) cid);
                if (character == null)
                {
                    Log.WriteError($"Could find character {cid} for Quest.CreateEntry");
                    return null;
                }

                rc.QuestStates.Add(new Models.QuestState
                {
                    QID = qid,
                    Player = character,
                    State = 0,
                    Progress = 0,
                    FailCount = 0
                });
                rc.SaveChanges();
                return new Quest(qid) { CID = cid };
            }
        }

        public bool IncrementProgress()
        {
            using (var rc = Database.GetContext())
            {
                var questState = rc.QuestStates.SingleOrDefault(q => q.CID == (long)CID && q.QID == QID);
                if (questState == null)
                {
                    Log.WriteError($"Could find QuestState with CID {CID} for Quest.IncrementProgress");
                    return false;
                }

                questState.Progress = ++Progress;

                rc.SaveChanges();
                return true;
            }
        }

        public bool SetState(int state)
        {
            using (var rc = Database.GetContext())
            {
                var questState = rc.QuestStates.SingleOrDefault(q => q.CID == (long) CID && q.QID == QID);
                if (questState == null)
                {
                    Log.WriteError($"Could find QuestState with CID {CID} for Quest.IncrementProgress");
                    return false;
                }

                State = state;
                questState.State = State;

                rc.SaveChanges();
                return true;
            }
        }

        public bool Fail(bool gaveUp = false)
        {

            using (var rc = Database.GetContext())
            {
                var questState = rc.QuestStates.SingleOrDefault(q => q.CID == (long) CID && q.QID == QID);
                if (questState == null)
                {
                    Log.WriteError($"Could find QuestState with CID {CID} for Quest.IncrementProgress");
                    return false;
                }

                State = gaveUp ? 4 : 3;
                FailCount++;
                questState.State = State;
                questState.FailCount = (short)FailCount;

                rc.SaveChanges();
                return true;
            }
        }
    }
}
