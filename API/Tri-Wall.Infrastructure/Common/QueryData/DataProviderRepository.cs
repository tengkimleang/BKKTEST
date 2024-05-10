
using SAPbobsCOM;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.Common.Interfaces.Setting;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.Infrastructure.Common.QueryData;

public class DataProviderRepository : IDataProviderRepository
{
    private readonly IConnection IConnection;
    private readonly IConvertRecordsetToDataTable convertRecordsetToDataTable;
    public DataProviderRepository(IConnection iConnection, IConvertRecordsetToDataTable convertRecordsetToDataTable)
    {
        IConnection = iConnection;
        this.convertRecordsetToDataTable = convertRecordsetToDataTable;
    }
    public Task<DataTable> Query(DataProvider dataProviderRequest)
    {
        var connection = IConnection.Connect();
        Recordset recordset = (Recordset)connection.GetBusinessObject(BoObjectTypes.BoRecordset);
        string query = connection.DbServerType == BoDataServerTypes.dst_HANADB
            ? $"CALL \"{connection.CompanyDB}\".\"{dataProviderRequest.StoreName}\" ('{dataProviderRequest.DBType}','{dataProviderRequest.Par1}','{dataProviderRequest.Par2}','{dataProviderRequest.Par3}','{dataProviderRequest.Par4}','{dataProviderRequest.Par5}')"
            : $"EXEC \"{connection.CompanyDB}\".\"{dataProviderRequest.StoreName}\" ('{dataProviderRequest.DBType} ',' {dataProviderRequest.Par1} ',' {dataProviderRequest.Par2} ',' {dataProviderRequest.Par3} ',' {dataProviderRequest.Par4} ',' {dataProviderRequest.Par5}')";

        recordset.DoQuery(query);

        return Task.FromResult(convertRecordsetToDataTable.ToDataTable(recordset));
    }
}
