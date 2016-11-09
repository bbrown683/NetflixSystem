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
                //Console.WriteLine(movieID + ": " + aRating + ", " + iRating);

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
        return 0.0f;
    }

    public float MeanRating(UserInfo v)
    {
        return 0.0f;
    }

    public float WeightedSumOfOtherUsers(Users users, uint activeUserID, uint movieID)
    {
        uint numRatings = 0;
        float sumRatings = 0.0f;

        foreach(KeyValuePair<uint, UserInfo> data in users.GetDataset())
        {
            if(data.Key != activeUserID)
            {
                float rating = users.GetRatingForUser(data.Key, movieID);
                if(rating != 0.0f)
                {
                    numRatings++;
                    sumRatings += rating;
                }
            }
        }

        return sumRatings / numRatings;
    }
}