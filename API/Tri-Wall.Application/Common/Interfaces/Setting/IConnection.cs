

using SAPbobsCOM;

namespace Tri_Wall.Application.Common.Interfaces.Setting;

public interface IConnection
{
    Company Connect();
    void Disconnect();
}
