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

    [Header("Custom Settings")]
    [Tooltip("ใส่ขนาดหน้าจอที่ต้องการให้ผู้เล่นเลือกได้ (เช่น X=1920, Y=1080)")]
    // สร้าง Array แบบกำหนดค่าเองได้ใน Inspector พร้อมค่าเริ่มต้น
    public Vector2Int[] customResolutions = new Vector2Int[]
    {
        new Vector2Int(1280, 720),   // HD
        new Vector2Int(1600, 900),   // HD+
        new Vector2Int(1920, 1080),  // Full HD
        new Vector2Int(2560, 1440)   // 2K
    };
    private int currentResIndex = 2; // ให้เริ่มต้นที่ Full HD (Index ที่ 2)

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
        if (audioSettings == null) audioSettings = FindAnyObjectByType<AudioSettings>();

        FilterSupportedResolutions();
        LoadSettings();
        UpdateUITexts();

        // เพิ่มบรรทัดนี้ เพื่อบังคับให้เกมปรับระดับเสียงตาม UI ทันทีตอนเริ่มเกม
        ApplyVolumeRealtime();
    }

    // ==========================================
    // ฟังก์ชันอัปเดตตัวหนังสือ
    // ==========================================
    private void UpdateUITexts()
    {
        resolutionText.text = customResolutions[currentResIndex].x + " x " + customResolutions[currentResIndex].y;
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
    public void NextResolution() { currentResIndex = (currentResIndex + 1) % customResolutions.Length; UpdateUITexts(); }
    public void PrevResolution() { currentResIndex = (currentResIndex - 1 + customResolutions.Length) % customResolutions.Length; UpdateUITexts(); }

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
        Vector2Int res = customResolutions[currentResIndex];
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        QualitySettings.vSyncCount = currentVsyncIndex;
        Application.targetFrameRate = fpsOptions[currentFpsIndex];

        // เปลี่ยนชื่อ Key เป็น ...LevelUI เพื่อไม่ให้ชนกับ AudioSettings
        PlayerPrefs.SetInt("ResIndex", currentResIndex);
        PlayerPrefs.SetInt("VsyncIndex", currentVsyncIndex);
        PlayerPrefs.SetInt("FpsIndex", currentFpsIndex);
        PlayerPrefs.SetInt("MasterLevelUI", currentMasterLevel);
        PlayerPrefs.SetInt("SFXLevelUI", currentSfxLevel);
        PlayerPrefs.SetInt("MusicLevelUI", currentMusicLevel);
        PlayerPrefs.SetInt("AmbientLevelUI", currentAmbientLevel);
        PlayerPrefs.Save();

        //Debug.Log("Settings Applied and Saved!");
    }

    private void LoadSettings()
    {
        int savedResIndex = PlayerPrefs.GetInt("ResIndex", 2);
        currentResIndex = Mathf.Clamp(savedResIndex, 0, customResolutions.Length - 1);
        currentVsyncIndex = PlayerPrefs.GetInt("VsyncIndex", 0);
        currentFpsIndex = PlayerPrefs.GetInt("FpsIndex", 1);

        // โหลดข้อมูลโดยใช้ชื่อ Key ใหม่
        currentMasterLevel = PlayerPrefs.GetInt("MasterLevelUI", 10);
        currentSfxLevel = PlayerPrefs.GetInt("SFXLevelUI", 10);
        currentMusicLevel = PlayerPrefs.GetInt("MusicLevelUI", 10);
        currentAmbientLevel = PlayerPrefs.GetInt("AmbientLevelUI", 10);
    }

    private void FilterSupportedResolutions()
    {
        // หาค่า Resolution สูงสุดที่หน้าจอของผู้เล่นรองรับได้ (มักจะอยู่ตัวสุดท้ายของ Array)
        Resolution maxMonitorRes = Screen.resolutions[Screen.resolutions.Length - 1];

        List<Vector2Int> validResolutions = new List<Vector2Int>();

        // วนลูปเช็ก customResolutions ที่เราตั้งค่าไว้ใน Inspector
        foreach (Vector2Int res in customResolutions)
        {
            // ถ้าความกว้างและความสูง น้อยกว่าหรือเท่ากับ จอของผู้เล่น ให้เก็บไว้
            if (res.x <= maxMonitorRes.width && res.y <= maxMonitorRes.height)
            {
                validResolutions.Add(res);
            }
        }

        // เผื่อกรณีฉุกเฉิน: ถ้าไม่มีจอไหนผ่านเงื่อนไขเลย ให้ยึดขนาดจอสูงสุดของผู้เล่นไปเลย
        if (validResolutions.Count == 0)
        {
            validResolutions.Add(new Vector2Int(maxMonitorRes.width, maxMonitorRes.height));
        }

        // นำค่าที่ผ่านการคัดกรองแล้ว ไปทับ Array ตัวเดิม
        customResolutions = validResolutions.ToArray();
    }
}