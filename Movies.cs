using System.Collections.Generic;

public class Movies
{
    Dictionary<uint, MovieInfo> movies;

    public Movies()
    {
        movies = new Dictionary<uint, MovieInfo>();
    }

    public MovieInfo GetMovie(uint movieID)
    {
        if(movies.ContainsKey(movieID))
            return movies[movieID];
        else
            throw new KeyNotFoundException();
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