using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
                CloseApp();
            }


            Console.WriteLine("\n -------------------------------------------------------------------------------------------- \n");

            foreach (string user in usersFile)
            {
                bool last = usersFile.Last() == user;
                GetUserData(user);

                if (!last)
                    Thread.Sleep(5000);
                else
                    CloseApp();
            }

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

        private static async void GetUserData(string user)
        {
            string baseUrl = "https://api.bitbucket.org/2.0/users/" + user;

            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("User {0}", user);
                Console.WriteLine("Requesting {0}", baseUrl);

                HttpResponseMessage response = await client.GetAsync(baseUrl);

                string data = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: {0}", data);

                Console.WriteLine("\n -------------------------------------------------------------------------------------------- \n");

                LogRequestToFile(user, baseUrl, data);

            }
        }


        private static void LogRequestToFile(string user, string request, string data)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentPath, "requests-log.txt");

            using(StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("User {0}", user);
                writer.WriteLine("Request: {0}", request);
                writer.WriteLine("Response: {0}", data);
                writer.WriteLine("-------------------------------------------------------------------------------------");
            }

        }
    }
}
