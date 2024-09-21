public static class PlayerAudio
{
    public static void NotifyDanger(bool value)
    {
        var clip = AudioManager.DangerClip;

        if (value)
        {
            AudioManager.Play(AudioManager.SourceType.Notification, clip, true);
        }
        else
        {
            AudioManager.Stop(AudioManager.SourceType.Notification, clip);
        }
    }
}
