using System.Collections.Generic;

public class Movies
{
    Dictionary<uint, MovieInfo> movies;

    public Movies()
    {
        movies = new Dictionary<uint, MovieInfo>();
    }

    public bool MovieExists(uint movieID)
    {
        if(movies.ContainsKey(movieID))
            return true;
        return false;
    }

    public MovieInfo GetMovie(uint movieID)
    {
        return movies[movieID];
    }

    public void AddMovie(uint movieID, MovieInfo movieInfo)
    {
        movies.Add(movieID, movieInfo);
    }

    public Dictionary<uint, MovieInfo> GetDataset()
    {
        return movies;
    }
}