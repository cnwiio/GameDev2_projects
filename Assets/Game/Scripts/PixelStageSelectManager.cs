using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class PixelStageSelectManager : MonoBehaviour
{
    [Header("UI Object References")]
    public RectTransform highlightCorners;
    public RectTransform buttonGridRoot;
    //public TextMeshProUGUI titleText;

    [Header("Game Data")]
    [Tooltip("Set this to your maximum number of levels (e.g., 12)")]
    public int totalLevels = 12; // <-- ADDED: Easily control max levels from Inspector
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
            }
        }

        // --- NEW PLAYERPREFS LOGIC HERE ---

        // Grab the highest stage reached. If "CurrentStage" isn't saved yet, it defaults to 1.
        int highestUnlockedStage = PlayerPrefs.GetInt("CurrentStage", 1);

        allLevelsData = new List<LevelData>();
        for (int i = 1; i <= totalLevels; i++)
        {
            // The level is locked if its number is greater than the player's highest unlocked stage
            bool isLevelLocked = i > highestUnlockedStage;

            allLevelsData.Add(new LevelData { number = i, isLocked = isLevelLocked });
        }

        // ----------------------------------

        totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);
    }

    private void Start()
    {

        // BUG FIX 1: Ensure we start exactly on Page 0, Button 0
        currentPageIndex = 0;
        currentSelectedIndex = 0;

        // Force Canvas to update so the layout group arranges the buttons before we move the cursor
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

    private void HandleGridNavigation()
    {
        int col = currentSelectedIndex % gridCols;
        int row = currentSelectedIndex / gridCols;

        int proposedRow = row;
        int proposedCol = col;

        // Up/Down logic remains the same (wraps vertically within the same column)
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
        // Left/Right now handles wrapping to previous/next rows
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (proposedCol > 0)
            {
                proposedCol--; // Move left normally
            }
            else
            {
                // Wrap to the last column
                proposedCol = gridCols - 1;

                // Jump up one row (wrap to bottom row if currently at the top row)
                if (proposedRow > 0) proposedRow--;
                else proposedRow = gridRows - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (proposedCol < gridCols - 1)
            {
                proposedCol++; // Move right normally
            }
            else
            {
                // Wrap to the first column
                proposedCol = 0;

                // Jump down one row (wrap to top row if currently at the bottom row)
                if (proposedRow < gridRows - 1) proposedRow++;
                else proposedRow = 0;
            }
        }

        int newSelectedIndex = (proposedRow * gridCols) + proposedCol;

        // Check if the new index actually contains a level before moving
        if (newSelectedIndex != currentSelectedIndex && IsValidButtonIndex(newSelectedIndex))
        {
            currentSelectedIndex = newSelectedIndex;
            UpdateUIFeedback();
        }
    }

    // Helper method to verify a slot has an active level
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
            currentSelectedIndex = 0; // Always reset to the first button on page flip
            UpdateLevelNumbersOnButtons();
            UpdateUIFeedback();
        }
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
            Debug.Log($"Loading level: {selectedLevel.number}");
            SceneManager.LoadScene(levelScenePrefix + selectedLevel.number);
        }
        else
        {
            Debug.Log("Cannot load level: Level is locked!");
        }
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
                // Deactivate buttons that exceed total levels (e.g., slot 13)
                levelButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateUIFeedback()
    {
        // BUG FIX 1 (Part 2): Use transform.position (world space) instead of anchoredPosition
        // This prevents the cursor from misaligning if the layout group shifts.
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
    }

    private LevelData GetSelectedLevelData()
    {
        int globalIndex = (currentPageIndex * levelsPerPage) + currentSelectedIndex;
        if (globalIndex >= 0 && globalIndex < allLevelsData.Count) return allLevelsData[globalIndex];
        return null;
    }
}