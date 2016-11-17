using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

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
            Dictionary<Tuple<UserInfo, UserInfo>, float> weight = new Dictionary<Tuple<UserInfo, UserInfo>, float>();

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
            "error on each instance of the test set on the training set." +
            "\nexit - quits the program." +
            "\ncommand - displays all available commands." +
            "\nquery - allows you to perform queries as a user on a particular year.");

            string input = "";
            do
            {
                Console.Write(">>>");
                input = Console.ReadLine();
                switch(input)
                {
                    case "command":
                        Console.WriteLine("\nCommands:\nerror - computes the " +
                        "error on each instance of the test set on the training set." +
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
                                Console.WriteLine("userID: " + users.Key + "\nmovieID: " + ratings.Key);
                                Console.WriteLine("Mean Absolute Error: ");
                                Console.WriteLine("Root Mean Squared Error: " + "\n");
                            } 
                        }
                        break;
                    case "query":
                        Console.Write("Enter a userID: ");
                        uint userID = uint.Parse(Console.ReadLine());
                        Console.Write("Enter a particular year: ");
                        uint year = uint.Parse(Console.ReadLine());

                        if(trainingUsers.UserExists(userID))
                        {
                            Console.WriteLine("\nMovies you have watched:");
                            foreach(KeyValuePair<uint, float> movie in trainingUsers.GetUser(userID).GetDataset())
                            {
                                if(movies.MovieExists(movie.Key))
                                {
                                    Console.WriteLine(movies.GetMovie(movie.Key).Title + ", " + 
                                    movies.GetMovie(movie.Key).Year + ": " + movie.Value);
                                }
                            }
                            
                            Console.WriteLine("Average rating is: " + computation.WeightedSumOfUser(trainingUsers.GetUser(userID)));

                            // Gather the movies only produced in that particular year.
                            Movies particularMovies = new Movies();
                            foreach(KeyValuePair<uint, MovieInfo> movie in movies.GetDataset())
                                if(year == movie.Value.Year)
                                    particularMovies.AddMovie(movie.Key, movie.Value);

                            // Now we perform analysis on each of these movies.
                            uint test0 = 0;
                            uint test1 = 0;
                            foreach(KeyValuePair<uint, UserInfo> user in trainingUsers.GetDataset())
                            {
                                float correlation = computation.Correlation(particularMovies, trainingUsers.GetUser(userID), user.Value);
                                if(correlation > 0)
                                {
                                    Tuple<UserInfo, UserInfo> tupleCorrelation = new Tuple<UserInfo, UserInfo>(trainingUsers.GetUser(userID), user.Value);
                                    // sort by userID. so we have a distinct set.
                                    if(userID <= user.Key)
                                        tupleCorrelation = new Tuple<UserInfo, UserInfo>(trainingUsers.GetUser(userID), user.Value);
                                    else
                                        tupleCorrelation = new Tuple<UserInfo, UserInfo>(user.Value, trainingUsers.GetUser(userID));
                                    test0++;
                                    // store weights in dictionary with the users as the key.
                                    if(!weight.ContainsKey(tupleCorrelation))
                                    {
                                        weight.Add(tupleCorrelation, correlation);
                            
                                    }
                                }
                                /*
                                foreach(KeyValuePair<uint, MovieInfo> movie in particularMovies.GetDataset())
                                {
                                    if(user.Value.RatingExists(movie.Key))
                                    {
                                        //Console.WriteLine(movie.Value.Title + ", " + movie.Value.Year);
                                        //float weightedSum = computation.WeightedSumOfOtherUsers(trainingUsers, userID, movie.Key);
                                        //Console.WriteLine("WeightedSum: " + weightedSum);
                                    }
                                }
                                */
                            }
                            Console.WriteLine(weight.Count);
                            Console.WriteLine(test0 + ", " + test1);               
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
        }
    }
}