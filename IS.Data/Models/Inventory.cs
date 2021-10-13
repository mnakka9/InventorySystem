using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Data.Models
{
    public class Inventory
    {
        public int Id {  get; set; }
        public int MaxSlots { get; set; } = 40;
        public List<Slot> Slots {  get; set; } = new List<Slot>();
        public Guid UserId {  get; set; }
    }
}
