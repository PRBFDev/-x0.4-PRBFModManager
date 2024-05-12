using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PRBFModManager
{
    public static class FileTools
    {
        public static void ChangeExtension(string path , string newFormat)
        {
            if (Path.GetExtension(path) != newFormat)
            {
                string newFilePath = Path.ChangeExtension(path, newFormat);
                File.Move(path, newFilePath);
               path = newFilePath; 
            }
        }
    }
}
