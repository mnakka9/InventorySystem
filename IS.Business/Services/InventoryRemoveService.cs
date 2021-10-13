using IS.Business.ViewModels;
using IS.Data.Models;
using IS.Data.Services.CommonRepo;
using IS.Data.Services.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Business.Services
{
    public class InventoryRemoveService : IInventoryRemoveService
    {
        private ICommonRepo commonRepo;
        private IUserRepo userRepo;

        public InventoryRemoveService(ICommonRepo _commonRepo, IUserRepo _userRepo)
        {
            commonRepo = _commonRepo;
            userRepo = _userRepo;
        }

        public void RemoveItemFromUserInventory(InventoryViewModel viewModel)
        {
            var user = userRepo.GetUserById(viewModel.Guid);

            if (!this.IsItemAvailable(viewModel.AvailableItem, user))
            {
                throw new Exception($"There is no {viewModel.AvailableItem.Name} to remove");
            }

            var allSlots = user.Inventory.Slots;

            if (allSlots.Count > 0)
            {
                DeleteItemsIfMultipleSlotsExist(allSlots, user, viewModel);
            }
        }

        public virtual void DeleteItemsIfMultipleSlotsExist(List<Slot> allSlots, User user, InventoryViewModel model)
        {
            var maxCapacity = commonRepo.MaximumAllowedSlotsForType(model.AvailableItem.ItemTypeId);

            var allItems = allSlots.Where(x => x.ItemTypeId == model.AvailableItem.ItemTypeId).Select(s => s.Items);

            int totalItems = allItems.Sum(x => x.Count);

            int diff = totalItems - model.Amount;

            if(diff <= 0)
            {
                var slotsWithItems = allSlots.Where(x => x.ItemTypeId == model.AvailableItem.ItemTypeId).ToList();

                if(slotsWithItems.Count == 1)
                {
                    DeleteSlotOrItems(true, user, slotsWithItems[0], model.AvailableItem, model.Amount);

                    return;
                }

                foreach (var slot in slotsWithItems)
                {
                    DeleteSlotOrItems(true, user, slot, model.AvailableItem, model.Amount);
                }
            }
            else
            {
                while(model.Amount > 0)
                {
                    DeleteMultiSlotItems(user.Inventory.Slots, model, user);
                }
            }
        }

        public void DeleteMultiSlotItems(List<Slot> allSlots, InventoryViewModel viewModel, User user)
        {
            if (GetLowestCapacitySlot(allSlots, viewModel.AvailableItem.ItemTypeId) is var lowestCapacitySlot && lowestCapacitySlot != null)
            {
                bool deleteSlot = CheckIfSlotShouldBeDeleted(lowestCapacitySlot, viewModel.Amount);

                int amount = lowestCapacitySlot.Items.Count;

                if (deleteSlot)
                {
                    DeleteSlotOrItems(true, user, lowestCapacitySlot, viewModel.AvailableItem, viewModel.Amount);
                }
                else
                {
                    DeleteSlotOrItems(false, user, lowestCapacitySlot, viewModel.AvailableItem, viewModel.Amount);
                }

                viewModel.Amount = viewModel.Amount - amount;
            }
        }

        public Slot GetLowestCapacitySlot(List<Slot> allSlots, int itemTypeId)
        {
            var minValue = allSlots.Where(x => x.ItemTypeId == itemTypeId).Min(x => x.Items.Count);
            var lowestCapacitySlot = allSlots.Where(x => x.Items.Count == minValue && x.ItemTypeId == itemTypeId).FirstOrDefault();

            return lowestCapacitySlot;
        }


        public void DeleteSlotOrItems(bool isSlotDelete, User user, Slot slot, AvailableItem item, int amount)
        {
            if (isSlotDelete)
            {
                user.Inventory.Slots.Remove(slot);
            }
            else
            {
                var itemsToRemove = slot.Items.Where(i => i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)).ToList();

                int idex = itemsToRemove.Count - amount - 1;

                user.Inventory.Slots.Where(s => s.Id == slot.Id).FirstOrDefault()?.Items.RemoveRange(idex, amount);
            }
        }

        public virtual bool CheckIfSlotShouldBeDeleted(Slot slot, int amount)
        {
            var maxCapacity = commonRepo.MaximumAllowedSlotsForType(slot.ItemTypeId);

            int diff = maxCapacity - amount;

            amount = diff > 0 ? (slot.Items.Count - amount) : 0;

            return amount <= 0 ? true : false;
        }


        public virtual bool IsItemAvailable(AvailableItem item, User user)
        {
            bool isAvailable = false;

            var slots = user.Inventory.Slots;

            if (slots.Count == 0)
            {
                return false;
            }

            foreach (Slot slot in slots)
            {
                if (slot.Items?.Any(i => i.Name.Equals(item.Name)) ?? false)
                {
                    isAvailable = true;
                    break;
                }
            }

            return isAvailable;
        }
    }
}
