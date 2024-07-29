using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_v1.Models
{
    public class Mark
    {
        public int MarkId { get; set; }
        public int Value { get; set; }
        public DateTime DateGiven { get; set; }
        public string Subject { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
