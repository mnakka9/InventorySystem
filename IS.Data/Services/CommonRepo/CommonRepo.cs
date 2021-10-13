using IS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Data.Services.CommonRepo
{
    public class CommonRepo : ICommonRepo
    {
        public List<AvailableItem> GetAvailableItems()
        {
            List<ItemType> availableItems = GetItemTypes();
            return new List<AvailableItem>()
            {
                new AvailableItem() { Id= 1, Name= "Softwood", ItemType= availableItems[0], ItemTypeId = 1 },
                new AvailableItem() { Id= 2, Name= "Hardwood", ItemType=availableItems[0] , ItemTypeId = 1},
                new AvailableItem() { Id= 3, Name= "Tree branch", ItemType=availableItems[0] , ItemTypeId = 1},
                new AvailableItem() { Id= 4, Name= "Bronze", ItemType=availableItems[1] , ItemTypeId = 2},
                new AvailableItem() { Id= 5, Name= "Silver", ItemType=availableItems[1] , ItemTypeId = 2},
                new AvailableItem() { Id= 6, Name= "Gold", ItemType=availableItems[1], ItemTypeId = 2 },
                new AvailableItem() { Id= 7, Name= "Ruby", ItemType=availableItems[2], ItemTypeId = 3 },
                new AvailableItem() { Id= 8, Name= "Sapphire", ItemType=availableItems[2], ItemTypeId = 3 },
                new AvailableItem() { Id= 9, Name= "Diamond", ItemType=availableItems[2] , ItemTypeId = 3}
            };
        }

        public List<ItemType> GetItemTypes()
        {
            return new List<ItemType>()
            {
                new ItemType() { Id = 1, Name = "Wood", AllowedSlots = 30 },
                new ItemType() { Id = 2, Name = "Metal", AllowedSlots = 20 },
                new ItemType() { Id = 3, Name = "Mineral", AllowedSlots = 10 }
            };
        }

        public int MaximumAllowedSlotsForType(int typeId)
        {
            var item = GetItemTypes().Find(x => x.Id == typeId);

            if (item == null)
            {
                return 0;
            }

            return item.AllowedSlots;
        }
    }
}
