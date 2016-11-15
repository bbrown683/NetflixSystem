using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Used to read the text files.
            StreamReader movieStream = new StreamReader(new FileStream("netflix/movie_titles.txt", FileMode.Open));
            //StreamReader userTestingStream = new StreamReader(new FileStream("netflix/TestingRatings.txt", FileMode.Open));
            //StreamReader userTrainingStream = new StreamReader(new FileStream("netflix/TrainingRatings.txt", FileMode.Open));
            StreamReader userTestingStream = new StreamReader(new FileStream("netflix/reduced/TestingRatings-1.txt", FileMode.Open));
            StreamReader userTrainingStream = new StreamReader(new FileStream("netflix/reduced/TrainingRatings-1.txt", FileMode.Open));
            
            // Container objects
            Movies movies = new Movies();
            Users trainingUsers = new Users();
            Users testingUsers = new Users();

            Console.WriteLine("Populating movie database...");
            while(!movieStream.EndOfStream)
            {
                string[] movieData = movieStream.ReadLine().Split(new char[] {','}, 3);

                uint movieID = uint.Parse(movieData[0]);
                uint movieYear = uint.Parse(movieData[1]);
                string movieTitle = movieData[2];

                movies.AddMovie(movieID, new MovieInfo(movieYear, movieTitle));
            }

            Console.WriteLine("Populating training database...");
            while(!userTrainingStream.EndOfStream)
            {
                string[] trainingData = userTrainingStream.ReadLine().Split(',');

                uint movieID = uint.Parse(trainingData[0]);
                uint userID = uint.Parse(trainingData[1]);
                float rating = float.Parse(trainingData[2]);

                // Check to see if user already exists in the dictionary.
                if(trainingUsers.UserExists(userID))
                    trainingUsers.GetUser(userID).AddRating(movieID, rating);
                else
                {
                    // Add user and rating
                    trainingUsers.AddUser(userID, new UserInfo());
                    trainingUsers.GetUser(userID).AddRating(movieID, rating);
                }
            }

            Console.WriteLine("Populating testing database...");
            while(!userTestingStream.EndOfStream)
            {
                string[] testingData = userTestingStream.ReadLine().Split(',');

                uint movieID = uint.Parse(testingData[0]);
                uint userID = uint.Parse(testingData[1]);
                float rating = float.Parse(testingData[2]);

                // Check to see if user already exists in the dictionary.
                if(testingUsers.UserExists(userID))
                    testingUsers.GetUser(userID).AddRating(movieID, rating);
                else
                {
                    // Add user and rating
                    testingUsers.AddUser(userID, new UserInfo());
                    testingUsers.GetUser(userID).AddRating(movieID, rating);
                }
            }

            Computation computation = new Computation();
            foreach(KeyValuePair<uint, UserInfo> users in testingUsers.GetDataset())
            {
                for(uint i = 0; i < movies.GetDataset().Count; i++)
                {
                    if(users.Value.RatingExists(i))
                        Console.WriteLine("Weighted sum for user " + users.Key + ": " + computation.WeightedSumOfOtherUsers(testingUsers, users.Key, i));
                }

                foreach(KeyValuePair<uint, UserInfo> otherUsers in testingUsers.GetDataset())
                {
                    if(users.Key != otherUsers.Key)
                    {
                        Console.WriteLine(computation.Correlation(movies, users.Value, otherUsers.Value));
                    }
                }
            }
            //Console.WriteLine("Average vote of other users (excluding userID 361407) for movieID 8: " + computation.WeightedSumOfOtherUsers(trainingUsers, 361407, 8));
            //Console.WriteLine("Correlation between userID's 361407 and 1205593: " + computation.Correlation(movies, trainingUsers.GetUser(361407), trainingUsers.GetUser(1205593)));
        }
    }
}