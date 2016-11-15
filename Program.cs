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
            StreamReader userTestingStream = new StreamReader(new FileStream("netflix/TestingRatings.txt", FileMode.Open));
            StreamReader userTrainingStream = new StreamReader(new FileStream("netflix/TrainingRatings.txt", FileMode.Open));
            //StreamReader userTestingStream = new StreamReader(new FileStream("netflix/reduced/TestingRatings-1.txt", FileMode.Open));
            //StreamReader userTrainingStream = new StreamReader(new FileStream("netflix/reduced/TrainingRatings-1.txt", FileMode.Open));
            
            // Container objects
            Movies movies = new Movies();
            Users trainingUsers = new Users();
            Users testingUsers = new Users();

            Computation computation = new Computation();

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

            Console.WriteLine("\nCommands:\nerror - computes the " +
            "error on each instance of the training set compared to the test set." +
            "\nexit - quits the program." +
            "\ncommand - displays all available commands." +
            "\nquery - allows you to perform queries as a user on a particular year.");

            string input = "";
            do
            {
                input = Console.ReadLine();
                switch(input)
                {
                    case "command":
                        Console.WriteLine("\nCommands:\nerror - computes the " +
                        "error on each instance of the training set compared to the test set." +
                        "\nexit - quits the program." +
                        "\ncommand - displays all available commands." +
                        "\nquery - allows you to perform queries as a user on a particular year.");
                        break;
                    case "error":
                        Console.WriteLine("\nComputing the error with regards to the test set.");
                        
                        // For each user we must go through each of their ratings and make a prediction.
                        foreach(KeyValuePair<uint, UserInfo> users in testingUsers.GetDataset())
                        {
                            foreach(KeyValuePair<uint, float> ratings in users.Value.GetDataset())
                            {
                                
                            }    
                        }
                        break;
                    case "query":
                        Console.Write("Enter a userID: ");
                        uint userID = uint.Parse(Console.ReadLine());
                        Console.Write("Enter a particular year: ");
                        uint year = uint.Parse(Console.ReadLine());

                        if(testingUsers.UserExists(userID))
                        {
                            Console.WriteLine("\nMovies you have watched:");
                            foreach(KeyValuePair<uint, float> movie in testingUsers.GetUser(userID).GetDataset())
                            {
                                if(movies.MovieExists(movie.Key))
                                {
                                    Console.WriteLine(movies.GetMovie(movie.Key).Title + ", " + 
                                    movies.GetMovie(movie.Key).Year + ": " + movie.Value);
                                }
                            }

                            // Gather the movies only produced in that particular year.
                            Movies particularMovies = new Movies();
                            foreach(KeyValuePair<uint, MovieInfo> movie in movies.GetDataset())
                                if(year == movie.Value.Year)
                                    particularMovies.AddMovie(movie.Key, movie.Value);

                            // Now we perform analysis on each of these movies.
                            
                        }
                        else
                            Console.WriteLine("Invalid userID");
                        break;
                    case "exit":
                        Console.WriteLine("Exiting database...");
                        break;
                    default:
                        Console.WriteLine("Unknown value has been input.");
                        break;
                }
            } while(input != "exit");

            /*
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
            */
            //Console.WriteLine("Average vote of other users (excluding userID 361407) for movieID 8: " + computation.WeightedSumOfOtherUsers(trainingUsers, 361407, 8));
            //Console.WriteLine("Correlation between userID's 361407 and 1205593: " + computation.Correlation(movies, trainingUsers.GetUser(361407), trainingUsers.GetUser(1205593)));
            
        }
    }
}