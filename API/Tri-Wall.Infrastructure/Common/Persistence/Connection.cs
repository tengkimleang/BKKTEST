using Microsoft.Extensions.Options;
using SAPbobsCOM;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.Common.Interfaces.Setting;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Infrastructure.Common.Persistence;

public class Connection : IConnection, IUnitOfWork
{
    private readonly ConnectionSettings _settings;
    public static Company _company = new();
    public Connection(IOptions<ConnectionSettings> settings)
    {
        _settings = settings.Value;
        Connect();
    }

    public void BeginTransaction()
    {
        if (_company.InTransaction)
        {
            _company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
        }
        _company.StartTransaction();
    }

    public void Commit()
    {
        _company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
    }

    public Company Connect()
    {
        //cheecking coonection
        if (_company.Connected) return _company;
        //set value from setting to _company
        _company = new Company
        {
            Server = _settings.Server,
            CompanyDB = _settings.CompanyDB,
            UserName = _settings.UserNameSAP,
            Password = _settings.Password,
            DbUserName = _settings.DbUserName,
            DbPassword = _settings.DbPassword,
            UseTrusted = _settings.UseTrusted,
            // DbServerType 7 = dst_MSSQL2012,8 = dst_MSSQL2014,9 = dst_HANADB
            DbServerType = (BoDataServerTypes)_settings.DbServerType,
            SLDServer = _settings.SLDServer,
            LicenseServer = _settings.LicenseServer
        };
        _company.Connect();
        return _company;
    }

    public void Disconnect()
    {
        //cheecking coonection
        if (!_company.Connected) return;
        _company.Disconnect();
    }

    public void Rollback()
    {
        if (_company.InTransaction)
        {
            _company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
        }
    }
}
