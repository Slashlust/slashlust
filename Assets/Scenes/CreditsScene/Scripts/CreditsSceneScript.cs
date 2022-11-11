using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

public class CreditsSceneScript : MonoBehaviour
{
  public void OnBackClick()
  {
    SceneManager.LoadScene(((int)GameScenes.menuScene), LoadSceneMode.Single);
  }
}
