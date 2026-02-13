using System.Threading.Tasks;
using TarodevController;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [Header("Swap Settings")]
    [SerializeField] private SwapBox swapBox1;
    [SerializeField] private SwapBox swapBox2;
    private bool isSwapped = false;

    #region Goal
    [Header("Goal Settings")]
    [SerializeField] private Goal goal1;
    [SerializeField] private Goal goal2;
    [Header("Level Settings")]
    [SerializeField] private bool IsGoNextLVL;
    [SerializeField] private string SceneName;
    private bool db = false;
    private byte currentLVL, nextLVL;
    #endregion

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        currentLVL = (byte)currentScene.buildIndex;
        nextLVL = (byte)(currentScene.buildIndex + 1);
    }

    // Update is called once per frame
    void Update()
    {
        LevelTransition();
        SwapPlayer();
    }

    private void LevelTransition()
    {
        if (db) return;

        if (goal1.IsReached && goal2.IsReached)
        {
            db = true;
            Debug.Log("Both goals reached! Stage Win!");
            //Debug.Log("Current Scene Index: " + buildIndex);

            Task.WaitAll(Task.Delay(500)); // Wait for 0.5 second before loading next scene
            if (!IsGoNextLVL)
            {
                SaveStageReached(currentLVL);

                if (SceneName == "")
                {
                    Debug.LogError("SceneName is empty! Cannot load scene.");
                    Quit();
                    return;
                }

                SceneManager.LoadScene(SceneName);
                return;
            }
            else
            {
                SaveStageReached(nextLVL);
                SceneManager.LoadScene(nextLVL);
            }
        }
    }

    private void SaveStageReached(byte stage)
    {
        if (PlayerPrefs.HasKey("CurrentStage"))
        {
            if (stage > PlayerPrefs.GetInt("CurrentStage"))
            {
                PlayerPrefs.SetInt("CurrentStage", stage);
            }
        }
        else
        {
            PlayerPrefs.SetInt("CurrentStage", stage);
        }
    }

    private void SwapPlayer()
    {
        if (swapBox1 == null || swapBox2 == null) return;
        if (swapBox1.isReady && swapBox2.isReady && !isSwapped)
        {
            //Debug.Log("Swapping Players!");
            isSwapped = true;
            swapBox1.SetTarget(swapBox2.player);
            swapBox2.SetTarget(swapBox1.player);
            Vector3 tempPosition = swapBox1.player.transform.position;
            swapBox1.SwapToPos(swapBox2.player.transform.position);
            swapBox2.SwapToPos(tempPosition);
            isSwapped = false; 
        }
    }

    private void Quit()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
