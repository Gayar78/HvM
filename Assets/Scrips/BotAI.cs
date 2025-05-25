using UnityEngine;

public class BotAI : MonoBehaviour
{
    public float botSpeed = 3f;           // Moins rapide que le joueur
    public float stopDistance = 1.0f;     // Distance minimale pour "coller" sans traverser
    public float detectionRange = 30f;    // Distance à laquelle il "voit" les joueurs
    public LayerMask playerLayer;         // À assigner dans l'inspector

    private Transform targetPlayer;       // Joueur poursuivi

    private void Update()
    {
        // Cherche tous les joueurs dans la range
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);

        // Trouver le plus proche
        float closestDist = Mathf.Infinity;
        targetPlayer = null;

        foreach (var col in playersInRange)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                targetPlayer = col.transform;
            }
        }

        // Si pas de joueur, ne fait rien
        if (targetPlayer == null) return;

        // Direction vers le joueur ciblé
        Vector3 direction = targetPlayer.position - transform.position;
        direction.y = 0; // Reste sur le sol

        // Si on n'est pas déjà collé
        if (direction.magnitude > stopDistance)
        {
            Vector3 move = direction.normalized * botSpeed * Time.deltaTime;
            if (move.magnitude > direction.magnitude - stopDistance)
                move = direction.normalized * (direction.magnitude - stopDistance);

            transform.position += move;

            // Rotation Y uniquement (reste à plat)
            if (move.sqrMagnitude > 0.001f)
            {
                float targetY = Quaternion.LookRotation(direction).eulerAngles.y;
                Quaternion lookRot = Quaternion.Euler(0, targetY, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
            }
        }
    }

    // (Optionnel) Visualise la range de détection dans la scène
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
