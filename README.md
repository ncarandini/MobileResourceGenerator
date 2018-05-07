# MobileResourceGenerator
A command line image resizer that produce Android and iOS bitmap artifacts.

## Preamble

We all know that the first thing to do when designing UI layouts is to use Device Independent Pixels, called DIP or more often DP, to take in account the different pixel density, dimensions and form factors of device screens.

So the best way to operate when creating artifacts is to use vector images as much as we can.

But when creating the app, with Xamarin or Android Studio or Apple XCode or whatever else, at the end of the day we need bitmap artifacts (tipically PNG files) on many different dimensions. So for each "source" artifact, we need a lot of resized artifact to put on Android drawable resources and iOS image collections.

To create those artifacts, I use an Excel file that I've made to calculate the pixel sizes starting from iOS DPs and for your convenience I've put a copy of it in the "DP Calulator" folder of this project.

To ease the production of resized ertifacts, I've developed a simple console app that starting from a XXXHPDI image (the biggest one) produces all the other images, for iOS (image, image@2x, image@3x) and Android (on filders "drawable", "drawable-hdpi", ..., "drawable-xxxhdpi").

## Install
1. Create a folder in your PC (i.e. c:\Moregen) and copy there the two files from the "Console App" folder of this project:
- Magick.NET-Q16-AnyCPU.dll
- MoReGen.exe
2. Add the path (i.e. c:\Moregen) to the PATH environment variable

## Usage
The command is:
MOREGEN \[imagepath \[ sourcefolder \[targetfolder]]]

where:
- if none of the optional parameters are set, sourcefolder and targetfolder are set to the current directory and the production of artifacts will be done for each PNG file of the current directory. If none exist, nothing is done.
- if the imagepath contains just the name of PNG file (i.e. "myimage.png") and the file exist in the current directory, the production of artifacts will be done exclusively for that file.
- if the imagepath contains a full path of an existing  PNG file (i.e. "c:/mydir/myimage.png"), the sourcefolder and target folder will be set to the same folder of the imagepath and the production of artifacts will be done exclusively for that file.
- if the imagepath contains a full path of an existing  PNG file (i.e. "c:/mydir/myimage.png"), but a sourcefolder parameter is specified on the command, the imagepath will contain only the filename and the sourcefolder will be used as a source and target folder path.

On the target folder, if non already present, the command will create the folder structure needed to contain the generated artifacts.

## Example
As an example, lets suppose you have two images on a test folder (i.e. Desktop/Test):

![image](https://user-images.githubusercontent.com/139274/39706997-64c957da-5213-11e8-90ad-321610ce9a03.png)

Now open a command windows and set the current directory to the folder where the images are (i.e. Desktop/Test) and execute the command  `moregen `:

![image](https://user-images.githubusercontent.com/139274/39707445-c01c5e24-5214-11e8-9c46-b089f4aad2dc.png)

Et voilà, now you can find two new folders, named "iOS" and "Android", where you can find all the resized artifacts that yuo can use on your mobile apps.

## License
My code is licensed with the M.I.T. License and because I use the ImageMagick library to open, resize and save images, please take note also of their license, both available at the root of this project.
