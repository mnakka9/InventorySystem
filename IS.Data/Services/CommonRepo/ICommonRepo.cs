using IS.Data.Models;
using System.Collections.Generic;

namespace IS.Data.Services.CommonRepo
{
    public interface ICommonRepo
    {
        List<ItemType> GetItemTypes();

        int MaximumAllowedSlotsForType(int typeId);

        List<AvailableItem> GetAvailableItems();
    }
}
