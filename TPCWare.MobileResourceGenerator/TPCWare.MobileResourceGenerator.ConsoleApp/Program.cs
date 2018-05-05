using ImageMagick;
using System;
using System.IO;
using System.Linq;

namespace TPCWare.MobileResourcesGenerator.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFileName = (args.Length > 0) ? Path.GetFileName(args[0]) : string.Empty;
            string sourceRootDir = (args.Length > 1) ? Path.GetFullPath(args[1]) : Path.GetFullPath("./");
            string targetRootDir = (args.Length > 2) ? Path.GetFullPath(args[2]) : sourceRootDir;

            // Create the out folders structure if not already present
            Directory.CreateDirectory($"{targetRootDir}/iOS");
            Directory.CreateDirectory($"{targetRootDir}/iOS/Resources");
            Directory.CreateDirectory($"{targetRootDir}/Android");
            Directory.CreateDirectory($"{targetRootDir}/Android/Resources");
            Directory.CreateDirectory($"{targetRootDir}/Android/Resources/drawable");
            Directory.CreateDirectory($"{targetRootDir}/Android/Resources/drawable-hdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/Resources/drawable-xhdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/Resources/drawable-xxhdpi");
            Directory.CreateDirectory($"{targetRootDir}/Android/Resources/drawable-xxxhdpi");

            Console.WriteLine($"Source dir: {sourceRootDir}");

            if (!string.IsNullOrWhiteSpace(sourceFileName))
            {
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
                        targetFilepath = $"{targetRootDir}/iOS/Resources/{sourceFileName}";
                        break;
                    case Resolution.Pixel2:
                        targetFilepath = $"{targetRootDir}/iOS/Resources/{sourceFileNameWithoutExtension}@2x{sourceFileNameExtension}";
                        break;
                    case Resolution.Pixel3:
                        targetFilepath = $"{targetRootDir}/iOS/Resources/{sourceFileNameWithoutExtension}@3x{sourceFileNameExtension}";
                        break;
                    case Resolution.Mdpi:
                        targetFilepath = $"{targetRootDir}/Android/Resources/drawable/{sourceFileName}";
                        break;
                    case Resolution.Hdpi:
                        targetFilepath = $"{targetRootDir}/Android/Resources/drawable-hdpi/{sourceFileName}";
                        break;
                    case Resolution.Xhdpi:
                        targetFilepath = $"{targetRootDir}/Android/Resources/drawable-xhdpi/{sourceFileName}";
                        break;
                    case Resolution.Xxhdpi:
                        targetFilepath = $"{targetRootDir}/Android/Resources/drawable-xxhdpi/{sourceFileName}";
                        break;
                    case Resolution.Xxxhdpi:
                        targetFilepath = $"{targetRootDir}/Android/Resources/drawable-xxxhdpi/{sourceFileName}";
                        break;
                    default:
                        throw new ArgumentException();
                }

                sourceImage.Write(targetFilepath);

                Console.WriteLine($"'-- {targetFilepath}");
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

