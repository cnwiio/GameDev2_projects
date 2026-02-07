using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public Goal goal1;
    public Goal goal2;
    public bool IsGoNextLVL;
    public string SceneName;
    private bool db = false;
    private byte currentLVL, nextLVL;

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

    private void Quit()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
