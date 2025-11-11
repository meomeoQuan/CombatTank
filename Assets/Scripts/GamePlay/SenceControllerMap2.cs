using UnityEngine;
using UnityEngine.SceneManagement;

public class SenceControllerMap2 : MonoBehaviour
{
   public static SenceControllerMap2 instance;
    
  // GLOBAL PLAYER DATA!
    public static int PlayerCurrentHP = 100;
    public static int PlayerMaxHP = 100;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        Debug.Log($"[SceneController] Loading next level. HP: {PlayerCurrentHP}/{PlayerMaxHP}");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    // SAVE HP
    public static void SavePlayerHP(int currentHP)
    {
        PlayerCurrentHP = currentHP;
        Debug.Log($"[SceneController] Saved HP: {PlayerCurrentHP}");
    }
}
