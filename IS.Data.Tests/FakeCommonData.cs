using IS.Data.Models;
using IS.Data.Services.CommonRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Data.Tests
{
    internal class FakeCommonData : ICommonRepo
    {
        public List<AvailableItem> GetAvailableItems()
        {
            return new List<AvailableItem>()
            {
                new AvailableItem() { Id= 1, Name= "WoodTest", ItemType=new ItemType() {Id = 1, Name = "Wood", AllowedSlots = 30 } },
                new AvailableItem() { Id= 2, Name= "WoodTest2", ItemType=new ItemType() {Id = 1, Name = "Wood", AllowedSlots = 30 } },
                new AvailableItem() { Id= 3, Name= "MetalTest", ItemType=new ItemType() { Id = 2, Name = "Metal", AllowedSlots = 20 } },
                new AvailableItem() { Id= 4, Name= "MetalTest2", ItemType=new ItemType() { Id = 2, Name = "Metal", AllowedSlots = 20 } },
                new AvailableItem() { Id= 5, Name= "MineralTest", ItemType=new ItemType() { Id = 3, Name = "Mineral", AllowedSlots = 10 } },
                new AvailableItem() { Id= 6, Name= "MineralTest2", ItemType=new ItemType() { Id = 3, Name = "Mineral", AllowedSlots = 10 } }
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

            if(item == null)
            {
                return 0;
            }

            return item.AllowedSlots;
        }
    }
}
