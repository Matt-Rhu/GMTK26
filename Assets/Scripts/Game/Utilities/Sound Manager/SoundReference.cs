using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SoundReference", menuName = "FX Data/Sound Reference", order = 2)]
public class SoundReference : ScriptableObject
{
    public new string name;
    public EventReference Ref;
    public string[] parameters;
}