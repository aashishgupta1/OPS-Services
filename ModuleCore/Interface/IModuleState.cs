using ModuleCore;
using System;

namespace OPSService.ModuleCore.Interfaces
{
    public interface IModuleState
    {
        string ConnectionString { get; set; }

        bool Add(string moduleName, int moduleId, string moduleState = "");

        void Update(string moduleState);

        void Update(DateTime endDate);

        string GetLastInfo();
        StateInfo[] GetAllInfo();
        void DeleteRows(int numberOfDays);
        void DeleteRow(int id);
    }
}
