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
    public class InventoryAddTests
    {
        private InventoryAddService service;
        private ICommonRepo commonRepo;
        private IUserRepo userRepo;

        [TestInitialize]
        public void Init()
        {
            commonRepo = new CommonRepo();
            userRepo = new UserRepo();
            service = new InventoryAddService(commonRepo, userRepo);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Given_UserTriedToAddAnItem_WhenAllSlotsAreFilled_ShouldThrowAnError_SayingSlotsAreFull()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = item };

            Mock<InventoryAddService> mockService = new Mock<InventoryAddService>(commonRepo, userRepo);
            mockService.Setup(m => m.AreAllSlotsFilled(It.IsAny<List<Slot>>())).Returns(true);

            mockService.Object.AddItemToUserInventory(model);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Given_UserTriedToAddAnItem_WhenNoSlotAvailableForItem_ShouldThrowAnError_WithNoRoomForItemMessage()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = item };

            Mock<InventoryAddService> mockService = new Mock<InventoryAddService>(commonRepo, userRepo);
            mockService.Setup(m => m.IsSlotAvailabeForItemType(It.IsAny<List<Slot>>(), It.IsAny<int>())).Returns(false);

            mockService.Object.AddItemToUserInventory(model);
        }

        [TestMethod]
        public void Given_UserTriedToAddAnItem_WithAllSlotsAreFree_ShouldCreateSlot_AndAddItems_VerifyMethodCall()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var user = userRepo.GetUserById(userId);
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = item };

            Mock<InventoryAddService> mockService = new Mock<InventoryAddService>(commonRepo, userRepo);
            mockService.Setup(m => m.IsSlotAvailabeForItemType(It.IsAny<List<Slot>>(), It.IsAny<int>())).Returns(true);

            mockService.Object.AddItemToUserInventory(model);
            mockService.Verify(i => i.AddSlotIfInventoryIsEmpty(item, 10, user, 1), Times.Once);
        }

        [TestMethod]
        public void Given_UserTriedToAddAnItem_WithAllSlotsAreFree_ShouldCreateSlot_AndAddItems_VerifySlotCount()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var user = userRepo.GetUserById(userId);

            service.AddSlotIfInventoryIsEmpty(item, 10, user);

            Assert.IsTrue(user.Inventory.Slots.Count > 0);
        }

        [TestMethod]
        public void Given_UserTriedToAddAnItem_WithAnExistingSlot_ShouldAddTheItem_ToSlot()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var user = userRepo.GetUserById(userId);
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = item };

            //Adding an intial slot for empty slot
            service.AddSlotIfInventoryIsEmpty(item, 10, user);

            // Now add same item
            service.AddItemToUserInventory(model);

            Assert.IsTrue(user.Inventory.Slots[0].Items.Count > 10);
        }

        [TestMethod]
        public void Given_UserTriedToAddAnItem_WithAnExistingSlotFullCapacity_ShouldCreateNewSlot_WithSameType_AndAddTheItem_ToSlot()
        {
            AvailableItem item = commonRepo.GetAvailableItems().First();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var user = userRepo.GetUserById(userId);
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = item };

            //Adding an intial slot for empty slot
            service.AddSlotIfInventoryIsEmpty(item, 30, user);

            // Now add same item
            service.AddItemToUserInventory(model);

            Assert.IsTrue(user.Inventory.Slots.Count > 1);
        }

        [TestMethod]
        public void Given_UserTriedToAddAnItem_WithAnExistingSlotWithDifferentItemType_ShouldCreateTheNewSlot_AndAddTheItem_ToSlot()
        {
            AvailableItem firstItem = commonRepo.GetAvailableItems().First();
            AvailableItem lastitem = commonRepo.GetAvailableItems().Last();
            Guid userId = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var user = userRepo.GetUserById(userId);
            var model = new InventoryViewModel() { Amount = 10, Guid = userId, AvailableItem = lastitem };

            //Adding an intial slot for empty slot
            service.AddSlotIfInventoryIsEmpty(firstItem, 30, user);

            // Now add same item
            service.AddItemToUserInventory(model);

            Assert.IsTrue(user.Inventory.Slots.Count > 1);
        }
    }
}
