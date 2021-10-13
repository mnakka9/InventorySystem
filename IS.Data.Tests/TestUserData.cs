using IS.Data.Models;
using IS.Data.Services.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Data.Tests
{
    internal class TestUserData : IUserRepo
    {
        public TestUserData()
        {
            List<User> users = new List<User>()
            {
                new User() { Id = Guid.NewGuid(), Name = "Mahesh"},
                new User() { Id = Guid.NewGuid(), Name = "Mayank"}
            };

            Users = users;
        }

        public List<User> Users {  get; set; }

        public bool AddUser(User user)
        {
            if(user == null)
            {
                return false;
            }

            Users.Add(user);

            return true;
        }

        public bool DeleteUser(Guid id)
        {
            User user = GetUserById(id);

            if(user == null) return false;

            Users.Remove(user);
            return true;
        }

        public User GetUserById(Guid id)
        {
            return Users.Find( u => u.Id == id);
        }

        public IList<User> GetUsers()
        {
            return Users;
        }
    }
}
