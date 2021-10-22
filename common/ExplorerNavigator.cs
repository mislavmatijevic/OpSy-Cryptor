using System.Diagnostics;

namespace OpSy_Cryptor.common
{
    public static class ExplorerNavigator
    {
        public static void NavigateWindowsExplorerTo(string fullPath)
        {
            Process ExplorerWindowProcess = new();
            ExplorerWindowProcess.StartInfo.FileName = "explorer.exe";
            ExplorerWindowProcess.StartInfo.Arguments = $"/select,\"{fullPath}\"";
            ExplorerWindowProcess.Start();
        }
    }
}
