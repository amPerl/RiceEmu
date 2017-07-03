using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Rice.Server.Database.Models
{
    public class Character
    {
        [Key]
        public long ID { get; set; }

        [StringLength(32)]
        public string Name { get; set; }

        public long Mito { get; set; }
        public long Experience { get; set; }

        public int Avatar { get; set; }
        public int Level { get; set; }

        public int City { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public int PosState { get; set; }

        public int CurrentCarID { get; set; }
        public int GarageLevel { get; set; }

        public long UID { get; set; }
        [ForeignKey("UID")]
        public virtual User Owner { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }

        public long TID { get; set; }
        // TODO: Crew relation
    }
}
