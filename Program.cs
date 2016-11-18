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
            //StreamReader userTestingStream = new StreamReader(new FileStream("netflix/reduced/TestingRatings-10.txt", FileMode.Open));
            //StreamReader userTrainingStream = new StreamReader(new FileStream("netflix/reduced/TrainingRatings-10.txt", FileMode.Open));
            
            // Container objects
            Movies movies = new Movies();
            Users trainingUsers = new Users();
            Users testingUsers = new Users();

            Computation computation = new Computation();

            // stores the weights (correlation) between two users.
            Dictionary<Tuple<uint, uint>, float> weight;

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
                        weight = new Dictionary<Tuple<uint, uint>, float>();

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
                        weight = new Dictionary<Tuple<uint, uint>, float>();
                        
                        Console.Write("Enter a userID: ");
                        uint userID = uint.Parse(Console.ReadLine());
                        Console.Write("Enter a particular year: ");
                        uint year = uint.Parse(Console.ReadLine());

                        List<float> predictedRatings = new List<float>();

                        if(trainingUsers.UserExists(userID))
                        {
                            Console.WriteLine("\nMovies you have watched:");
                            foreach(KeyValuePair<uint, float> movie in trainingUsers.GetUser(userID).GetDataset())
                                if(movies.MovieExists(movie.Key))
                                    Console.WriteLine(movie.Key + " - " + movies.GetMovie(movie.Key).Title + ", " + 
                                        movies.GetMovie(movie.Key).Year + ": " + movie.Value);
                            
                            // Gather the movies only produced in that particular year.
                            Movies particularMovies = new Movies();
                            foreach(KeyValuePair<uint, MovieInfo> movie in movies.GetDataset())
                                if(year == movie.Value.Year)
                                    particularMovies.AddMovie(movie.Key, movie.Value);
                            
                            // Computes the average rating of the active user.
                            float averageRatingOfActive = computation.WeightedSumOfUser(trainingUsers.GetUser(userID));
                            
                            // Now we perform analysis on each of these movies.
                            foreach(KeyValuePair<uint, MovieInfo> movie in particularMovies.GetDataset())
                            {
                                float predictedRating = 0.0f;
                                float cumulativeSum = 0.0f;
                                foreach(KeyValuePair<uint, UserInfo> user in trainingUsers.GetDataset())
                                {
                                    //Console.WriteLine("User: " + user.Key);
                                    float correlation = computation.Correlation(particularMovies, trainingUsers.GetUser(userID), user.Value);
                                    
                                    // we only want nonzero weights to be included into the dictionary.
                                    if(correlation > 0.0f)
                                    {
                                        var tuple = new Tuple<uint, uint>(userID, user.Key);
                                        // store weights in dictionary with the users as the key.
                                        if(!weight.ContainsKey(tuple))
                                            weight.Add(tuple, correlation);

                                        if(user.Value.RatingExists(movie.Key))
                                        {
                                            cumulativeSum += correlation * (user.Value.GetRating(movie.Key) - 
                                                computation.WeightedSumOfUser(user.Value));
                                            //Console.WriteLine("cumulativeSum of movieID " + movie.Key + 
                                            //" for userID " + user.Key + " is " + cumulativeSum);
                                        }  
                                    }
                                }
                                
                                predictedRating = averageRatingOfActive + cumulativeSum / computation.SumWeight(weight);
                                predictedRatings.Add(predictedRating);
                                Console.WriteLine("PR of movieID " + movie.Key + " is " + predictedRating);   
                            }

                            predictedRatings.Sort();
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