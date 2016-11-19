using System;
using System.Collections.Generic;

public class Computation
{
    /*
    // Algorithm 3.
    // Correlation gives us the similarity between two users a and i.
    // Can range between 0 and 1 where 0 is that they share nothing in
    // common and 1 is where they share everything in common.
    public float Correlation(Movies movies, UserInfo a, UserInfo i)
    {
        float correlation = 0.0f;
        
        // we normalize the ratings of the users.
        float aNormalizedRating = 0.0f;
        float iNormalizedRating = 0.0f;
            
        foreach(KeyValuePair<uint, float> aUserData in a.GetDataset())
            aNormalizedRating += aUserData.Value * aUserData.Value;    

        foreach(KeyValuePair<uint, float> iUserData in i.GetDataset())
            iNormalizedRating += iUserData.Value * iUserData.Value;

        Movies ratedMovies = MoviesRatedByBoth(movies, a, i);
        foreach(KeyValuePair<uint, MovieInfo> movie in ratedMovies.GetDataset())
        {
            float aRating = a.GetRating(movie.Key);
            float iRating = i.GetRating(movie.Key);

            correlation += (aRating / (float)Math.Sqrt(aNormalizedRating)) * 
                (iRating / (float)Math.Sqrt(iNormalizedRating));
        }

        return correlation;
    }
    */

    // Algorithm 2.
    // Correlation gives us the similarity between two users a and i.
    // Can range between -1 and 1 where 0 is that they share nothing in
    // common and 1 is where they share everything in common.
    public float Correlation(Movies movies, UserInfo a, UserInfo i)
    {
        Movies ratedMovies = MoviesRatedByBoth(movies, a, i);

        // Compute the average ratings for the set of rated movies.
        float aAverageRating = 0.0f;
        float iAverageRating = 0.0f;
        foreach(KeyValuePair<uint, MovieInfo> movie in ratedMovies.GetDataset())
        {
            aAverageRating += a.GetRating(movie.Key);
            iAverageRating += i.GetRating(movie.Key);
        }

        aAverageRating = aAverageRating / ratedMovies.GetDataset().Count;
        iAverageRating = iAverageRating / ratedMovies.GetDataset().Count;

        float topResult = 0.0f;
        float bottomAResult = 0.0f;
        float bottomIResult = 0.0f;
        foreach(KeyValuePair<uint, MovieInfo> movie in ratedMovies.GetDataset())
        {
            topResult += (a.GetRating(movie.Key) - aAverageRating) * (i.GetRating(movie.Key) - iAverageRating);
            bottomAResult += (a.GetRating(movie.Key) - aAverageRating) * (a.GetRating(movie.Key) - aAverageRating);
            bottomIResult += (i.GetRating(movie.Key) - iAverageRating) * (i.GetRating(movie.Key) - iAverageRating);  
        } 
        
        float correlation = (topResult / (float)(Math.Sqrt(bottomAResult) * Math.Sqrt(bottomIResult)));

        return correlation;
    }

    public Movies MoviesRatedByBoth(Movies movies, UserInfo a, UserInfo i)
    {
        Movies ratedMovies = new Movies();
        
        foreach(KeyValuePair<uint, MovieInfo> movie in movies.GetDataset())
            if(a.RatingExists(movie.Key) && i.RatingExists(movie.Key))
                ratedMovies.AddMovie(movie.Key, movie.Value);

        return ratedMovies;
    }

    public float MeanAbsoluteError(float predictedRating, float trueRating)
    {
        return (float)Math.Abs(trueRating - predictedRating);
    }

    public float MeanRating(Users training, Users testing)
    {
        float mean = 0;
        uint numUsers = 0;

        foreach(KeyValuePair<uint, UserInfo> user in training.GetDataset())
        {
            numUsers++;
        }

        return mean / numUsers;
    }

    // Returns the average rating of the current user on a set of movies.
    public float WeightedSumOfUser(UserInfo user)
    {
        float sumRatings = 0.0f;
        foreach(KeyValuePair<uint, float> rating in user.GetDataset())
        {
            sumRatings += rating.Value;
        }

        return sumRatings / user.GetDataset().Count;
    }

    public float SumWeight(Dictionary<Tuple<uint, uint>, float> weight)
    {
        float sumWeights = 0.0f; 
        
        foreach(KeyValuePair<Tuple<uint, uint>, float> w in weight)
            sumWeights += Math.Abs(w.Value);

        return sumWeights;
    }
}