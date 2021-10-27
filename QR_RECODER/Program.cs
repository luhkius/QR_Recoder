using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ZXing;
using ImageMagick;

namespace QR_RECODER
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] inputFiles = null;

            if (args != null && args.Length > 0)
            {
                if (Directory.Exists(args[0]))
                {
                    //argument is a directory
                    inputFiles = Directory.EnumerateFiles(args[0], "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".bmp") || s.EndsWith(".gif")).ToArray();

                    for (int i = 0; i < inputFiles.Length; i++)
                    {
                        Console.WriteLine("File: " + inputFiles[i]);
                    }
                }
                else if (File.Exists(args[0]))
                {
                    //argument is a file
                    inputFiles = new string[] { args[0] };
                }
                else
                {
                    Console.WriteLine("Please specify an image file, or a directory containing image files!");
                    Console.WriteLine("Press enter to quit...");
                    Console.ReadLine();
                    return;
                }
            }
            else
            {
                Console.WriteLine("Please specify a file/folder by dragging it onto this executable.");
                Console.WriteLine("Press enter to quit...");
                Console.ReadLine();
                return;
            }

            for(int f = 0; f < inputFiles.Length; f++)
            {
                var reader1 = new ZXing.BarcodeReader();
                reader1.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };
                string outputString = null;
                MagickImage img = null;
                Result result = null;
                try
                {
                    img = new MagickImage(inputFiles[f]);
                    result = reader1.Decode(img);
                }
                catch
                {
                    Console.WriteLine("{0}: Failed to decode! Skipping...", inputFiles[f]);
                    continue;
                }
                if(result == null)
                {
                    Console.WriteLine("{0}: Failed to decode! Skipping...", inputFiles[f]);
                    continue;
                }

                outputString = result.Text;
                Console.WriteLine("{0}: Decoded: {1}", inputFiles[f], outputString);    

                ZXing.BarcodeWriter writer = new ZXing.BarcodeWriter();
                writer.Format = BarcodeFormat.QR_CODE;
                writer.Options.Height = 512;
                writer.Options.Width = 512;
                writer.Options.Margin = 1;
                var outputImage = writer.Write(outputString);
                outputImage.Save(inputFiles[f]+"_new.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            Console.WriteLine("Done! Press enter to close");
            Console.ReadLine();
        }
    }
}
