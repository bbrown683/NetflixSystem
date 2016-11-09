using System.Collections.Generic;

public class UserInfo
{   
    private Dictionary<uint, float> ratings;
    
    public UserInfo()
    {
        ratings = new Dictionary<uint, float>();
    }

    public bool RatingExists(uint movieID)
    {
        if(ratings.ContainsKey(movieID))
            return true;
        return false;
    }

    public void AddRating(uint movieID, float rating)
    {
        ratings.Add(movieID, rating);
    }

    public float GetRating(uint movieID)
    {
        return ratings[movieID];
    }

    public Dictionary<uint, float> GetDataset()
    {
        return ratings;
    }
}