using IS.Data.Models;
using IS.Data.Services.UserRepo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IS.Data.Tests
{
    [TestClass]
    public class UserRepoTests
    {
        private Mock<IUserRepo> mockUserRepo;
        private IUserRepo data = new TestUserData();

        [TestInitialize]
        public void Init()
        {
            mockUserRepo = new Mock<IUserRepo>();
            mockUserRepo.Setup(m => m.GetUsers()).Returns(data.GetUsers());
        }

        [TestMethod]
        public void Given_usersareavailable_whencalledgetusers_shouldreturs_alluserslist()
        {
            var users = mockUserRepo.Object.GetUsers();

            Assert.AreEqual(2, users.Count);

            Assert.AreEqual<string>("Mahesh", users[0].Name);
        }

        [TestMethod]
        public void Given_userisavailabe_whenmethodgetusercalled_shouldreturn_theuse()
        {
            var repo = mockUserRepo.Object;
            var users = repo.GetUsers();
            var id = users[0].Id;

            User actual = data.GetUserById(id);

            Assert.AreEqual(id, actual.Id);
            Assert.AreEqual(users[0].Name, actual.Name);
        }

        [TestMethod]
        public void Given_userisnotavailabe_whenmethodgetusercalled_shouldreturn_null()
        {
             User actual = data.GetUserById(Guid.Empty);

            Assert.IsNull(actual);
        }
    }
}
