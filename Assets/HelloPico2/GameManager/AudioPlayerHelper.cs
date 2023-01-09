using UnityEngine;

public static class AudioPlayerHelper
{    
    public static void PlayAudio(string clipName, Vector3 pos)
    {
        if (clipName == "") return;
        Project.EventBus.Post(new HelloPico2.AudioEventRequested(clipName, pos));
    }
    public static void PlayMultipleAudio(string clipName, Vector3 pos)
    {
        var audioEvent = new HelloPico2.AudioEventRequested(clipName, pos);

        audioEvent.UsingMultipleAudioClips = true;

        Project.EventBus.Post(audioEvent);
    }
    public static void PlayMultipleAudio(string clipName, int index, Vector3 pos)
    {
        var audioEvent = new HelloPico2.AudioEventRequested(clipName, pos);
        
        audioEvent.ClipsIndex = index;

        audioEvent.UsingMultipleAudioClips = true;

        Project.EventBus.Post(audioEvent);
    }
    public static string PlayRandomAudio(string[] clipName, Vector3 pos)
    {
        var value = Random.Range(0, clipName.Length);
        
        Project.EventBus.Post(new HelloPico2.AudioEventRequested(clipName[value], pos));

        return clipName[value];
    }
}
