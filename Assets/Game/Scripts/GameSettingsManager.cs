using UnityEngine;
using TMPro; // ใช้สำหรับ TextMeshPro
using System.Collections.Generic;

public class GameSettingsManager : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI vsyncText;
    public TextMeshProUGUI fpsText;

    [Header("Audio Text References")]
    public TextMeshProUGUI masterText;
    public TextMeshProUGUI sfxText;
    public TextMeshProUGUI musicText;    // เพิ่มบรรทัดนี้
    public TextMeshProUGUI ambientText;  // เพิ่มบรรทัดนี้

    [Header("Systems")]
    public AudioSettings audioSettings; // ลาก AudioSettings (หรือ SoundManager เดิม) มาใส่ช่องนี้

    // ตัวแปรเก็บข้อมูล
    private Resolution[] resolutions;
    private int currentResIndex = 0;

    private string[] vsyncOptions = { "Off", "On" };
    private int currentVsyncIndex = 0;

    private int[] fpsOptions = { 30, 60, 120, 144, -1 }; // -1 คือ ไม่จำกัด (Unlimited)
    private int currentFpsIndex = 1; // เริ่มที่ 60 FPS

    // ระดับเสียง 0 ถึง 10 (คิดเป็น 0% ถึง 100%)
    private int currentSfxLevel = 10;
    private int currentMasterLevel = 10;
    private int currentMusicLevel = 10;   // เพิ่มบรรทัดนี้
    private int currentAmbientLevel = 10; // เพิ่มบรรทัดนี้

    private void Start()
    {
        // 1. ดึงข้อมูล Resolution ที่หน้าจอผู้เล่นรองรับ
        resolutions = Screen.resolutions;

        // 2. โหลดค่า Setting ที่เคยเซฟไว้ (ถ้ามี)
        LoadSettings();

        // 3. อัปเดตตัวหนังสือบนหน้าจอ
        UpdateUITexts();
    }

    // ==========================================
    // ฟังก์ชันอัปเดตตัวหนังสือ
    // ==========================================
    private void UpdateUITexts()
    {
        resolutionText.text = resolutions[currentResIndex].width + " x " + resolutions[currentResIndex].height;
        vsyncText.text = vsyncOptions[currentVsyncIndex];
        fpsText.text = fpsOptions[currentFpsIndex] == -1 ? "Unlimited" : fpsOptions[currentFpsIndex].ToString();
        // อัปเดตตัวเลข % ของเสียงทั้ง 4 หมวด
        masterText.text = (currentMasterLevel * 10) + "%";
        sfxText.text = (currentSfxLevel * 10) + "%";
        musicText.text = (currentMusicLevel * 10) + "%";
        ambientText.text = (currentAmbientLevel * 10) + "%";

    }

    // ==========================================
    // ฟังก์ชันปรับค่า Resolution
    // ==========================================
    public void NextResolution() { currentResIndex = (currentResIndex + 1) % resolutions.Length; UpdateUITexts(); }
    public void PrevResolution() { currentResIndex = (currentResIndex - 1 + resolutions.Length) % resolutions.Length; UpdateUITexts(); }

    // ==========================================
    // ฟังก์ชันปรับค่า VSync
    // ==========================================
    public void NextVsync() { currentVsyncIndex = (currentVsyncIndex + 1) % vsyncOptions.Length; UpdateUITexts(); }
    public void PrevVsync() { currentVsyncIndex = (currentVsyncIndex - 1 + vsyncOptions.Length) % vsyncOptions.Length; UpdateUITexts(); }

    // ==========================================
    // ฟังก์ชันปรับค่า FPS
    // ==========================================
    public void NextFPS() { currentFpsIndex = (currentFpsIndex + 1) % fpsOptions.Length; UpdateUITexts(); }
    public void PrevFPS() { currentFpsIndex = (currentFpsIndex - 1 + fpsOptions.Length) % fpsOptions.Length; UpdateUITexts(); }

    // ==========================================
    // หมวดปรับเสียง (เพิ่ม Music และ Ambient)
    // ==========================================
    public void NextMaster() { if (currentMasterLevel < 10) currentMasterLevel++; UpdateUITexts(); ApplyVolumeRealtime(); }
    public void PrevMaster() { if (currentMasterLevel > 0) currentMasterLevel--; UpdateUITexts(); ApplyVolumeRealtime(); }

    public void NextSFX() { if (currentSfxLevel < 10) currentSfxLevel++; UpdateUITexts(); ApplyVolumeRealtime(); }
    public void PrevSFX() { if (currentSfxLevel > 0) currentSfxLevel--; UpdateUITexts(); ApplyVolumeRealtime(); }

    public void NextMusic() { if (currentMusicLevel < 10) currentMusicLevel++; UpdateUITexts(); ApplyVolumeRealtime(); }
    public void PrevMusic() { if (currentMusicLevel > 0) currentMusicLevel--; UpdateUITexts(); ApplyVolumeRealtime(); }

    public void NextAmbient() { if (currentAmbientLevel < 10) currentAmbientLevel++; UpdateUITexts(); ApplyVolumeRealtime(); }
    public void PrevAmbient() { if (currentAmbientLevel > 0) currentAmbientLevel--; UpdateUITexts(); ApplyVolumeRealtime(); }

    private void ApplyVolumeRealtime()
    {
        if (audioSettings != null)
        {
            float masterVol = currentMasterLevel == 0 ? 0.0001f : (float)currentMasterLevel / 10f;
            float sfxVol = currentSfxLevel == 0 ? 0.0001f : (float)currentSfxLevel / 10f;
            float musicVol = currentMusicLevel == 0 ? 0.0001f : (float)currentMusicLevel / 10f;
            float ambientVol = currentAmbientLevel == 0 ? 0.0001f : (float)currentAmbientLevel / 10f;

            audioSettings.SetMasterVolume(masterVol);
            audioSettings.SetSFXVolume(sfxVol);
            audioSettings.SetMusicVolume(musicVol);
            audioSettings.SetAmbientVolume(ambientVol);
        }
    }

    // ==========================================
    // ปุ่ม Apply Change (ยืนยันการตั้งค่ากราฟิกและบันทึก)
    // ==========================================
    public void ApplyChanges()
    {
        // ตั้งค่า Resolution
        Resolution res = resolutions[currentResIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // ตั้งค่า VSync
        QualitySettings.vSyncCount = currentVsyncIndex;

        // ตั้งค่า FPS
        Application.targetFrameRate = fpsOptions[currentFpsIndex];

        // เซฟค่าทั้งหมด
        PlayerPrefs.SetInt("ResIndex", currentResIndex);
        PlayerPrefs.SetInt("VsyncIndex", currentVsyncIndex);
        PlayerPrefs.SetInt("FpsIndex", currentFpsIndex);
        PlayerPrefs.SetInt("MasterLevel", currentMasterLevel);
        PlayerPrefs.SetInt("SfxLevel", currentSfxLevel);
        PlayerPrefs.SetInt("MusicLevel", currentMusicLevel);
        PlayerPrefs.SetInt("AmbientLevel", currentAmbientLevel);
        PlayerPrefs.Save();

        //Debug.Log("Settings Applied and Saved!");
    }

    private void LoadSettings()
    {
        currentResIndex = PlayerPrefs.GetInt("ResIndex", resolutions.Length - 1);
        currentVsyncIndex = PlayerPrefs.GetInt("VsyncIndex", 0);
        currentFpsIndex = PlayerPrefs.GetInt("FpsIndex", 1);
        currentMasterLevel = PlayerPrefs.GetInt("MasterLevel", 10);
        currentSfxLevel = PlayerPrefs.GetInt("SfxLevel", 10);
        currentMusicLevel = PlayerPrefs.GetInt("MusicLevel", 10);     // โหลดค่า
        currentAmbientLevel = PlayerPrefs.GetInt("AmbientLevel", 10); // โหลดค่า
    }
}