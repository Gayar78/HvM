using UnityEngine;
using UnityEngine.InputSystem; // pour le Input System
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuPanel; // à glisser dans l'inspector

    void Update()
{
    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
        if (pauseMenuPanel != null)
        {
            bool newState = !pauseMenuPanel.activeSelf;
            pauseMenuPanel.SetActive(newState);
            isPaused = newState; // ← Met à jour le flag
        }
    }
}
public void ResumeGame()
{
    if (pauseMenuPanel != null)
        pauseMenuPanel.SetActive(false);
    isPaused = false;
}
public void QuitGame()
{
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
}}
