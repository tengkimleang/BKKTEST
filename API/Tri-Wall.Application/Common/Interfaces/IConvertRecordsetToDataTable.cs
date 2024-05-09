

using SAPbobsCOM;
using System.Data;

namespace Tri_Wall.Application.Common.Interfaces;

public interface IConvertRecordsetToDataTable
{
    DataTable ToDataTable(Recordset recordset);
}
