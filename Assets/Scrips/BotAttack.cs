using UnityEngine;

public class BotAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackRate = 1f;
    private float nextAttackTime = 0f;

    private BotAI botAI;

    void Start()
    {
        botAI = GetComponent<BotAI>();
    }

    private void Update()
    {
        if (botAI == null || botAI.TargetPlayer == null)
            return;

        Transform target = botAI.TargetPlayer;
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= attackRange && Time.time >= nextAttackTime)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null && playerHealth.isPlayer)
            {
                playerHealth.TakeDamage(attackDamage);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }
}
