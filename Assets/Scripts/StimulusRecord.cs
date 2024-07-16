using CsvHelper.Configuration.Attributes;

public class StimulusRecord
{
    public float Time { get; set; } = 0.0f;
    [Optional]
    public float Pitch { get; set; } = 0.0f;
    [Optional]
    public float Roll { get; set; } = 0.0f;
    [Optional]
    public float Yaw { get; set; } = 0.0f;
    [Optional]
    public float Trans_Ap { get; set; } = 0.0f;
    [Optional]
    public float Trans_Ml { get; set; } = 0.0f;
    [Optional]
    public float Trans_Ud { get; set; } = 0.0f;
    [Optional]
    public float Scale { get; set; } = 1.0f;
}
