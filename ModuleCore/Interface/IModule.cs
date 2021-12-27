namespace OPSService.ModuleCore.Interfaces
{
    public interface IModule
    {
        ISettings ModuleSetings { get; }
        //IModuleState ModuleState { get; }
        void StartProcess();
        void StopProcess();
        bool IsRunning();
        void CompletePendingTask();
        void Run();
        void MoveFileInErrorFolder(string fileName, bool isFullFilePath = true);
        void MoveFileInErrorFolder2(string fileName, bool isFullFilePath = true);
        void MoveFileInReportOutputPath(string fileName, bool isFullFilePath = true);
        void MoveFileInOutputFolder(string fileName, bool isFullFilePath = true);

    }
}
