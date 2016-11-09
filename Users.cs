using System.Collections.Generic;

public class Users
{
    Dictionary<uint, UserInfo> users;

    public Users()
    {
        users = new Dictionary<uint, UserInfo>();
    }

    public bool UserExists(uint userID)
    {
        if(users.ContainsKey(userID))
            return true;
        return false;
    }

    public UserInfo GetUser(uint userID)
    {
        return users[userID];
    }

    public void AddUser(uint userID, UserInfo userInfo)
    {
        users.Add(userID, userInfo);
    }

    // Retrieves a rating of a users for a specific movie. 
    // It will return the rating if it exists, or 0 if it does not.
    public float GetRatingForUser(uint userID, uint movieID)
    {
        if(UserExists(userID))
            if(GetUser(userID).RatingExists(movieID))
                return GetUser(userID).GetRating(movieID);
        return 0.0f;
    }

    public Dictionary<uint, UserInfo> GetDataset()
    {
        return users;
    }
}