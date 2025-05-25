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

        // Billboard : toujours face caméra (sans inclinaison verticale)
        if (Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f; // Ignore le tilt vertical
            if (camForward.sqrMagnitude > 0.01f)
                transform.forward = camForward.normalized;
        }
    }
}
