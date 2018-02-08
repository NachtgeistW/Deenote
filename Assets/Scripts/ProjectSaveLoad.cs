using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;

public class ProjectSaveLoad : MonoBehaviour
{
    public GameObject saveCompleteText;
    public GameObject loadCompleteText;
    public GameObject backGroundImageLeft;
    public GameObject savingText;
    public GameObject loadingText;
    public StageController stage;
    private ProjectFileDataV2 projectData = null;
    private void Save(ProjectFileDataV2 projectData, string fileFullName)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(fileFullName, FileMode.Create);
        BinaryWriter binaryWriter = new BinaryWriter(fileStream);
        binaryWriter.Write("dsprojv");
        binaryWriter.Write(2);
        binaryWriter.Close();
        binaryFormatter.Serialize(fileStream, projectData);
        fileStream.Close();
    }
    private void Load(string fileFullName)
    {
        projectData = null;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(fileFullName, FileMode.Open);
        try
        {
            ProjectFileDataV1 projectDataV1 = null;
            projectDataV1 = (ProjectFileDataV1)binaryFormatter.Deserialize(fileStream); // Project file version 1
            fileStream.Close();
            ProjectV1 projectV1 = projectDataV1.project;
            ProjectV2 projectV2 = new ProjectV2
            {
                name = projectV1.name,
                chartMaker = projectV1.chartMaker,
                artistName = "",
                songName = projectV1.songName,
                charts = new ChartV2[4]
            };
            for (int i = 0; i < 4; i++)
            {
                ChartV1 chartV1 = projectV1.charts[i];
                List<TempoEvent> tempoEvents = new List<TempoEvent>();
                List<float> beats = chartV1.beats;
                for (int j = 0; j < beats.Count - 1; j++)
                {
                    TempoEvent tempoEvent = new TempoEvent
                    {
                        bpm = 60.0f / (beats[j + 1] - beats[j]),
                        time = beats[j]
                    };
                    tempoEvents.Add(tempoEvent);
                }
                if (beats.Count > 0)
                {
                    TempoEvent tempoEvent = new TempoEvent
                    {
                        bpm = 0.0f,
                        time = beats[beats.Count - 1]
                    };
                    tempoEvents.Add(tempoEvent);
                }
                projectV2.charts[i] = new ChartV2
                {
                    speed = chartV1.speed,
                    difficulty = chartV1.difficulty,
                    level = chartV1.level,
                    tempoEvents = tempoEvents,
                    notes = chartV1.notes
                };
            }
            projectData = new ProjectFileDataV2
            {
                project = projectV2,
                sampleData = projectDataV1.sampleData,
                frequency = projectDataV1.frequency,
                channel = projectDataV1.channel,
                length = projectDataV1.length
            };
        }
        catch (SerializationException)
        {
            string head;
            int ver;
            BinaryReader binaryReader = new BinaryReader(fileStream);
            head = binaryReader.ReadString();
            if (head != "dsprojv")
            {
                binaryReader.Close();
                fileStream.Close();
                // Insert error handle method here...
            }
            else
            {
                ver = binaryReader.ReadInt32();
                binaryReader.Close();
                switch (ver)
                {
                    case 2: // Project file version 2, starts from Deenote 0.6.6
                        projectData = (ProjectFileDataV2)binaryFormatter.Deserialize(fileStream);
                        break;
                    default:
                        fileStream.Close();
                        // Insert error handle method here...
                        break;
                }
            }
        }
    }
    public IEnumerator SaveProjectIntoFile(ProjectV2 project, AudioClip clip, string fileFullName) //Save the project in file fileFullName
    {
        stage.forceToPlaceNotes = true;
        if (backGroundImageLeft.activeInHierarchy == true)
            savingText.GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        else
            savingText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        savingText.SetActive(true);
        projectData = new ProjectFileDataV2
        {
            project = project,
            length = clip.samples,
            frequency = clip.frequency,
            channel = clip.channels
        };
        projectData.sampleData = new float[projectData.length * projectData.channel];
        clip.GetData(projectData.sampleData, 0);
        Thread saveThread = new Thread(() => Save(projectData, fileFullName));
        saveThread.Start();
        while (saveThread.IsAlive) yield return null;
        savingText.SetActive(false);
        if (backGroundImageLeft.activeInHierarchy == true)
            saveCompleteText.GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        else
            saveCompleteText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        saveCompleteText.SetActive(true);
        stage.forceToPlaceNotes = false;
        yield return new WaitForSeconds(3.0f);
        saveCompleteText.SetActive(false);
    }
    public IEnumerator LoadProjectFromFile(Action<ProjectV2> project, Action<AudioClip> clip, string fileFullName) //Load a project from a project file
    {
        stage.forceToPlaceNotes = true;
        if (backGroundImageLeft.activeInHierarchy == true)
            loadingText.GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        else
            loadingText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        loadingText.SetActive(true);
        AudioClip newClip = null;
        Thread loadThread = new Thread(() => Load(fileFullName));
        loadThread.Start();
        while (loadThread.IsAlive) yield return null;
        newClip = AudioClip.Create("SongAudioClip", projectData.length, projectData.channel, projectData.frequency, false);
        newClip.SetData(projectData.sampleData, 0);
        project(projectData.project); clip(newClip);
        loadingText.SetActive(false);
        if (backGroundImageLeft.activeInHierarchy == true)
            loadCompleteText.GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        else
            loadCompleteText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        loadCompleteText.SetActive(true);
        stage.forceToPlaceNotes = false;
    }
}
