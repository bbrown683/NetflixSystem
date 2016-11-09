using System;
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
            
            // Container objects
            Movies movies = new Movies();
            Users users = new Users();

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
                if(users.UserExists(userID))
                    users.GetUser(userID).AddRating(movieID, rating);
                else
                {
                    // Add user and rating
                    users.AddUser(userID, new UserInfo());
                    users.GetUser(userID).AddRating(movieID, rating);
                }
            }

            /*    
            if(movies.MovieExists(16))
                Console.WriteLine(movies.GetMovie(16).Title);

            Console.WriteLine(users.GetUser(411537).GetRating(8));
        
            if(users.GetRatingForUser(1748849, 8) != 0.0f)
                Console.WriteLine("Rating does exist");
            else
                Console.WriteLine("Rating does not exist");
            */

            Computation computation = new Computation();
            Console.WriteLine("Average vote of other users (excluding userID 361407) for movieID 8: " + computation.WeightedSumOfOtherUsers(users, 361407, 8));
            
            Console.WriteLine("Correlation between userID's 361407 and 1205593: " + computation.Correlation(movies, users.GetUser(361407), users.GetUser(1205593)));
        }
    }
}