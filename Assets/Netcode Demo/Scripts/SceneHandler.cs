using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneHandler : MonoBehaviour
{

  private List<string> sceneHistory = new List<string>();  //running history of scenes
  public GameObject[] sceneArray;
  // supaya background button bisa dipencet
  float timeWait = 0.9f;
  bool isLoading = false;
  float maxRadius;

  public static SceneHandler Instance { get; private set; }


  private void Awake()
  {

    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
    }
    else
    {
      Instance = this;
    }

  }
  void Start()
  {

    sceneHistory.Add(SceneManager.GetActiveScene().name);
    DontDestroyOnLoad(this.gameObject);
   
  }

  IEnumerator transitionScene(int newSceneIdx)
  {

    isLoading = true;
    yield return new WaitForSeconds(timeWait);
    isLoading = false;
    SceneManager.LoadScene(newSceneIdx);
    // harus dibawah karena scene harus loaded dahulu untuk getScene
    sceneHistory.Add(SceneManager.GetSceneByBuildIndex(newSceneIdx).name);

  }

  IEnumerator transitionScene(string newScene)
  {
    sceneHistory.Add(newScene);
    isLoading = true;
    yield return new WaitForSeconds(timeWait);
    SceneManager.LoadScene(newScene);
    isLoading = false;
  }
  public void LoadScene(string newScene)
  {

    if (isLoading) return;
    StartCoroutine(transitionScene(newScene));
  }

  public void LoadScene(int newSceneIdx)
  {
    if (isLoading) return;
    StartCoroutine(transitionScene(newSceneIdx));
  }
  void Update()
  {
  }

  public bool PreviousScene()
  {
    bool returnValue = false;
    Debug.Log(sceneHistory.Count);
    if (sceneHistory.Count >= 1)  //Checking that we have actually switched scenes enough to go back to a previous scene
    {
      returnValue = true;
      string lastScene = sceneHistory[sceneHistory.Count - 1];
      sceneHistory.RemoveAt(sceneHistory.Count - 1);
      LoadScene(lastScene);
    }

    return returnValue;
  }

  public void loadLoseScene()
  {
    LoadScene("LoseScene");
  }
  public void loadWinScene()
  {
    LoadScene("WinScene");
  }

  public void restartButton()
  {
    PreviousScene();
  }

  public void restartFirstLevel()
  {
    LoadScene("MainScene");
  }

  public void restartMainMenu()
  {
    LoadScene("MainMenu");
  }

  public void QuitGame()
  {
    Debug.Log("Quit Game");
    Application.Quit();
  }

  public void loadNextLevel()
  {
    int nextSceneIdx = SceneManager.GetActiveScene().buildIndex + 1;
    LoadScene(nextSceneIdx);
  }

}
