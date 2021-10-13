using IS.Business.ViewModels;
using IS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Business.Services
{
    public interface IInventoryRemoveService
    {
        void RemoveItemFromUserInventory(InventoryViewModel viewModel)
        {
            if(!this.IsItemAvailable(viewModel.AvailableItem, new User()))
            {
                throw new Exception($"There is no {viewModel.AvailableItem.Name} to remove");
            }
        }

        bool IsItemAvailable(AvailableItem item, User user);
    }
}
