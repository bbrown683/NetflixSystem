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
            
            // Container objects
            Movies movies = new Movies();
            Users users = new Users();

            while(!movieStream.EndOfStream)
            {
                string[] movieData = movieStream.ReadLine().Split(new char[] {','}, 3);

                uint movieID = uint.Parse(movieData[0]);
                uint movieYear = uint.Parse(movieData[1]);
                string movieTitle = movieData[2];

                movies.AddMovie(movieID, new MovieInfo(movieYear, movieTitle));
            }

            while(!userTrainingStream.EndOfStream)
            {
                string[] trainingData = userTrainingStream.ReadLine().Split(',');

                uint userID = uint.Parse(trainingData[0]);
                uint movieID = uint.Parse(trainingData[1]);
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
            
            Console.WriteLine(movies.GetMovie(16).Title);
            Console.WriteLine(users.GetUser(8).GetRating(306466));
        }
    }
}