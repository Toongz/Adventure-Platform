using UnityEngine;

public enum SoundType
{
    Jump, Jump_On_Enemy, Hit

}
[System.Serializable]
public class Sound
{
    public string SoundName;
    public SoundType Type;
    public AudioClip Audio;
}
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager Instance => _instance;

    [SerializeField] private SoundConfig _soundConfig;
    [SerializeField] private AudioSource _soundPlayer;
    [SerializeField] private AudioSource _musicPlayer;

    private bool _soundOn = true;

    private void Awake()
    {
        _instance = this;
    }


    public void OnPlaySound(SoundType type)
    {
        if (!_soundOn) return;

        Sound sound = _soundConfig.TotalSounds.Find(x => x.Type == type);
        if(sound != null)
        {
            _soundPlayer.PlayOneShot(sound.Audio);
        }
    }
}
