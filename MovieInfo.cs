public class MovieInfo
{    
    private uint year;
    private string title;

    public uint Year
    {
        get { return year; }
    }
    public string Title
    {
        get { return title; }
    }
    
    public MovieInfo(uint year, string title)
    {
        this.year = year;
        this.title = title;
    }
}