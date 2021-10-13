using IS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Data.Services.UserRepo
{
    public class UserRepo : IUserRepo
    {
        private List<User> UsersList = new(){
                new User() { Id = new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"), Name = "Mahesh", Inventory = new Models.Inventory() },
                new User() { Id = Guid.NewGuid(), Name = "Mayank", Inventory = new Inventory() }
            };

        public User GetUserById(Guid id)
        {
            return UsersList.Find(i => i.Id == id);
        }

        public IList<User> GetUsers()
        {
            return UsersList;
        }
    }
}
