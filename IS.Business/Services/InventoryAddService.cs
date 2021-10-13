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
    public class InventoryAddService : IInventoryAddService
    {
        private ICommonRepo commonRepo;
        private IUserRepo userRepo;

        public InventoryAddService(ICommonRepo _commonRepo, IUserRepo _userRepo)
        {
            commonRepo = _commonRepo;
            userRepo = _userRepo;
        }

        public void AddItemToUserInventory(InventoryViewModel inventoryViewModel)
        {
            try
            {
                int amount = inventoryViewModel.Amount;
                var userId = inventoryViewModel.Guid;
                var item = inventoryViewModel.AvailableItem;

                User user = this.GetUser(userId);

                var slots = user.Inventory.Slots;

                if (AreAllSlotsFilled(slots))
                {
                    throw new Exception("Inventory is full");
                }

                if (!IsSlotAvailabeForItemType(slots, item.ItemTypeId))
                {
                    throw new Exception($"There is no room for {item.Name}");
                }

                if (slots.All(i => i == null) || slots.Count == 0)
                {
                    AddSlotIfInventoryIsEmpty(item, amount, user);

                    return;
                }

                var existingSlots = this.GetExistsingSlots(slots, item.ItemTypeId);

                if (existingSlots != null)
                {
                    Slot capacityAvailableSlot = null;

                    foreach (Slot slot in existingSlots)
                    {
                        var maxCapacity = commonRepo.MaximumAllowedSlotsForType(slot.ItemTypeId);

                        if (slot.Items != null && slot.Items.Count < maxCapacity)
                        {
                            capacityAvailableSlot = slot;
                            break;
                        }
                    }

                    if (capacityAvailableSlot != null)
                    {
                        AddItemsToExistingSlot(capacityAvailableSlot, item, amount, user);
                    }
                    else
                    {
                        var id = user.Inventory.Slots.Count + 1;

                        AddSlotIfInventoryIsEmpty(item, amount, user, id);
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        private void AddItemsToExistingSlot(Slot slot, AvailableItem item, int amount, User user)
        {

            List<AvailableItem> availableItems = new List<AvailableItem>();
            var maxCapacity = commonRepo.MaximumAllowedSlotsForType(item.ItemTypeId) - slot.Items.Count;

            if(maxCapacity == 0)
            {
                var id = user.Inventory.Slots.Count + 1;

                AddSlotIfInventoryIsEmpty(item, amount, user, id);
            }

            amount = amount > maxCapacity ? maxCapacity : amount;

            for (int i = 1; i <= amount; i++)
            {
                availableItems.Add(item);
            }

            slot.Items.AddRange(availableItems);
        }

        public virtual void AddSlotIfInventoryIsEmpty(AvailableItem item, int amount, User user, int id = 1)
        {
            List<AvailableItem> availableItems = new List<AvailableItem>();
            var maxCapacity = commonRepo.MaximumAllowedSlotsForType(item.ItemTypeId);

            amount = amount > maxCapacity ? maxCapacity : amount;

            for (int i = 1; i <= amount; i++)
            {
                availableItems.Add(item);
            }

            Slot slot = new()
            {
                Id = id,
                InventoryId = user.Inventory.Id,
                UserId = user.Id,
                Items = availableItems,
                ItemTypeId = item.ItemTypeId,
                ItemsCount = amount
            };

            user.Inventory.Slots.Add(slot);

            return;
        }

        public virtual bool IsSlotAvailabeForItemType(List<Slot> slots, int itemTypeId)
        {
            bool isAvailable = false;

            if (slots.Any(i => i == null) || slots.Count < 40)
            {
                return true;
            }

            if (!this.AreAllSlotsFilled(slots))
            {
                var currentTypeSlots = this.GetExistsingSlots(slots, itemTypeId);

                foreach (Slot slot in currentTypeSlots)
                {
                    var maxCapacity = commonRepo.MaximumAllowedSlotsForType(slot.ItemTypeId);

                    if (slot.Items != null && slot.Items.Count < maxCapacity)
                    {
                        isAvailable = true;
                        break;
                    }
                }
            }

            return isAvailable;
        }

        public virtual bool AreAllSlotsFilled(List<Slot> slots)
        {
            bool areFilled = true;

            if (slots.Any(i => i == null) || slots.Count < 40)
            {
                return false;
            }
            foreach (Slot slot in slots)
            {
                var maxCapacity = commonRepo.MaximumAllowedSlotsForType(slot.ItemTypeId);

                if (slot.Items != null && slot.Items.Count < maxCapacity)
                {
                    areFilled = false;
                    break;
                }

            }

            return areFilled;
        }

        public List<Slot> GetExistsingSlots(List<Slot> slots, int itemTypeId)
        {
            if (slots == null)
            {
                return null;
            }

            var existingSlots = slots?.Where(s => s.ItemTypeId == itemTypeId)?.ToList();

            if (existingSlots == null) return null;

            return existingSlots;
        }

        public User GetUser(Guid id)
        {
            var user = userRepo.GetUserById(id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }
    }
}
