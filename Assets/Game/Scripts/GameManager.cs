using System.Collections;
using System.Threading.Tasks;
using TarodevController;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    //[Header("Player")]
    //[SerializeField] private PlayerMovement player1;
    //[SerializeField] private PlayerMovement player2;

    [Header("Swap Settings")]
    [SerializeField] private SwapBox swapBox1;
    [SerializeField] private SwapBox swapBox2;

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
    [Header("Transition")]
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private float transitionTime;




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
        CheckReset();
        //ResetPlayer();
    }

    #region Scene Handling
    private void LevelTransition()
    {
        if (db) return;

        if (goal1.IsReached && goal2.IsReached)
        {
            db = true;
            Debug.Log("Both goals reached! Stage Win!");
            //Debug.Log("Current Scene Index: " + buildIndex);

            //Task.WaitAll(Task.Delay(500)); // Wait for 0.5 second before loading next scene  
            if (!IsGoNextLVL)
            {
                SaveStageReached(currentLVL);

                if (SceneName == "")
                {
                    Debug.LogError("SceneName is empty! Cannot load scene.");
                    //Quit();
                    return;
                }

                SceneManager.LoadScene(SceneName);
                return;
            }
            else
            {
                StartCoroutine(Playtransition());
            }
        }
    }

    private void LoadNextScene()
    {
        SaveStageReached(nextLVL);
        SceneManager.LoadScene(nextLVL);
    }

    private IEnumerator Playtransition()
    {
        fadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime); // Wait for the animation to finish
        LoadNextScene();
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
    #endregion
    private void SwapPlayer()
    {
        if (swapBox1 == null || swapBox2 == null) return;
        if (swapBox1.isReady && swapBox2.isReady /*&& !isSwapped*/)
        {
            //Debug.Log("Swapping Players!");
            //isSwapped = true;
            swapBox1.SetTarget(swapBox2.player);
            swapBox2.SetTarget(swapBox1.player);
            //Vector3 tempPosition = swapBox1.player.transform.position;
            swapBox1.SwapToPos(swapBox2.transform.position);
            swapBox2.SwapToPos(swapBox1.transform.position);
            //isSwapped = false; 
        }
    }

    private void CheckReset()
    {
        if (swapBox1 != null & swapBox2 != null)
        {
            if ((swapBox1.isReady || swapBox2.isReady) && (goal1.IsReached || goal2.IsReached))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } 
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
//    private void Quit()
//    {

//#if UNITY_EDITOR
//        EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif

//    }
}
