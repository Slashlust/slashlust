using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

public class MenuSceneScript : MonoBehaviour
{
  public void OnStartClick()
  {
    SceneManager.LoadScene(((int)GameScenes.mainScene), LoadSceneMode.Single);
  }
}
