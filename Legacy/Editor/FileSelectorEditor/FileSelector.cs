using System;
using System.IO;
using History.Tools.Editor;

namespace History.Editor.FileSelectorEditor
{
    public static class FileSelector
    {
        private static FileSelectorEditorWindow _selectorWindow;
        
        public static void Open(string folder, FileInfo selectedFile, string[] extensions, Action<FileInfo> fileSelectAction)
        {
            var files = FileSystemUtilsEditor.GetFilesInFolderAll(folder, extensions);
            
            
            _selectorWindow = FileSelectorEditorWindow.Open();
            _selectorWindow.Setup(files, selectedFile, fileSelectAction);
        }
        
    }
}