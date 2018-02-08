using UnityEngine;

public class TGridController: MonoBehaviour
{
    public TGridInfo info;
    private float time;
    private float curTime;
    public StageController stage;
    public Line grid;
    private void CheckForReturn()
    {
        if (time > curTime)
        {
            grid.SetActive(false);
            stage.SetPrevLineID(info.id);
            stage.ReturnLine(this);
        }
    }
    public void ForceReturn()
    {
        grid.SetActive(false);
        stage.SetPrevLineID(info.id);
        stage.ReturnLine(this);
    }
    private void PositionUpdate()
    {
        float z = Parameters.maximumNoteRange / Parameters.NoteFallTime(stage.chartPlaySpeed) * (curTime - time);
        grid.MoveTo
        (
            new Vector3(-Parameters.maximumNoteWidth * 2, 0, z + 32),
            new Vector3(Parameters.maximumNoteWidth * 2, 0, z + 32)
        );
        ColorUpdate();
    }
    public void Activate(TGridInfo info, StageController stageController)
    {
        this.info = info;
        curTime = info.time;
        stage = stageController;
        ColorUpdate();
        Update();
    }
    private void ColorUpdate()
    {
        switch (info.type)
        {
            case TGridInfo.TGridType.ChangeTempo:
                grid.Color = new Color(0.0f, 0.0f, 0.5f);
                grid.AlphaMultiplier = 1.0f;
                break;
            case TGridInfo.TGridType.FreeTempo:
                grid.Color = new Color(0.0f, 0.5f, 0.0f);
                grid.AlphaMultiplier = 1.0f;
                break;
            case TGridInfo.TGridType.Beat:
                grid.Color = new Color(0.5f, 0.0f, 0.0f);
                grid.AlphaMultiplier = 1.0f;
                break;
            case TGridInfo.TGridType.SubBeat:
                grid.Color = new Color(42 / 255.0f, 42 / 255.0f, 42 / 255.0f);
                grid.AlphaMultiplier = 0.75f;
                break;
        }
    }
    private void Update()
    {
        time = stage.timeSlider.value;
        CheckForReturn();
        PositionUpdate();
    }
}

public class TGridInfo
{
    public int id = 0;
    public int eventId = 0;
    public float time;
    public float bpm = 0.0f;
    public enum TGridType
    {
        ChangeTempo,
        FreeTempo,
        Beat,
        SubBeat
    };
    public TGridType type;
}
