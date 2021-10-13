using IS.Business.ViewModels;
using IS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Business.Services
{
    public interface IInventoryAddService
    {
        void AddItemToUserInventory(InventoryViewModel inventoryViewModel);
        bool AreAllSlotsFilled(List<Slot> slots);
    }
}
