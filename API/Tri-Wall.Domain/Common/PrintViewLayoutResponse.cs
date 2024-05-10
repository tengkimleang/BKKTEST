using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Domain.Common;

public record PrintViewLayoutResponse(
    string ErrorMessage="",
    string ErrCode = "",
    byte[] ?Data=null,
    string ApplicationType = "",
    string FileName = ""
    );
