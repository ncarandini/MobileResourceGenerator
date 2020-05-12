// Copyright 2018 Nicolò carandini
//
// Licensed under the ImageMagick License(the "License"); you may not use
// this file except in compliance with the License.  You may obtain a copy
// of the License at https://www.imagemagick.org/script/license.php
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the
// License for the specific language governing permissions and limitations
// under the License.

using ImageMagick;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TPCWare.MobileResourceGenerator
{
    class Program
    {
        static bool appIconGeneration;

        // To generate app icon:
        //
        // moregen --ai [<source_path>]
        //
        // Note:
        // - the command must contain --ai or --appicon parameter.
        // - the PNG file has to be at least 1024x1024 pixels.
        // - if <source_path> is not declared, the current directory is used and must contain a single PNG file.
        //
        // To generate image assets (one for each PNG file contained in the source directory):
        //
        // moregen [<source_path> [<target_path>]]
        //
        // Note:
        // if <source_path> is not declared, the current directory is used
        // if <target_path> is not declared, the directory <source_path> is used

        static void Main(string[] args)
        {
            appIconGeneration = CheckforAppIconGeneration(ref args);
            string sourceFileName = (args.Length > 0) ? Path.GetFileName(args[0]) : string.Empty;
            string sourceRootDir = (args.Length > 0) ? Path.GetFullPath(args[0]) : Path.GetFullPath("./");
            string targetRootDir = (args.Length > 1) ? Path.GetFullPath(args[1]) : sourceRootDir;

            Console.WriteLine($"Source dir: {sourceRootDir}");

            if (appIconGeneration)
            {
                if (string.IsNullOrWhiteSpace(sourceFileName))
                {
                    var filePaths = Directory.EnumerateFiles(sourceRootDir, "*.png");
                    if (filePaths.Count() == 1)
                    {
                        sourceFileName = Path.GetFileName(filePaths.First());
                    }
                }

                if (!string.IsNullOrWhiteSpace(sourceFileName))
                {
                    CreateIconFolderStructure(targetRootDir);
                    Console.WriteLine($"Generating App icon for image {sourceFileName} :");
                    GenerateAppIcon($"{sourceRootDir}/{sourceFileName}", targetRootDir);
                }
                else
                {
                    Console.WriteLine("Cannot generate App icon files: No source filename has been specified and the current directory contains zero or multiple PNG files.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(sourceFileName))
            {
                CreateAssetsFolderStructure(targetRootDir);
                Console.WriteLine($"Generating resources for image {sourceFileName} :");
                GenerateResources($"{sourceRootDir}/{sourceFileName}", targetRootDir);
            }
            else
            {
                var filePaths = Directory.EnumerateFiles(sourceRootDir, "*.png");

                if (filePaths.Count() == 0)
                {
                    Console.WriteLine("No PNG files in the source directory.");
                }
                else
                {
                    CreateAssetsFolderStructure(targetRootDir);
                    List<string> assets = new List<string>();
                    foreach (var filePath in filePaths)
                    {
                        Console.WriteLine($"Generating resource for image {filePath.Replace("\\", "/")} :");
                        GenerateResources(filePath, targetRootDir);
                        assets.Add(Path.GetFileNameWithoutExtension(filePath));
                    }
                    AddContentsJsonFile(assets);
                }
            }
        }

        private static bool CheckforAppIconGeneration(ref string[] args)
        {
            bool result = false;
            List<string> cmds = args.ToList();
            if (cmds.Any(cmd => cmd == "--appicon" || cmd == "--ai"))
            {
                result = true;
                cmds.RemoveAll(cmd => cmd == "--appicon");
                cmds.RemoveAll(cmd => cmd == "--ai");
                args = cmds.ToArray();
            }
            return result;
        }

        private static void CreateIconFolderStructure(string targetRootDir)
        {
            // Create the out folders structure if not already present
            Directory.CreateDirectory($"{targetRootDir}/iOS");
            Directory.CreateDirectory($"{targetRootDir}/iOS/AppIcon.appiconset");
            Directory.CreateDirectory($"{targetRootDir}/Android");
            Directory.CreateDirectory($"{targetRootDir}/Android/mipmap-mdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/mipmap-hdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/mipmap-xhdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/mipmap-xxhdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/mipmap-xxxhdpi");
        }

        private static void CreateAssetsFolderStructure(string targetRootDir)
        {
            // Create the out folders structure if not already present
            Directory.CreateDirectory($"{targetRootDir}/iOS");
            Directory.CreateDirectory($"{targetRootDir}/Android");
            Directory.CreateDirectory($"{targetRootDir}/Android/drawable");
            Directory.CreateDirectory($"{targetRootDir}/Android/drawable-hdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/drawable-xhdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/drawable-xxhdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/drawable-xxxhdpi");
        }

        private static void GenerateAppIcon(string sourceFilePath, string targetRootDir)
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("File not found");
            }
            else
            {
                // Create Apple icons 
                MakeNewAppleIcon(sourceFilePath, 16, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 20, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 29, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 32, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 40, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 48, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 55, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 58, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 60, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 64, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 76, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 80, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 87, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 88, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 100, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 120, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 128, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 152, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 167, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 172, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 180, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 196, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 216, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 256, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 512, targetRootDir);
                MakeNewAppleIcon(sourceFilePath, 1024, targetRootDir);

                // Add content.json file
                var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly);
                var fileInfo = manifestEmbeddedProvider.GetFileInfo("Data\\Contents.json");
                using Stream stream = fileInfo.CreateReadStream();
                using StreamReader sr = new StreamReader(stream);
                var contentsJson = sr.ReadToEnd();
                File.WriteAllText($"{targetRootDir}iOS\\AppIcon.appiconset\\Contents.json", contentsJson);

                // Add Android Icons
                MakeNewAndroidIcon(sourceFilePath, 48, $"{targetRootDir}Android\\mipmap-mdpi\\");
                MakeNewAndroidIcon(sourceFilePath, 72, $"{targetRootDir}Android\\mipmap-hdpi\\");
                MakeNewAndroidIcon(sourceFilePath, 96, $"{targetRootDir}Android\\mipmap-xhdpi\\");
                MakeNewAndroidIcon(sourceFilePath, 144, $"{targetRootDir}Android\\mipmap-xxhdpi\\");
                MakeNewAndroidIcon(sourceFilePath, 192, $"{targetRootDir}Android\\mipmap-xxxhdpi\\");
            }
        }

        private static void MakeNewAppleIcon(string sourceFilePath, int iconWidth, string targetRootDir)
        {
            using (MagickImage sourceImage = new MagickImage(sourceFilePath))
            {
                sourceImage.Resize(iconWidth, iconWidth);
                string targetFilepath = $"{targetRootDir}iOS\\AppIcon.appiconset\\Icon{iconWidth}.png";
                sourceImage.Write(targetFilepath);
                Console.WriteLine($"'-- {targetFilepath.Replace("\\", "/")}");
            }
        }

        private static void MakeNewAndroidIcon(string sourceFilePath, int iconWidth, string targetRootDir)
        {
            using (MagickImage sourceImage = new MagickImage(sourceFilePath))
            {
                sourceImage.Resize(iconWidth, iconWidth);
                string targetFilepath = $"{targetRootDir}icon.png";
                sourceImage.Write(targetFilepath);
                Console.WriteLine($"'-- {targetFilepath.Replace("\\", "/")}");
            }
        }

        private static void GenerateResources(string sourceFilePath, string targetRootDir)
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("File not found");
            }
            else
            {
                // Create iOS artifacts
                string sourceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
                string sourceFileNameExtension = Path.GetExtension(sourceFilePath);
                string imageSetDir = $"{targetRootDir}iOS\\{sourceFileNameWithoutExtension}.imageset\\";
                Directory.CreateDirectory(imageSetDir);
                //Path.GetFileNameWithoutExtension(sourceFileName)
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Pixel, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Pixel2, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Pixel3, targetRootDir);
                string contentsJson = $"{{\"images\":[{{\"filename\":\"{sourceFileNameWithoutExtension}{sourceFileNameExtension}\",";
                contentsJson += $"\"scale\":\"1x\",\"idiom\":\"universal\"}},";
                contentsJson += $"{{\"filename\":\"{sourceFileNameWithoutExtension}@2x{sourceFileNameExtension}\",";
                contentsJson += $"\"scale\":\"2x\",\"idiom\":\"universal\"}},";
                contentsJson += $"{{\"filename\":\"{sourceFileNameWithoutExtension}@3x{sourceFileNameExtension}\",";
                contentsJson += $"\"scale\":\"3x\",\"idiom\":\"universal\"}}],";
                contentsJson += $"\"info\":{{\"version\":1,\"author\":\"xcode\"}}}}";
                File.WriteAllText($"{imageSetDir}Contents.json", contentsJson);

                // Create Android artifacts
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Mdpi, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Hdpi, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Xhdpi, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Xxhdpi, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Xxxhdpi, targetRootDir);
            }
        }

        private static void MakeNewImage(string sourceFilePath, Resolution fromResolution, Resolution toResolution, string targetRootDir)
        {
            using (MagickImage sourceImage = new MagickImage(sourceFilePath))
            {
                MagickImageInfo sourceInfo = new MagickImageInfo(sourceFilePath);

                int width = PixelConverter(sourceInfo.Width, fromResolution, toResolution);
                int height = PixelConverter(sourceInfo.Height, fromResolution, toResolution);

                sourceImage.Resize(width, height);

                string sourceFileName = Path.GetFileName(sourceFilePath);
                string sourceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFileName);
                string sourceFileNameExtension = Path.GetExtension(sourceFileName);

                string targetFilepath;
                switch (toResolution)
                {
                    case Resolution.Pixel:
                        targetFilepath = $"{targetRootDir}iOS\\{sourceFileNameWithoutExtension}.imageset\\{sourceFileName}";
                        break;
                    case Resolution.Pixel2:
                        targetFilepath = $"{targetRootDir}iOS\\{sourceFileNameWithoutExtension}.imageset\\{sourceFileNameWithoutExtension}@2x{sourceFileNameExtension}";
                        break;
                    case Resolution.Pixel3:
                        targetFilepath = $"{targetRootDir}iOS\\{sourceFileNameWithoutExtension}.imageset\\{sourceFileNameWithoutExtension}@3x{sourceFileNameExtension}";
                        break;
                    case Resolution.Mdpi:
                        targetFilepath = $"{targetRootDir}Android/drawable/{sourceFileName}";
                        break;
                    case Resolution.Hdpi:
                        targetFilepath = $"{targetRootDir}Android/drawable-hdpi/{sourceFileName}";
                        break;
                    case Resolution.Xhdpi:
                        targetFilepath = $"{targetRootDir}Android/drawable-xhdpi/{sourceFileName}";
                        break;
                    case Resolution.Xxhdpi:
                        targetFilepath = $"{targetRootDir}Android/drawable-xxhdpi/{sourceFileName}";
                        break;
                    case Resolution.Xxxhdpi:
                        targetFilepath = $"{targetRootDir}Android/drawable-xxxhdpi/{sourceFileName}";
                        break;
                    default:
                        throw new ArgumentException();
                }

                sourceImage.Write(targetFilepath);

                Console.WriteLine($"--> {targetFilepath.Replace("\\", "/")}");
            }
        }

        private static void AddContentsJsonFile(List<string> assets)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var asset in assets)
            {
                // TODO
            }

            // TODO: save the file
        }

        private static int PixelConverter(int fromValue, Resolution fromResolution, Resolution toResolution)
        {
            double toValue;
            double androidDpiValue;
            switch (fromResolution)
            {
                case Resolution.Pixel:
                    androidDpiValue = fromValue * (160.0 / 163.0);
                    break;
                case Resolution.Pixel2:
                    androidDpiValue = fromValue / 2.0 * (160.0 / 163.0);
                    break;
                case Resolution.Pixel3:
                    androidDpiValue = fromValue / 3.0 * (160.0 / 163.0);
                    break;
                case Resolution.Mdpi:
                    androidDpiValue = fromValue;
                    break;
                case Resolution.Hdpi:
                    androidDpiValue = fromValue / 1.5;
                    break;
                case Resolution.Xhdpi:
                    androidDpiValue = fromValue / 2.0;
                    break;
                case Resolution.Xxhdpi:
                    androidDpiValue = fromValue / 3.0;
                    break;
                case Resolution.Xxxhdpi:
                    androidDpiValue = fromValue / 4.0;
                    break;
                default:
                    throw new ArgumentException();
            }

            switch (toResolution)
            {
                case Resolution.Pixel:
                    toValue = androidDpiValue * (163.0 / 160.0);
                    break;
                case Resolution.Pixel2:
                    toValue = androidDpiValue * (163.0 / 160.0) * 2.0;
                    break;
                case Resolution.Pixel3:
                    toValue = androidDpiValue * (163.0 / 160.0) * 3.0;
                    break;
                case Resolution.Mdpi:
                    toValue = androidDpiValue;
                    break;
                case Resolution.Hdpi:
                    toValue = androidDpiValue * 1.5;
                    break;
                case Resolution.Xhdpi:
                    toValue = androidDpiValue * 2.0;
                    break;
                case Resolution.Xxhdpi:
                    toValue = androidDpiValue * 3.0;
                    break;
                case Resolution.Xxxhdpi:
                    toValue = androidDpiValue * 4.0;
                    break;
                default:
                    throw new ArgumentException();
            }

            return Convert.ToInt32(Math.Round(toValue));
        }
    }
}
