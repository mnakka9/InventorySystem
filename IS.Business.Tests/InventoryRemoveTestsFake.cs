using IS.Business.Services;
using IS.Business.ViewModels;
using IS.Data.Models;
using IS.Data.Services.CommonRepo;
using IS.Data.Services.UserRepo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IS.Business.Tests
{
    [TestClass]
    public class InventoryRemoveTestsFake
    {
        private IInventoryRemoveService service;
        private IInventoryAddService addService;
        private ICommonRepo commonRepo;
        private IUserRepo userRepo;

        [TestInitialize]
        public void Init()
        {
            commonRepo = new CommonRepo();
            userRepo = new UserRepo();
            addService = new InventoryAddService(commonRepo, userRepo);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Given_UserTriedToRemovedItem_When_ItemDoesnotExists_Then_ThrowsAnError()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = item };

            service = new RemoveFakeService();

            service.RemoveItemFromUserInventory(model);
        }

        [TestMethod]
        public void Given_UserHas20ItemsInSlot_AndTriesToRemove30_Then_Slot_ShouldBeDeleted()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 20, Guid = userId, AvailableItem = item };

            addService.AddItemToUserInventory(model);

            var user = userRepo.GetUserById(userId);
            var expected = user.Inventory.Slots.Count;

            model = new InventoryViewModel() { Amount = 30, Guid = userId, AvailableItem = item };

            service = new FakeService(commonRepo, userRepo);

            service.RemoveItemFromUserInventory(model);
            var actual = user.Inventory.Slots.Count;

            Assert.AreEqual(expected - 1, actual);
        }
    }

    internal class FakeService : IInventoryRemoveService
    {
        private ICommonRepo commonRepo;
        private IUserRepo userRepo;
        public FakeService(ICommonRepo commonRepo, IUserRepo userRepo)
        {
            this.commonRepo = commonRepo;
            this.userRepo = userRepo;
        }

        public void RemoveItemFromUserInventory(InventoryViewModel viewModel)
        {
            var user = userRepo.GetUserById(viewModel.Guid);
            var allSlots = user.Inventory.Slots;

            if (allSlots.Count > 0)
            {
                var getExistingSlot = allSlots.Where(i => i.ItemTypeId == viewModel.AvailableItem.ItemTypeId)?.FirstOrDefault();

                if (getExistingSlot != null)
                {
                    var maxCapacity = commonRepo.MaximumAllowedSlotsForType(getExistingSlot.ItemTypeId);

                    var amount = getExistingSlot.Items.Count - maxCapacity;

                    var deleteSlot = amount <= 0 ? true : false;

                    if (deleteSlot)
                    {
                        user.Inventory.Slots.Remove(getExistingSlot);
                    }
                    else
                    {
                        getExistingSlot.Items.RemoveAll( i => i.Name.Equals(viewModel.AvailableItem.Name, StringComparison.OrdinalIgnoreCase) );
                    }
                }
            }
        }

        public bool IsItemAvailable(AvailableItem item, User user)
        {
            return true;
        }
    }

    public class RemoveFakeService : IInventoryRemoveService
    {
        public bool IsItemAvailable(AvailableItem item, User user)
        {
            return false;
        }
    }
}
