﻿

namespace Tri_Wall.Shared.Models.Gets;

public record GetBatchOrSerial(
        string ItemCode,
        string Qty,
        string SerialBatch,
        string MfrSerialNo,
        string ExpDate,
        string MrfDate,
        string Type,
        string LineNum,
        string InputQty=""
    );
