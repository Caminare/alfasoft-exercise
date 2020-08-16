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

                HttpResponseMessage response = await client.GetAsync(baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(data);
                }

                else if(response.StatusCode == HttpStatusCode.NotFound)
                    Console.WriteLine("User not found");

                Console.WriteLine("\n -------------------------------------------------------------------------------------------- \n");

            }
        }
    }
}
