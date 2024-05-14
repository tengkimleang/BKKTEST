using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tri_Wall.Shared.Models;

namespace Tri_Wall.Shared.Services;

public interface ILoadMasterData
{
    Task LoadItemMaster();
    Task LoadVendorMaster();
}
