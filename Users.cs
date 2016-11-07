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
        if(UserExists(userID))
            return users[userID];
        throw new KeyNotFoundException();
    }

    public void AddUser(uint userID, UserInfo userInfo)
    {
        users.Add(userID, userInfo);
    }

    public Dictionary<uint, UserInfo> GetDataset()
    {
        return users;
    }
}