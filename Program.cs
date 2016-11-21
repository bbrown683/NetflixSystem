using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            StreamReader userTestingStream = new StreamReader(new FileStream("netflix/reduced/TestingRatings-new.txt", FileMode.Open));
            StreamReader userTrainingStream = new StreamReader(new FileStream("netflix/reduced/TrainingRatings-new.txt", FileMode.Open));
            
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

                        Console.WriteLine("\nComputing the error with regards to the test set. This may take a few minutes.");

                        float differenceRating = 0.0f;
                        float differenceRatingSquared = 0.0f;
                        float numRatings = 0.0f; 

                        foreach(KeyValuePair<uint, UserInfo> users in testingUsers.GetDataset())
                        {
                            weight = new Dictionary<Tuple<uint, uint>, float>();

                            // ensure the user exists in the training data as well.
                            // otherwise we cannot predict their rating.
                            if(trainingUsers.UserExists(users.Key))
                            {
                                // Computes the average rating of the active user.
                                float averageRatingOfActive = computation.WeightedSumOfUser(trainingUsers.GetUser(users.Key));

                                // compute weights.
                                foreach(KeyValuePair<uint, UserInfo> otherUsers in trainingUsers.GetDataset())
                                {
                                    float correlation = computation.Correlation(movies, trainingUsers.GetUser(users.Key), otherUsers.Value);

                                    // exclude any zero correlations, and some numbers are just too small.
                                    if(correlation != 0.0f && !float.IsNaN(correlation))
                                        weight.Add(new Tuple<uint, uint>(users.Key, otherUsers.Key), correlation);
                                }
    
                                float sumWeight = computation.SumWeight(weight);
                                foreach(KeyValuePair<uint, float> ratings in users.Value.GetDataset())
                                {

                                    float predictedRating = 0.0f;
                                    float cumulativeSum = 0.0f;

                                    foreach(KeyValuePair<Tuple<uint, uint>, float> weights in weight)
                                        if(trainingUsers.GetUser(weights.Key.Item2).RatingExists(ratings.Key))
                                            cumulativeSum += weights.Value *
                                                (trainingUsers.GetRatingForUser(weights.Key.Item2, ratings.Key) - 
                                                    computation.WeightedSumOfUser(trainingUsers.GetUser(weights.Key.Item2)));
                                        else
                                            cumulativeSum += weights.Value;    
                                    // odd occurrence when there is no other weights.
                                    if(sumWeight != 0.0f)
                                        predictedRating = averageRatingOfActive + cumulativeSum / sumWeight;
                                    else
                                        predictedRating = averageRatingOfActive;
                                    
                                    // Logging.
                                    Console.WriteLine("==============================");
                                    Console.WriteLine("UserID: " + users.Key);
                                    Console.WriteLine("Predicted Rating of " + movies.GetMovie(ratings.Key).Title + " is " + predictedRating);
                                    Console.WriteLine("Actual Rating of " + movies.GetMovie(ratings.Key).Title + " is " + users.Value.GetRating(ratings.Key));

                                    // These simply return the difference in ratings and adds them up.
                                    // we will divide by the number of ratings to get the actual mean and squared error.
                                    differenceRating += computation.MeanAbsoluteError(predictedRating, users.Value.GetRating(ratings.Key));
                                    differenceRatingSquared += computation.MeanAbsoluteError(predictedRating, users.Value.GetRating(ratings.Key)) * 
                                        computation.MeanAbsoluteError(predictedRating, users.Value.GetRating(ratings.Key));
                                    numRatings++;
                                }
                            }
                        }   

                        float meanAbsoluteError = differenceRating / numRatings;
                        float rootMeanSquaredError = (float)Math.Sqrt(differenceRatingSquared / numRatings);
                        Console.WriteLine("MAE : " + meanAbsoluteError);
                        Console.WriteLine("RMSE : " + rootMeanSquaredError);

                        break;
                    case "query":
                        weight = new Dictionary<Tuple<uint, uint>, float>();
                        
                        Console.Write("Enter a userID: ");
                        uint userID = uint.Parse(Console.ReadLine());
                        Console.Write("Enter a particular year: ");
                        uint year = uint.Parse(Console.ReadLine());

                        List<Dictionary<float, MovieInfo>> predictedRatings = new List<Dictionary<float, MovieInfo>>();

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

                            // Compute the weights of the users and add them to the dictionary.
                            Console.WriteLine("Computing weights for users...");
                            foreach(KeyValuePair<uint, UserInfo> user in trainingUsers.GetDataset())
                            {
                                if(user.Key != userID)
                                {
                                    // Compute their correlation accross all movies in the database rather than the particular movies.
                                    float correlation = computation.Correlation(movies, trainingUsers.GetUser(userID), user.Value);
                                
                                    if(correlation != 0.0f && !float.IsNaN(correlation))
                                        weight.Add(new Tuple<uint, uint>(userID, user.Key), correlation);
                                }
                            }

                            float sumWeight = computation.SumWeight(weight);

                            // Now we perform analysis on each of these movies.
                            foreach(KeyValuePair<uint, MovieInfo> movie in particularMovies.GetDataset())
                            {
                                float predictedRating = 0.0f;
                                float cumulativeSum = 0.0f;

                                foreach(KeyValuePair<Tuple<uint, uint>, float> weights in weight)
                                    if(trainingUsers.GetUser(weights.Key.Item2).RatingExists(movie.Key))
                                        cumulativeSum += weights.Value *
                                            (trainingUsers.GetRatingForUser(weights.Key.Item2, movie.Key) - 
                                                computation.WeightedSumOfUser(trainingUsers.GetUser(weights.Key.Item2)));
                                    else
                                        cumulativeSum += weights.Value;    
                                // odd occurrence when there is no other weights.
                                if(sumWeight != 0.0f)
                                    predictedRating = averageRatingOfActive + cumulativeSum / sumWeight;
                                else
                                    predictedRating = averageRatingOfActive;
                                Console.WriteLine("Predicted Rating of " + movie.Value.Title + " is " + predictedRating);   
                            }
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