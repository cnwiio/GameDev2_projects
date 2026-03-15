using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems; // <-- สำคัญมาก! ต้องมีตัวนี้สำหรับทำ Hover

public class PixelStageSelectManager : MonoBehaviour
{
    [Header("UI Object References")]
    public RectTransform highlightCorners;
    public RectTransform buttonGridRoot;

    [Header("Game Data")]
    [Tooltip("Set this to your maximum number of levels (e.g., 12)")]
    public int totalLevels = 12;
    public string levelScenePrefix = "Level_";

    [Header("Input Keys")]
    public KeyCode pageLeftKey = KeyCode.Q;
    public KeyCode pageRightKey = KeyCode.E;
    public KeyCode selectKey = KeyCode.Return;

    [Header("Grid Setup")]
    private const int levelsPerPage = 10;
    private const int gridRows = 2;
    private const int gridCols = 5;

    [Header("Colors")]
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color normalColor = Color.white;

    private int currentSelectedIndex = 0;
    private int currentPageIndex = 0;
    private int totalPages;

    [System.Serializable]
    public class LevelData
    {
        public int number;
        public bool isLocked;
    }

    private List<LevelData> allLevelsData;
    private Button[] levelButtons;
    private TextMeshProUGUI[] buttonTexts;

    private void Awake()
    {
        levelButtons = new Button[levelsPerPage];
        buttonTexts = new TextMeshProUGUI[levelsPerPage];

        for (int i = 0; i < levelsPerPage; i++)
        {
            if (i < buttonGridRoot.childCount)
            {
                levelButtons[i] = buttonGridRoot.GetChild(i).GetComponent<Button>();
                buttonTexts[i] = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                // --- เพิ่มระบบ MOUSE (CLICK & HOVER) ---
                int buttonIndex = i; // ต้องเก็บค่า i ไว้ในตัวแปร local เพื่อให้ Event ดึงไปใช้ได้ถูกต้อง

                // 1. ระบบ Click
                levelButtons[i].onClick.AddListener(() => OnButtonClicked(buttonIndex));

                // 2. ระบบ Hover (Pointer Enter)
                // เช็คก่อนว่ามี EventTrigger คอมโพเนนต์อยู่ไหม ถ้าไม่มีให้ใส่เพิ่มเข้าไป
                EventTrigger trigger = levelButtons[i].gameObject.GetComponent<EventTrigger>();
                if (trigger == null) trigger = levelButtons[i].gameObject.AddComponent<EventTrigger>();

                // สร้าง Event เมื่อเมาส์ลากเข้ามาในปุ่ม
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { OnButtonHovered(buttonIndex); });
                trigger.triggers.Add(entry);
                // ------------------------------------
            }
        }

        int highestUnlockedStage = PlayerPrefs.GetInt("CurrentStage", 1);

        allLevelsData = new List<LevelData>();
        for (int i = 1; i <= totalLevels; i++)
        {
            bool isLevelLocked = i > highestUnlockedStage;
            allLevelsData.Add(new LevelData { number = i, isLocked = isLevelLocked });
        }

        totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);
    }

    private void Start()
    {
        currentPageIndex = 0;
        currentSelectedIndex = 0;

        Canvas.ForceUpdateCanvases();

        UpdateLevelNumbersOnButtons();
        UpdateUIFeedback();
    }

    private void Update()
    {
        HandleGridNavigation();
        HandlePageNavigation();
        HandleSelection();
    }

    // --- MOUSE EVENT METHODS ---

    private void OnButtonHovered(int index)
    {
        // เมื่อเมาส์ลากเข้ามาทับปุ่ม ให้เช็คก่อนว่าปุ่มนั้นมีด่านอยู่จริงไหม
        if (IsValidButtonIndex(index))
        {
            currentSelectedIndex = index;
            UpdateUIFeedback(); // ย้ายไฮไลท์ไปที่ปุ่มนั้น
        }
    }

    private void OnButtonClicked(int index)
    {
        // เมื่อคลิกปุ่ม ให้ย้ายไฮไลท์ไปที่นั่น (เผื่อไว้) แล้วกดยืนยันเข้าด่านเลย
        if (IsValidButtonIndex(index))
        {
            currentSelectedIndex = index;
            UpdateUIFeedback();
            ConfirmSelection();
        }
    }

    // ---------------------------

    private void HandleGridNavigation()
    {
        int col = currentSelectedIndex % gridCols;
        int row = currentSelectedIndex / gridCols;

        int proposedRow = row;
        int proposedCol = col;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (proposedRow > 0) proposedRow--;
            else proposedRow = gridRows - 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (proposedRow < gridRows - 1) proposedRow++;
            else proposedRow = 0;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (proposedCol > 0)
            {
                proposedCol--;
            }
            else
            {
                proposedCol = gridCols - 1;
                if (proposedRow > 0) proposedRow--;
                else proposedRow = gridRows - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (proposedCol < gridCols - 1)
            {
                proposedCol++;
            }
            else
            {
                proposedCol = 0;
                if (proposedRow < gridRows - 1) proposedRow++;
                else proposedRow = 0;
            }
        }

        int newSelectedIndex = (proposedRow * gridCols) + proposedCol;

        if (newSelectedIndex != currentSelectedIndex && IsValidButtonIndex(newSelectedIndex))
        {
            currentSelectedIndex = newSelectedIndex;
            UpdateUIFeedback();
        }
    }

    private bool IsValidButtonIndex(int indexOnPage)
    {
        int globalIndex = (currentPageIndex * levelsPerPage) + indexOnPage;
        return globalIndex < totalLevels && indexOnPage >= 0 && indexOnPage < levelsPerPage;
    }

    private void HandlePageNavigation()
    {
        if (Input.GetKeyDown(pageLeftKey)) ChangePage(-1);
        else if (Input.GetKeyDown(pageRightKey)) ChangePage(1);
    }

    private void ChangePage(int delta)
    {
        int newPage = currentPageIndex + delta;
        if (newPage < 0) newPage = totalPages - 1;
        else if (newPage >= totalPages) newPage = 0;

        if (newPage != currentPageIndex)
        {
            currentPageIndex = newPage;
            currentSelectedIndex = 0;
            UpdateLevelNumbersOnButtons();
            UpdateUIFeedback();
        }

        SoundManager.Instance.PlaySFX("Switch");
    }

    private void HandleSelection()
    {
        if (Input.GetKeyDown(selectKey)) ConfirmSelection();
    }

    private void ConfirmSelection()
    {
        LevelData selectedLevel = GetSelectedLevelData();
        if (selectedLevel == null) return;

        if (!selectedLevel.isLocked)
        {
            //Debug.Log($"Loading level: {selectedLevel.number}");
            SoundManager.Instance.PlaySFX("Click");
            PlayerPrefs.SetInt("EnterStage", 1);
            SceneManager.LoadScene(levelScenePrefix + selectedLevel.number);
        }
        //else
        //{
        //    //Debug.Log("Cannot load level: Level is locked!");
        //}
    }

    private void UpdateLevelNumbersOnButtons()
    {
        int startIndex = currentPageIndex * levelsPerPage;

        for (int i = 0; i < levelsPerPage; i++)
        {
            int globalLevelIndex = startIndex + i;
            if (globalLevelIndex < totalLevels)
            {
                levelButtons[i].gameObject.SetActive(true);
                if (buttonTexts[i] != null) buttonTexts[i].text = allLevelsData[globalLevelIndex].number.ToString();
            }
            else
            {
                levelButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateUIFeedback()
    {
        Button currentButton = levelButtons[currentSelectedIndex];
        highlightCorners.position = currentButton.transform.position;

        int startIndex = currentPageIndex * levelsPerPage;
        for (int i = 0; i < levelsPerPage; i++)
        {
            int globalIndex = startIndex + i;
            if (globalIndex < totalLevels)
            {
                bool isLocked = allLevelsData[globalIndex].isLocked;
                if (buttonTexts[i] != null) buttonTexts[i].color = isLocked ? lockedColor : normalColor;
            }
        }

        SoundManager.Instance.PlaySFX("Hover");
    }

    private LevelData GetSelectedLevelData()
    {
        int globalIndex = (currentPageIndex * levelsPerPage) + currentSelectedIndex;
        if (globalIndex >= 0 && globalIndex < allLevelsData.Count) return allLevelsData[globalIndex];
        return null;
    }
}