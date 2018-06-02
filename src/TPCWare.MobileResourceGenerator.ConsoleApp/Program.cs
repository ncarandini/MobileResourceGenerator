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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TPCWare.MobileResourcesGenerator.ConsoleApp
{
    class Program
    {
        static bool appIconGeneration;

        static void Main(string[] args)
        {
            appIconGeneration = CheckforAppIconGeneration(ref args);
            string sourceFileName = (args.Length > 0) ? Path.GetFileName(args[0]) : string.Empty;
            string sourceRootDir = (args.Length > 1) ? Path.GetFullPath(args[1]) : Path.GetFullPath("./");
            string targetRootDir = (args.Length > 2) ? Path.GetFullPath(args[2]) : sourceRootDir;

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
                    CreatefolderStructure(targetRootDir);
                    Directory.CreateDirectory($"{targetRootDir}/iOS/AppIcon");
                    Console.WriteLine($"Generating App icon for image {sourceFileName} :");
                    GenerateAppIcon($"{sourceRootDir}/{sourceFileName}", targetRootDir);
                }
                else
                {
                    Console.WriteLine("Cannot generate App icon: please specify source filename.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(sourceFileName))
            {
                CreatefolderStructure(targetRootDir);
                Console.WriteLine($"Generating resource for image {sourceFileName} :");
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
                    CreatefolderStructure(targetRootDir);
                    foreach (var filePath in filePaths)
                    {
                        Console.WriteLine($"Generating resource for image {filePath.Replace("\\", "/")} :");
                        GenerateResources(filePath, targetRootDir);
                    }
                }
            }

            Console.Write("Hit a key to terminate...");
            Console.ReadKey();
        }

        private static bool CheckforAppIconGeneration(ref string[] args)
        {
            bool result = false;
            List<string> cmds = args.ToList();
            if (cmds.Any(cmd => cmd == "-ai"))
            {
                result = true;
                cmds.RemoveAll(cmd => cmd == "-ai");
                args = cmds.ToArray();
            }
            return result;
        }

        private static void CreatefolderStructure(string targetRootDir)
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

        private static void GenerateResources(string sourceFilePath, string targetRootDir)
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("File not found");
            }
            else
            {
                // Create iOS artifacts
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Pixel, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Pixel2, targetRootDir);
                MakeNewImage(sourceFilePath, Resolution.Xxxhdpi, Resolution.Pixel3, targetRootDir);

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
                        targetFilepath = $"{targetRootDir}iOS/{sourceFileName}";
                        break;
                    case Resolution.Pixel2:
                        targetFilepath = $"{targetRootDir}iOS/{sourceFileNameWithoutExtension}@2x{sourceFileNameExtension}";
                        break;
                    case Resolution.Pixel3:
                        targetFilepath = $"{targetRootDir}iOS/{sourceFileNameWithoutExtension}@3x{sourceFileNameExtension}";
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

                Console.WriteLine($"'-- {targetFilepath.Replace("\\", "/")}");
            }
        }

        private static void GenerateAppIcon(string sourceFilePath, string targetRootDir)
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("File not found");
            }
            else
            {
                // Create iOS app icons
                MakeNewImage(sourceFilePath, 20, targetRootDir);
                MakeNewImage(sourceFilePath, 29, targetRootDir);
                MakeNewImage(sourceFilePath, 40, targetRootDir);
                MakeNewImage(sourceFilePath, 58, targetRootDir);
                MakeNewImage(sourceFilePath, 60, targetRootDir);
                MakeNewImage(sourceFilePath, 76, targetRootDir);
                MakeNewImage(sourceFilePath, 80, targetRootDir);
                MakeNewImage(sourceFilePath, 87, targetRootDir);
                MakeNewImage(sourceFilePath, 120, targetRootDir);
                MakeNewImage(sourceFilePath, 152, targetRootDir);
                MakeNewImage(sourceFilePath, 167, targetRootDir);
                MakeNewImage(sourceFilePath, 180, targetRootDir);
            }
        }

        private static void MakeNewImage(string sourceFilePath, int iconWidth, string targetRootDir)
        {
            using (MagickImage sourceImage = new MagickImage(sourceFilePath))
            {
                MagickImageInfo sourceInfo = new MagickImageInfo(sourceFilePath);

                sourceImage.Resize(iconWidth, iconWidth);

                string sourceFileName = Path.GetFileName(sourceFilePath);
                string sourceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFileName);
                string sourceFileNameExtension = Path.GetExtension(sourceFileName);

                string targetFilepath = $"{targetRootDir}iOS/AppIcon/Icon{iconWidth.ToString()}.png";
                sourceImage.Write(targetFilepath);
                Console.WriteLine($"'-- {targetFilepath.Replace("\\", "/")}");
            }
        }

        private static int PixelConverter(int fromValue, Resolution fromResolution, Resolution toResolution)
        {
            double toValue = 0;
            double androidDpiValue = 0;
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

    enum Resolution
    {
        Pixel,
        Pixel2,
        Pixel3,
        Mdpi,
        Hdpi,
        Xhdpi,
        Xxhdpi,
        Xxxhdpi
    }
}

