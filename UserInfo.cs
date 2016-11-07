using System.Collections.Generic;

public class UserInfo
{   
    private Dictionary<uint, float?> ratings;
    
    public UserInfo()
    {
        ratings = new Dictionary<uint, float?>();
    }

    public void AddRating(uint movieID, float rating)
    {
        ratings.Add(movieID, rating);
    }

    public float? GetRating(uint movieID)
    {
        if(ratings[movieID] != null)
            return ratings[movieID];
        else
            throw new KeyNotFoundException();
    }
}