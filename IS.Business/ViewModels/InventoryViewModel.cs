using IS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Business.ViewModels
{
    public class InventoryViewModel
    {
        public int Amount {  get; set; }
        public Guid Guid {  get; set; }
        public AvailableItem AvailableItem {  get; set; }
    }
}
