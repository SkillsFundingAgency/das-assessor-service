using System.IO;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Utilities
{
    public class FileUtilities
    {
        public void MoveDirectory(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            // Get Files & Copy
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);

                // ADD Unique File Name Check to Below!!!!
                string dest = Path.Combine(destFolder, name);
                File.Move(file, dest);
            }

            // Get dirs recursively and copy files
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                if (folder != destFolder)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    MoveDirectory(folder, dest);
                }
            }
        }
    }
}
