using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Shared.Models;

public record Vendors(
    string VendorCode,
    string VendorName,
    string PhoneNumber,
    string ContactID
    );
