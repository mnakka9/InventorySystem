using IS.Data.Services.CommonRepo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Data.Tests
{
    [TestClass]
    public class CommonRepoTest
    {
        private ICommonRepo commonRepo = new FakeCommonData();

        [TestMethod]
        public void Given_DataAvaialable_GetAllAvailableItems_ShouldReturn_ListOfAvailableItems()
        {
            var result = commonRepo.GetAvailableItems();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 1);
        }

        [TestMethod]
        public void Given_DataAvaialable_GetItemTypes_ShouldReturn_ListofItemTypes()
        {
            var result = commonRepo.GetItemTypes();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 1);
        }

        [TestMethod]
        public void Given_DataAvaialable_MaximumAllowedSlotsForType_ShouldReturn_MaximumItemsThatCanbeAdded_ForType()
        {
            var itemTypes = commonRepo.GetItemTypes();

            var result = commonRepo.MaximumAllowedSlotsForType(itemTypes[0].Id);

            Assert.IsTrue(result > 1);
            Assert.AreEqual(itemTypes[0].AllowedSlots, result);
        }

        [TestMethod]
        public void Given_DataNotAvaialable_MaximumAllowedSlotsForType_ShouldReturn_Zero()
        {
            var result = commonRepo.MaximumAllowedSlotsForType(0);

            Assert.IsTrue(result == 0);
        }
    }
}
