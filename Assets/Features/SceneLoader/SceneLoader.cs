using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SceneLoader", order = 1)]
public class SceneLoader : ScriptableObject
{
    public static void LoadMainMenu()
    {
        AudioManager.instance.Play("ButtonPress");
        SceneManager.LoadScene("Menu");
    }

    public static void LoadLevel()
    {
        AudioManager.instance.Play("ButtonPress");
        AudioManager.instance.Stop("MenuOST");
        SceneManager.LoadScene("Level");
    }

    public static void LoadGameOver()
    {
        AudioManager.instance.Stop("GameOST");
        SceneManager.LoadScene("GameOver");
    }

    public static void LoadWin()
    {
        SceneManager.LoadScene("Win");
    }
}