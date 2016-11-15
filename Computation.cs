using System;
using System.Collections.Generic;

public class Computation
{
    // Correlation gives us the similarity between two users
    // a and i as a float between -1 and 1.
    public float Correlation(Movies movies, UserInfo a, UserInfo i)
    {
        float correlation = 0.0f;
        
        for(uint movieID = 1; movieID <= movies.GetDataset().Count; movieID++)
        {
            float aRating = 0.0f;
            float iRating = 0.0f;

            if(a.RatingExists(movieID))
                aRating = a.GetRating(movieID);
            if(i.RatingExists(movieID))
                iRating = i.GetRating(movieID);

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
        return trueRating - predictedRating;
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

    public float WeightedSumOfOtherUsers(Users users, uint activeUserID, uint movieID)
    {
        uint numRatings = 0;
        float sumRatings = 0.0f;

        foreach(KeyValuePair<uint, UserInfo> user in users.GetDataset())
        {
            if(user.Key != activeUserID)
            {
                float rating = users.GetRatingForUser(user.Key, movieID);
                    
                if(rating != 0.0f)
                {
                    numRatings++;
                    sumRatings += rating;
                }
            }
        }

        // there is a possibility that there is no other ratings for this movie.
        float sum = sumRatings / numRatings;
        return sum > 0.0f ? sum : 0.0f;
    }
}