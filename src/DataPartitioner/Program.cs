using System;
using System.IO;
using System.Linq;

namespace DataPartitioner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Data Partitioning Application Started");
            
            try
            {
                // Get the path to the data directory (2 levels up)
                string currentDirectory = Directory.GetCurrentDirectory();
                string dataDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "../../data"));
                
                if (!Directory.Exists(dataDirectory))
                {
                    Console.WriteLine($"Data directory not found: {dataDirectory}");
                    return;
                }
                
                Console.WriteLine($"Processing folders in: {dataDirectory}");
                
                // Get all directories in the data directory
                string[] directories = Directory.GetDirectories(dataDirectory);
                Console.WriteLine($"Found {directories.Length} folders to organize");
                
                foreach (string directory in directories)
                {
                    // Skip directories that are just single letters (might be our target directories)
                    DirectoryInfo dirInfo = new DirectoryInfo(directory);
                    string folderName = dirInfo.Name;
                    
                    if (folderName.Length == 1 && char.IsLetter(folderName[0]))
                    {
                        Console.WriteLine($"Skipping letter directory: {folderName}");
                        continue;
                    }
                    
                    // Get the first letter of the folder name
                    if (folderName.Length > 0)
                    {
                        char firstChar = char.ToUpper(folderName[0]);
                        
                        // Only process if the first character is a letter
                        if (char.IsLetter(firstChar))
                        {
                            string letterDirectory = Path.Combine(dataDirectory, firstChar.ToString());
                            
                            // Create the letter directory if it doesn't exist
                            if (!Directory.Exists(letterDirectory))
                            {
                                Console.WriteLine($"Creating directory: {letterDirectory}");
                                Directory.CreateDirectory(letterDirectory);
                            }
                            
                            // Move the folder to the letter directory
                            string targetPath = Path.Combine(letterDirectory, folderName);
                            
                            if (!Directory.Exists(targetPath))
                            {
                                Console.WriteLine($"Moving {folderName} to {firstChar}/");
                                Directory.Move(directory, targetPath);
                            }
                            else
                            {
                                Console.WriteLine($"Target already exists: {targetPath}, skipping move operation");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Skipping {folderName} - doesn't start with a letter");
                        }
                    }
                }
                
                Console.WriteLine("Data partitioning completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
