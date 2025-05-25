using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f;           // Portée pour l’attaque de zone ou ciblée
    public int attackDamage = 20;
    public LayerMask enemyLayer;             // Mets ici le layer Enemy
    public float autoAttackRange = 1.5f;     // Portée pour auto-attaque
    public float autoAttackCooldown = 1f;    // Délai entre auto-attaques

    private float nextAutoAttackTime = 0f;

    void Update()
    {
        // --- Attaque par clic gauche (ciblée OU zone) ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Essaye d’attaquer un ennemi ciblé
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, enemyLayer))
            {
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth != null && !enemyHealth.isPlayer)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    return;
                }
            }

            // Sinon, attaque tous les ennemis proches dans la zone
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
            foreach (var h in hits)
            {
                Health enemyHealth = h.GetComponent<Health>();
                if (enemyHealth != null && !enemyHealth.isPlayer)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }

        // --- Auto-attaque sur ennemi proche (cooldown) ---
        if (Time.time >= nextAutoAttackTime)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, autoAttackRange, enemyLayer);
            foreach (var h in hits)
            {
                Health enemyHealth = h.GetComponent<Health>();
                if (enemyHealth != null && !enemyHealth.isPlayer)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    nextAutoAttackTime = Time.time + autoAttackCooldown;
                    break; // Attaque un seul ennemi à la fois en auto
                }
            }
        }
    }

    // (Optionnel) Visualise les portées d’attaque dans l’éditeur
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, autoAttackRange);
    }
}
