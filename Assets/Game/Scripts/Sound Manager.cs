using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    [Tooltip("ใส่ AudioSource สำหรับ Ambient หลายๆ ตัวได้ (เช่น 2 ตัวสำหรับซ้าย-ขวา)")]
    public AudioSource[] ambientSources; // เปลี่ยนเป็น Array เพื่อให้มีลำโพง Ambient หลายตัวได้

    [Header("Audio Clips")]
    public Sound[] bgmSounds;
    public Sound[] sfxSounds;
    public Sound[] ambientSounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string soundName)
    {
        Sound s = Array.Find(bgmSounds, x => x.name == soundName);
        if (s != null)
        {
            // เช็กว่ากำลังเล่นเพลงนี้อยู่หรือไม่ ถ้าเล่นอยู่แล้วให้ข้ามไปเลย
            if (bgmSource.clip == s.clip && bgmSource.isPlaying)
            {
                return;
            }

            bgmSource.clip = s.clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM: " + soundName + " ไม่พบในระบบ!");
        }
    }

    public void PlaySFX(string soundName)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == soundName);
        if (s != null) { sfxSource.PlayOneShot(s.clip); }
    }

    /// <summary>
    /// เล่นเสียง Ambient พร้อมปรับตำแหน่งซ้าย-ขวา
    /// </summary>
    /// <param name="soundName">ชื่อเสียง</param>
    /// <param name="panValue">ค่าความเอียงของเสียง (-1 = ซ้ายสุด, 0 = ตรงกลาง, 1 = ขวาสุด)</param>
    public void PlayAmbient(string soundName, float panValue = 0f)
    {
        Sound s = Array.Find(ambientSounds, x => x.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Ambient: " + soundName + " ไม่พบในระบบ!");
            return;
        }

        // 1. เช็กก่อนว่ามีลำโพงตัวไหนกำลังเล่นเสียงนี้อยู่แล้วหรือไม่
        foreach (AudioSource source in ambientSources)
        {
            if (source.clip == s.clip && source.isPlaying)
            {
                // ถ้าเล่นอยู่แล้ว เราจะแค่อัปเดตตำแหน่งซ้าย-ขวาให้เผื่อมีการเปลี่ยนแปลง แล้วจบการทำงาน
                source.panStereo = panValue;
                return;
            }
        }

        // 2. ถ้ายังไม่ได้เล่น ให้หาลำโพง Ambient ตัวที่ "ว่างอยู่" (ไม่ได้เล่นเสียงอะไร)
        AudioSource availableSource = Array.Find(ambientSources, x => !x.isPlaying);

        if (availableSource != null)
        {
            availableSource.clip = s.clip;
            availableSource.loop = true;
            availableSource.panStereo = panValue; // กำหนดค่า Pan (ซ้าย-ขวา)
            availableSource.Play();
        }
        else
        {
            Debug.LogWarning("ไม่มี AudioSource ว่างสำหรับเล่นเสียง Ambient: " + soundName);
        }
    }

    /// <summary>
    /// สั่งหยุดเสียง Ambient เฉพาะเสียงที่ระบุ
    /// </summary>
    public void StopAmbient(string soundName)
    {
        Sound s = Array.Find(ambientSounds, x => x.name == soundName);
        if (s != null)
        {
            foreach (AudioSource source in ambientSources)
            {
                if (source.clip == s.clip && source.isPlaying)
                {
                    source.Stop();
                }
            }
        }
    }
}