using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Shared.Models.Gets
{
    public record GetRequest(
        string StoreName,
        string DBType,
        string Par1 = "",
        string Par2 = "",
        string Par3 = "",
        string Par4 = "",
        string Par5 = "");
}
