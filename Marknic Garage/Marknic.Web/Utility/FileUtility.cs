using System;
using System.IO;

namespace Marknic.Web.Utility
{
    /// <summary>
    /// Static class that will automatically adapt to hardware device or emulator upon startup
    /// </summary>
    public static class FileUtility
    {
        private const string FileStoreSd = @"\SD\";
        private const string FileStoreWinfs = @"\WINFS\";

        static FileUtility()
        {
            FileStore = FileStoreSd;

            if (Directory.Exists(FileStore)) return;

            if (!Directory.Exists(FileStoreWinfs))
            {
                throw new ApplicationException("WebServer: File storage not found!");
            }

            FileStore = FileStoreWinfs;
        }

        public static string FileStore { get; private set; }
    }
}
