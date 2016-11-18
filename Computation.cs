using System;
using System.Collections.Generic;

public class Computation
{
    // Correlation gives us the similarity between two users a and i.
    // Can range between 0 and 1 where 0 is that they share nothing in
    // common and 1 is where they share everything in common.
    public float Correlation(Movies movies, UserInfo a, UserInfo i)
    {
        float correlation = 0.0f;
        
        foreach(KeyValuePair<uint, MovieInfo> movie in movies.GetDataset())
        {
            float aRating = 0.0f;
            float iRating = 0.0f;

            if(a.RatingExists(movie.Key))
                aRating = a.GetRating(movie.Key);
            if(i.RatingExists(movie.Key))
                iRating = i.GetRating(movie.Key);

            // if either are zero, calculations are unnecessary.
            if(aRating != 0.0f || iRating != 0.0f)
            {
                float aNormalizedRating = 0.0f;
                float iNormalizedRating = 0.0f;

                foreach(KeyValuePair<uint, float> aUserData in a.GetDataset())
                {
                    aNormalizedRating += aUserData.Value * aUserData.Value;    
                }  

                foreach(KeyValuePair<uint, float> iUserData in i.GetDataset())
                {
                    iNormalizedRating += iUserData.Value * iUserData.Value;
                }

                correlation += (aRating / (float)Math.Sqrt(aNormalizedRating)) * 
                    (iRating / (float)Math.Sqrt(iNormalizedRating));
            }
        }

        return correlation;
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

    public float PredictedRating(float testingRating, float correlation, float weightedSum)
    {
        return 0.0f;
    }

    // Returns the average rating of the current user.
    public float WeightedSumOfUser(UserInfo user)
    {
        float sumRatings = 0.0f;
        foreach(KeyValuePair<uint, float> rating in user.GetDataset())
        {
            sumRatings += rating.Value;
        }

        return sumRatings / Math.Abs(user.GetDataset().Count);
    }

    public float SumWeight(Dictionary<Tuple<uint, uint>, float> weight)
    {
        float sumWeights = 0.0f; 
        
        foreach(KeyValuePair<Tuple<uint, uint>, float> w in weight)
        {
            sumWeights += w.Value;
        }

        return sumWeights;
    }
}