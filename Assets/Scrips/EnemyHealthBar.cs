using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Health healthTarget; // à assigner au Health du bot
    public Image fillImage;


    private void Update()
    {
        if (healthTarget != null && fillImage != null)
        {
            float ratio = Mathf.Clamp01((float)healthTarget.currentHealth / healthTarget.maxHealth);
            fillImage.fillAmount = ratio;
        }

        // Toujours orienter la barre face à la caméra
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }
}
