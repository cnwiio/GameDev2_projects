using System.Runtime.CompilerServices;
using UnityEngine;

public class GetAudio : MonoBehaviour
{
    private SoundManager soundManager => FindAnyObjectByType<SoundManager>();
    public void playClick()
    {
        soundManager.PlaySFX("Click");
    }

    public void StopAmbient(string name)
    {
        soundManager.StopAmbient(name);
    }
}
