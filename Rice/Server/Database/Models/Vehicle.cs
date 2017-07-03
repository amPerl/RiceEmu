using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Rice.Server.Database.Models
{
    public class Vehicle
    {
        [Key]
        public long ID { get; set; }

        public long CID { get; set; }
        [ForeignKey("CID")]
        public virtual Character Owner { get; set; }

        public int CarID { get; set; }
        public int CarType { get; set; }

        public int Color { get; set; }
        public int Grade { get; set; }
        public int AuctionCount { get; set; }

        public float Mitron { get; set; }
        public float Kms { get; set; }
    }
}
