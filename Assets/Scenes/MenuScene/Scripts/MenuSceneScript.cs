using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

public class MenuSceneScript : MonoBehaviour
{
  public void OnCreditsClick()
  {
    SceneManager.LoadScene(((int)GameScenes.creditsScene), LoadSceneMode.Single);
  }

  public void OnStartClick()
  {
    SceneManager.LoadScene(((int)GameScenes.mainScene), LoadSceneMode.Single);
  }
}
