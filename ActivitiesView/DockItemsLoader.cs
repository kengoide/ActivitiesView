using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivitiesView {
    class DockItemsLoader {
        public static IEnumerable<DockItem> Load() {
            StringBuilder buffer = new StringBuilder(Win32.MAX_PATH);
            IEnumerable<string> e;
            try {
                e = Directory.EnumerateFiles("DockShortcuts");
            }
            catch (DirectoryNotFoundException) {
                yield break;
            }
            foreach (string fileName in e.Where(name => name.EndsWith(".lnk"))) {
                string linkFilePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                Win32.IShellLinkW shellLink = (Win32.IShellLinkW)new Win32.ShellLink();
                Win32.IPersistFile persistFile = (Win32.IPersistFile)shellLink;
                persistFile.Load(linkFilePath, Win32.STGM_READ);
                shellLink.Resolve(IntPtr.Zero, Win32.SLR_NO_UI);
                shellLink.GetPath(buffer, buffer.Capacity, IntPtr.Zero, 0);
                string path = buffer.ToString();
                shellLink.GetArguments(buffer, buffer.Capacity);
                string arguments = buffer.ToString();
                shellLink.GetWorkingDirectory(buffer, buffer.Capacity);
                string workingDirectory = buffer.ToString();
                shellLink.GetDescription(buffer, buffer.Capacity);
                string description = buffer.ToString();
                int showCommand = shellLink.GetShowCmd();
                string displayName = Path.GetFileNameWithoutExtension(linkFilePath);
                yield return new DockItem(path, arguments, workingDirectory, displayName, description, showCommand);
            }
        }
    }
}
