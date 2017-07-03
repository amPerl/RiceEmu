using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Rice.Server.Database.Models
{
    public class User
    {
        [Key]
        public long ID { get; set; }

        [StringLength(32)]
        public string Name { get; set; }

        [StringLength(32)]
        public string PasswordHash { get; set; }

        public byte Status { get; set; }

        [StringLength(15)]
        public string CreateIP { get; set; }

        // HanCoin / GoodBoyPoints
        public long Credits { get; set; }

        public virtual ICollection<Character> Characters { get; set; } 
    }
}
