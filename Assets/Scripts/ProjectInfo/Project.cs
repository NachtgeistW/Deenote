[System.Serializable]
public class ProjectV1 // Saves the project info and settings, audio clip not included
{
    public string name = ""; // The name of the project
    public string chartMaker = ""; // The name of the chart maker
    public string songName = ""; // The name of the song (the file)(includes extension) - cannot be changed after creating the project
    public ChartV1[] charts = { }; // Charts in the project
}

[System.Serializable]
public class ProjectV2
{
    public string name = "";
    public string chartMaker = "";
    public string artistName = ""; // New artist name field
    public string songName = "";
    public ChartV2[] charts = { };
}