using System;
using System.IO;
using System.Linq;
/*
The intention of this project is to explore file-search parameters and modification.

Explicit use is to change the filename of collected groups of image-files with a 
user-specified prefix and number array. Ex: 'DSC12345.jpg' -> 'Summer of 2023_1.jpg' etc. 
*/
class Program
{

    static void Main()
    {

        Console.WriteLine("Choose the operation you want to perform: ");
        Console.WriteLine("1. Rename files in a directory");
        Console.WriteLine("2. Sort and move files to separate directories");

        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Renaming();
        }

        else if (choice == "2")
        {
            Sorting();
        }

        else
        {
            Console.WriteLine("Invalid input. Please enter 1 or 2.");
        }
    }


    /*
     (1) Renaming feature.
     Will loop through files in indicated directory and rename them with users indicated prefix. 
    */
    static void Renaming()
    {
        string directoryPath = "";

        //Program asks user to provide a directory-path until a existing one is provided.
        //At this point, program can be cancelled by entering 'exit' as input

        while (true)
        {
            Console.WriteLine("Please specify the directory containing your files, full directory path needed. Enter 'exit' to quit: ");
            directoryPath = Console.ReadLine();

            if (directoryPath.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nExiting program...");
                Console.ReadKey();
                break;
            }

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"\n{directoryPath} is not a directory. Please specify an existing directory: ");
            }
            else
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                
                /*
                Tried using a separate funtion to check for all image-file extensions, did not work.
                Instead passing it as a part of the GetFiles-method.
                After consideration - also adding common video-capture extensions.
                */

                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", "mp4", "mkv" };
                FileInfo[] files = directory.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => imageExtensions.Contains(f.Extension.ToLower()))
                    .OrderByDescending(f => f.CreationTime)
                    .ToArray();

                if (files.Length == 0)
                {
                    Console.WriteLine("\nNo image files found in the directory.");
                }
                else
                {
                    Console.WriteLine($"\nFound {files.Length} image files:");
                    foreach (FileInfo file in files)
                    {
                        Console.WriteLine(file.Name);
                    }

                    //Prefix is entered by the user, and number is declared - to be incremented for each instance

                    Console.WriteLine("\nPlease enter a prefix to start renaming process: ");

                    string prefix = Console.ReadLine();
                    int number = 1;

                    try
                    {
                        foreach (FileInfo file in files)
                        {
                            string newFileName = $"{prefix}_{number++}{file.Extension}";
                            string newFilePath = Path.Combine(file.Directory.FullName, newFileName);

                            //Do not understand why File.Move has to be used to rename a file..
                            //I suppose it's just how file-properties work. 

                            File.Move(file.FullName, newFilePath);
                            Console.WriteLine($"Renamed {file.Name} to {newFileName}");
                        }
                        Console.WriteLine("\nImage files have been renamed successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nAn exception occurred: {ex.Message}");
                    }
                }

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                break;
            }
        }
        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }


    /*
     Want separate function for suer to choose at start - if they want to sort files instead of renaming
     (2) Sorting feature.
     Will loop through files in indicated directory - sort them out according to CreationTime, create directories for year+month of creations and move files to correct directory.
     Much of the code similar to Renaming function and only slightly modifed to fit this purpose instead.
    */
    static void Sorting()
    {
        string source = "";


        while (true)
        {
            Console.WriteLine("Please specify the directory containing your files, full directory path needed. Enter 'exit' to quit: ");
            source = Console.ReadLine();


            if (source.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nExiting program...");
                Console.ReadKey();
                break;
            }

            if (!Directory.Exists(source))
            {
                Console.WriteLine("\nThat is not a valid directory path. Please, try again: ");
            }
            else
            {
                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(source);
                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", "mp4", "mkv" };
                FileInfo[] files = sourceDirectoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                .Where(f => imageExtensions.Contains(f.Extension.ToLower()))
                .OrderByDescending(f => f.CreationTime)
                .ToArray();


                if (files.Length == 0)
                {
                    Console.WriteLine("\nThere are no recognised image-files in this folder");
                }

                else
                {
                    Console.WriteLine($"Found {files.Length} image files.");
                    foreach (FileInfo file in files)
                    {
                        Console.WriteLine($"{file.Name}\t-\t{file.CreationTime}");
                    }

                    Console.WriteLine("\nDo you wish to move these files? (yes/no)");
                    string confirmation = Console.ReadLine();

                    if (confirmation.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (FileInfo file in files)
                        {

                            //Create destination folder path based on year and month of CreationTIme.
                            string destinationFolder = Path.Combine(source, file.CreationTime.Year.ToString("D4"), file.CreationTime.Month.ToString("D2"));

                            //Checks if destination folder exists - creates if not.
                            Directory.CreateDirectory(destinationFolder);

                            //Sets destination and moves file.
                            string destinationPath = Path.Combine(destinationFolder, file.Name);
                            file.MoveTo(destinationPath);
                        }

                        Console.WriteLine("Files moved successfully.");
                    }
                    else
                    {
                        Console.WriteLine("\nFiles were not moved. Exiting program.");
                        Console.ReadKey();
                        break;
                    }
                }
            }
        }
    }
}