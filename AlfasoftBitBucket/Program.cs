using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AlfasoftBitBucket
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = string.Empty;
            bool validPath = false;

            while (!validPath)
            {
                Console.WriteLine("Please, insert a valid path to the users file:");
                path = Console.ReadLine();

                validPath = IsPathValid(path);

                if (!validPath)
                    Console.WriteLine("The path is invalid");
                else
                    Console.WriteLine("Starting to process the file...");
            }


            string[] usersFile = ReturnUsers(path);

            if (usersFile.Length == 0)
            {
                Console.WriteLine("No users were found in the file!");
            }


            foreach (string user in usersFile)
            {
                Console.WriteLine(user);
            }

            CloseApp();

        }

        private static bool IsPathValid(string path)
        {
            //Checking if the file is a txt
            if (!path.Contains(".txt"))
                return false;
            //Else, check if the path exists
            return File.Exists(path);
        }

        private static string[] ReturnUsers(string path)
        {
            return File.ReadAllLines(path);
        }

        private static void CloseApp()
        {
            for (int i = 5; i > 0; i--)
            {
                Console.WriteLine("Closing the app in {0} seconds...", i);
                Thread.Sleep(1000);
            }

            Environment.Exit(0);
        }
    }
}
