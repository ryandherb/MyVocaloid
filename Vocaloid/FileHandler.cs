using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Vocaloid;

public class TrackData
{
    [JsonPropertyName("trackName")]
    public string TrackName { get; set; } = "";

    [JsonPropertyName("tempo")]
    public int Tempo { get; set; } = 0;

    [JsonPropertyName("notes")]
    public List<NoteData> Notes { get; set; } = [];
}

public class NoteData
{
    [JsonPropertyName("note")]
    public string Note { get; set; } = "";

    [JsonPropertyName("duration")]
    public double Duration { get; set; } = 0.0;

    [JsonPropertyName("pitch")]
    public int Pitch { get; set; } = 0;
}