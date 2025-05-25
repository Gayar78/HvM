using UnityEngine;

public class BotAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackRate = 1f; // 1 attaque par seconde
    private float nextAttackTime = 0f;
    public LayerMask playerLayer; // À configurer dans l’inspector pour viser le(s) joueur(s)

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // Détecte tous les joueurs dans la zone d'attaque
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
            foreach (var hit in hits)
            {
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null && playerHealth.isPlayer)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
            if (hits.Length > 0)
            {
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }
}
