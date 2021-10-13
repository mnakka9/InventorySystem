using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Data.Models
{
    public class AvailableItem
    {
        public int Id {  get; set; }
        public string Name {  get; set; }
        public int ItemTypeId {  get; set; }
        public ItemType ItemType {  get; set; }
    }

    public class Slot
    {
        public int Id {  get; set; }
        public int ItemTypeId {  get; set; }
        public int InventoryId {  get; set; }
        public Guid UserId {  get; set; }
        public List<AvailableItem> Items {  get; set; }
        public int ItemsCount {  get; set; }
        public int MaxCount {  get; set; }
    }

    public class ItemType
    {
        public int Id {  get; set; }
        public string Name {  get; set; }
        public int AllowedSlots {  get; set; }
    }
}
