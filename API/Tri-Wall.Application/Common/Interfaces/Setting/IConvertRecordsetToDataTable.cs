

using SAPbobsCOM;
using System.Data;

namespace Tri_Wall.Application.Common.Interfaces.Setting;

public interface IConvertRecordsetToDataTable
{
    DataTable ToDataTable(Recordset recordset);
}
