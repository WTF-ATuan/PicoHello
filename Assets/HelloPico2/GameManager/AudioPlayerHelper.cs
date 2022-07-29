using UnityEngine;

public static class AudioPlayerHelper
{
    public static void PlayAudio(string clipName, Vector3 pos)
    {
        Project.EventBus.Post(new HelloPico2.AudioEventRequested(clipName, pos));
    }
    public static void PlayRandomAudio(string[] clipName, Vector3 pos)
    {
        var value = Random.Range(0, clipName.Length);

        Project.EventBus.Post(new HelloPico2.AudioEventRequested(clipName[value], pos));
    }
}
