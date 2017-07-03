using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Rice.Server.Database.Models
{
    public class QuestState
    {
        [Key]
        public long ID { get; set; }

        public int QID { get; set; }

        public int State { get; set; }
        public int Progress { get; set; }

        public short FailCount { get; set; }

        public long CID { get; set; }
        [ForeignKey("CID")]
        public virtual Character Player { get; set; }
    }
}
