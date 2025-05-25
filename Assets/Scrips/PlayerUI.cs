using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image healthBarFill;    // À assigner dans l'inspector (HealthBarFill)
    public Health playerHealth;    // À assigner dans l'inspector (le script Health sur le joueur)

    private float initialWidth;    // Largeur de base

    void Start()
    {
        if (healthBarFill != null)
            initialWidth = healthBarFill.rectTransform.sizeDelta.x;
    }

    void Update()
    {
        if (healthBarFill != null && playerHealth != null)
        {
            float ratio = Mathf.Clamp01((float)playerHealth.currentHealth / playerHealth.maxHealth);
            healthBarFill.rectTransform.sizeDelta = new Vector2(initialWidth * ratio, healthBarFill.rectTransform.sizeDelta.y);
        }
    }
}
