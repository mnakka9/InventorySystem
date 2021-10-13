using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS.Data.Models;

namespace IS.Data.Services.UserRepo
{
    public interface IUserRepo
    {
        User GetUserById(Guid id);

        IList<User> GetUsers();
    }
}
