// Copyright 2020 Nicolò Carandini

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TPCWare.ResourceIncludeGenerator
{
    class Program
    {
        static List<string> resourceSubPaths = new List<string>();

        static string sourceRootDir;

        // To generate resource include list (to be copied in the Xamarin Android .csproj file):
        // resinc [<project_path> [<target_path>]]
        static void Main(string[] args)
        {
            sourceRootDir = (args.Length == 1) ? Path.GetFullPath(args[0]) : Path.GetFullPath("./");
            string targetRootDir = (args.Length == 2) ? Path.GetFullPath(args[1]) : sourceRootDir;

            string resourcesDir = Path.Combine(sourceRootDir, "Resources");
            if (!Directory.Exists(resourcesDir))
            {
                Console.WriteLine($"Resource directory not found on project: {sourceRootDir}");
            }
            else
            {
                Console.WriteLine($"Source dir: {resourcesDir}");
                ProcessDirectory(resourcesDir);

                StringBuilder fileContent = new StringBuilder("<ItemGroup>\n");
                foreach (var subPath in resourceSubPaths.OrderBy(x => x))
                {
                    fileContent.Append($"<AndroidResource Include=\"{subPath}\" />\n");
                }
                fileContent.Append("</ItemGroup>");

                File.WriteAllText($"{targetRootDir}/ResourcesIncludeList.txt", fileContent.ToString());
            }
        }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            foreach(string fileName in Directory.GetFiles(targetDirectory))
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach(string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            Console.WriteLine("Processed file '{0}'.", path);
            string subPath = path.Replace(sourceRootDir, "").Trim();
            if (!string.IsNullOrWhiteSpace(subPath) && (subPath.StartsWith("/") || subPath.StartsWith("\\")))
            {
                subPath = subPath.Substring(1);
            }
            subPath = subPath.Replace("/", "\\");
            resourceSubPaths.Add(subPath);
        }

    }  
}