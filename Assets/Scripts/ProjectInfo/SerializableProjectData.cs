[System.Serializable]
public class ProjectFileDataV1 //Saves all the project data including the audio clip data
{
    public ProjectV1 project = null; //Other project data
    //Audio clip data
    public float[] sampleData = { };
    public int frequency = 0;
    public int channel = 0;
    public int length = 0; //Length is in samples
}

[System.Serializable]
public class ProjectFileDataV2
{
    public ProjectV2 project = null;
    //Audio clip data
    public float[] sampleData = { };
    public int frequency = 0;
    public int channel = 0;
    public int length = 0; //Length is in samples
}