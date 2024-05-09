

using SAPbobsCOM;

namespace Tri_Wall.Application.Common.Interfaces;

public interface IConnection
{
    Company Connect();
    void Disconnect();
}
