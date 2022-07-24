using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SceneLoader", order = 1)]
public class SceneLoader : ScriptableObject
{
    public static void LoadMainMenu()
    {
        AudioManager.Instance.Play("ButtonPress");
        SceneManager.LoadScene("Menu");
    }

    public static void LoadLevel()
    {
        AudioManager.Instance.Play("ButtonPress");
        AudioManager.Instance.Stop("MenuOST");
        AudioManager.Instance.Play("GameOST");
        SceneManager.LoadScene("Level");
    }

    public static void LoadGameOver()
    {
        AudioManager.Instance.Stop("GameOST");
        SceneManager.LoadScene("GameOver");
    }

    public static void LoadWin()
    {
        SceneManager.LoadScene("Win");
    }
}