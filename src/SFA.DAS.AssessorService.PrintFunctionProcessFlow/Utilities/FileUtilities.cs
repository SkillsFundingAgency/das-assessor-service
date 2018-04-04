using System;
using System.IO;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Utilities
{
    public class FileUtilities
    {
        public void MoveDirectory(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
           
            var files = Directory.GetFiles(sourceFolder);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
             
                var dest = Path.Combine(destFolder, name);
                File.Move(file, dest);
            }
        
            var folders = Directory.GetDirectories(sourceFolder);
            foreach (var folder in folders)
            {
                if (folder == destFolder)
                    continue;

                var name = Path.GetFileName(folder);
                var dest = Path.Combine(destFolder, name ?? throw new InvalidOperationException());

                MoveDirectory(folder, dest);
            }
        }
    }
}
