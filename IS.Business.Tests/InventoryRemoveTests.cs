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
    public class InventoryRemoveTests
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

            Mock<InventoryRemoveService> mockService = new Mock<InventoryRemoveService>(commonRepo, userRepo);
            mockService.Setup(m => m.IsItemAvailable(It.IsAny<AvailableItem>(), It.IsAny<User>())).Returns(false);

            mockService.Object.RemoveItemFromUserInventory(model);
        }

        [TestMethod]
        public void Given_UserTriedToRemovedItem_When_ItemExists_Then_DeleteItemsIfMultipleSlotsExist_MethodShouldBeCalled()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = item };
            addService.AddItemToUserInventory(model);

            Mock<InventoryRemoveService> mockService = new Mock<InventoryRemoveService>(commonRepo, userRepo);
            mockService.Setup(m => m.IsItemAvailable(It.IsAny<AvailableItem>(), It.IsAny<User>())).Returns(true);

            mockService.Object.RemoveItemFromUserInventory(model);
            mockService.Verify(m => m.DeleteItemsIfMultipleSlotsExist(It.IsAny<List<Slot>>(), It.IsAny<User>(), It.IsAny<InventoryViewModel>()), Times.Once);
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

            service = new InventoryRemoveService(commonRepo, userRepo);

            service.RemoveItemFromUserInventory(model);
            var actual = user.Inventory.Slots.Count;

            Assert.AreEqual(expected - 1, actual);
        }

        [TestMethod]
        public void Given_UserHas30ItemsInSlot_AndTriesToRemove20_Then_ItemsShouldDeleted()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 30, Guid = userId, AvailableItem = item };

            addService.AddItemToUserInventory(model);

            var user = userRepo.GetUserById(userId);
            var expected = user.Inventory.Slots.Count;

            var itemsCountExpected = user.Inventory.Slots[0].Items.Count - 20;

            model = new InventoryViewModel() { Amount = 20, Guid = userId, AvailableItem = item };

            service = new InventoryRemoveService(commonRepo, userRepo);

            service.RemoveItemFromUserInventory(model);

            var actual = user.Inventory.Slots.Count;
            var itemsCountActual = user.Inventory.Slots[0].Items.Count;

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(itemsCountExpected, itemsCountActual);
         }

        [TestMethod]
        public void Given_UserHas50ItemsInSlot_AndTriesToRemove40_Then_NecessarySlots_ShouldDeleted()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 30, Guid = userId, AvailableItem = item };

            addService.AddItemToUserInventory(model);

            model = new InventoryViewModel() { Amount = 20, Guid = userId, AvailableItem = item };

            addService.AddItemToUserInventory(model);

            var user = userRepo.GetUserById(userId);

            model = new InventoryViewModel() { Amount = 40, Guid = userId, AvailableItem = item };

            service = new InventoryRemoveService(commonRepo, userRepo);

            service.RemoveItemFromUserInventory(model);

            Assert.IsTrue(user.Inventory.Slots.Count == 1);
            Assert.IsTrue(user.Inventory.Slots[0].Items.Count == 10);
        }

        [TestMethod]
        public void Given_UserHas50ItemsInSlot_AndTriesToRemove40_Then_NecessarySlots_WithItems_ShouldDeleted()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 30, Guid = userId, AvailableItem = item };

            addService.AddItemToUserInventory(model);
            addService.AddItemToUserInventory(model);

            var user = userRepo.GetUserById(userId);

            model = new InventoryViewModel() { Amount = 40, Guid = userId, AvailableItem = item };

            service = new InventoryRemoveService(commonRepo, userRepo);

            service.RemoveItemFromUserInventory(model);

            Assert.IsTrue(user.Inventory.Slots.Count == 1);

        }
    }
}
