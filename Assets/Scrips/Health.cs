using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Health : NetworkBehaviour 
{
    public int maxHealth = 100;
    [SyncVar] public int currentHealth;
    public bool isPlayer = false; // Différencie le joueur du bot

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Appelée pour prendre des dégâts
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isPlayer)
        {
            // Quitter la partie ou charger une scène de "Game Over"
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            // Si c'est un bot, le fait disparaître
            Destroy(gameObject);
        }
    }
}
