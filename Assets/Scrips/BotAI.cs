using UnityEngine;
using Mirror;

public class BotAI : NetworkBehaviour
{
    public float botSpeed = 3f;
    public float stopDistance = 1.0f;
    public float detectionRange = 30f;
    public LayerMask playerLayer;
    private Transform targetPlayer;
    public Transform TargetPlayer => targetPlayer;


    private void Update()
    {
        if (!isServer) return;

        Collider[] playersInRange = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);

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

        if (targetPlayer == null) return;

        Vector3 direction = targetPlayer.position - transform.position;
        direction.y = 0; // Important : rester à plat

        if (direction.magnitude > stopDistance)
        {
            Vector3 move = direction.normalized * botSpeed * Time.deltaTime;
            if (move.magnitude > direction.magnitude - stopDistance)
                move = direction.normalized * (direction.magnitude - stopDistance);

            Vector3 newPos = transform.position + move;
            newPos.y = 0.5f;
            transform.position = newPos;

            // ---- Correction : rotation sur Y uniquement (jamais X/Z)
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                float targetY = lookRot.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0, targetY, 0); // << seulement Y !
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
