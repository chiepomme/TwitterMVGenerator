#if UNITY_STANDALONE_WIN

using System;
using System.Runtime.InteropServices;

namespace SFB
{


    public class StandaloneFileBrowserWindows : IStandaloneFileBrowser
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            return null;
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            return null;
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            return null;
        }

        // .NET Framework FileDialog Filter format
        // https://msdn.microsoft.com/en-us/library/microsoft.win32.filedialog.filter
        private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            return null;
        }

        private static string GetDirectoryPath(string directory)
        {
            return null;
        }
    }
}

#endif