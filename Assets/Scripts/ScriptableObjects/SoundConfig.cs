using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundConfig", menuName = "Scriptable Objects/SoundConfig")]
public class SoundConfig : ScriptableObject
{
    public List<Sound> TotalSounds;
}
