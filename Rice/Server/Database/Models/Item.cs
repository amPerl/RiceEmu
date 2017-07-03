using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Rice.Server.Database.Models
{
    public class Item
    {
        [Key]
        public long ID { get; set; }

        [StringLength(32)]
        public string ItemID { get; set; }

        public long CID { get; set; }
        [ForeignKey("CID")]
        public virtual Character Owner { get; set; }

        public int CurCarID { get; set; }
        public int LastCarID { get; set; }
        
        public short State { get; set; }
        public short Slot { get; set; }
        public int StackNum { get; set; }

        public int AssistA { get; set; }
        public int AssistB { get; set; }
        public int AssistC { get; set; }
        public int AssistD { get; set; }
        public int AssistE { get; set; }
        public int AssistF { get; set; }
        public int AssistG { get; set; }
        public int AssistH { get; set; }
        public int AssistI { get; set; }
        public int AssistJ { get; set; }

        public int Boxed { get; set; }
        public int Bound { get; set; }
        public int UpgradeLevel { get; set; }
        public int UpgradePoints { get; set; }

        //public DateTime ExpireTimestamp { get; set; }
        public float ItemHealth { get; set; }

        public int InvIdx { get; set; }
        public int Random { get; set; }
    }
}
