using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CubeDashRectangle : NetworkBehaviour
{
    public float dashLength = 5f;
    public float dashWidth = 1f;
    public float dashSpeed = 15f;
    public float moveSpeed = 5f;
    public LineRenderer lineRenderer;

    private Vector3 moveTarget;
    private Vector3 dashTarget;
    private bool moving = false;
    private bool dashPreview = false;
    private bool dashing = false;
    private bool dashCanceled = false;

    // Rotation lissée
    private Quaternion targetRotation;

    public GameObject playerUICanvas; // Drag & drop l’enfant UI (Canvas) ici

    public override void OnStartLocalPlayer()
    {
        // N'activer l'UI que pour le joueur local
        if (playerUICanvas != null)
            playerUICanvas.SetActive(true);
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            // Instancie une caméra que pour le joueur local
            GameObject camObj = new GameObject("LocalPlayerCamera");
            Camera cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera"; // pour Camera.main
            camObj.transform.position = transform.position + new Vector3(5, 10, -5); // Positionne l'offset

            // Ajoute et configure le script FollowedByCamera sur ton joueur
            FollowedByCamera camFollow = gameObject.AddComponent<FollowedByCamera>();
            camFollow.cameraToFollow = cam;
            camFollow.offset = new Vector3(-0.5f, 8, -8);
            camFollow.smoothSpeed = 0.125f;
            camObj.transform.rotation = Quaternion.Euler(45, 0, 0); // 45° vers le bas, Yaw 0, Roll 0

            // Désactive les autres caméras éventuelles
            foreach (Camera c in Camera.allCameras)
            {
                if (c != cam)
                    c.enabled = false;
            }
        }
    }


    void Update()
    {
        if (isLocalPlayer){
            // BLOQUE toutes les actions si on est en pause
            if (PauseMenuUI.isPaused)
                return;
            // --- GESTION DU DASH
            // Commencer la prévisualisation du dash tant qu'on maintient Espace
            if (!dashing && Keyboard.current.spaceKey.isPressed && !dashPreview && !dashCanceled)
            {
                dashPreview = true;
                if (lineRenderer != null) lineRenderer.enabled = true;
                UpdateDashRectangle();
            }

            // Actualiser la preview si elle est active
            if (dashPreview && !dashing && !dashCanceled)
            {
                UpdateDashRectangle();

                // Si on fait clic droit pendant espace : annule dash
                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    dashPreview = false;
                    dashCanceled = true;
                    if (lineRenderer != null) lineRenderer.enabled = false;
                }
            }

            // Quand on RELACHE la barre espace (et qu’on était en preview et pas annulé) -> dash !
            if (dashPreview && !dashing && !Keyboard.current.spaceKey.isPressed && !dashCanceled)
            {
                dashPreview = false;
                dashing = true;
                if (lineRenderer != null) lineRenderer.enabled = false;
                moving = false; // on annule tout mouvement normal

                // Oriente le cube dans la direction du dash
                Vector3 dashDir = (dashTarget - transform.position);
                dashDir.y = 0;
                if (dashDir.sqrMagnitude > 0.001f)
                    targetRotation = Quaternion.LookRotation(dashDir);
            }

            // Si on relâche espace et que dash était annulé, on réinitialise
            if (dashCanceled && !Keyboard.current.spaceKey.isPressed)
            {
                dashCanceled = false; // reset pour la prochaine preview
            }

            // Dash effectif
            if (dashing)
            {
                transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

                // Oriente le cube pendant le dash
                Vector3 dashDir = (dashTarget - transform.position);
                dashDir.y = 0;
                if (dashDir.sqrMagnitude > 0.001f)
                    targetRotation = Quaternion.LookRotation(dashDir);

                if (Vector3.Distance(transform.position, dashTarget) < 0.01f)
                {
                    dashing = false;
                    transform.position = dashTarget;
                    moveTarget = transform.position; // évite de repartir sur un move normal après
                }
            }

            // --- DEPLACEMENT NORMAL (clic droit) UNIQUEMENT SI PAS EN PREVIEW DASH
            if (!dashPreview && !dashing)
            {
                if (Mouse.current.rightButton.isPressed)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.CompareTag("Floor"))
                        {
                            moveTarget = new Vector3(hit.point.x, 0.5f, hit.point.z);
                            moving = true;

                            // Oriente le cube vers la cible
                            Vector3 lookDir = moveTarget - transform.position;
                            lookDir.y = 0;
                            if (lookDir.sqrMagnitude > 0.001f)
                                targetRotation = Quaternion.LookRotation(lookDir);
                        }
                    }
                }
                else if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.CompareTag("Floor"))
                        {
                            moveTarget = new Vector3(hit.point.x, 0.5f, hit.point.z);
                            moving = true;

                            // Oriente le cube vers la cible
                            Vector3 lookDir = moveTarget - transform.position;
                            lookDir.y = 0;
                            if (lookDir.sqrMagnitude > 0.001f)
                                targetRotation = Quaternion.LookRotation(lookDir);
                        }
                    }
                }
            }

            // --- MOUVEMENT NORMAL
            if (moving && !dashing)
            {
                transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);

                // Oriente le cube pendant le mouvement normal
                Vector3 moveDir = moveTarget - transform.position;
                moveDir.y = 0;
                if (moveDir.sqrMagnitude > 0.001f)
                    targetRotation = Quaternion.LookRotation(moveDir);

                if (!Mouse.current.rightButton.isPressed && Vector3.Distance(transform.position, moveTarget) < 0.01f)
                {
                    moving = false;
                }
            }

            // --- ROTATION LISSEE (tout le temps)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    void UpdateDashRectangle()
    {
        if(isLocalPlayer){
            // Direction vers la souris
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            Vector3 dashDir = transform.forward;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 target = hit.point;
                dashDir = (target - transform.position);
                dashDir.y = 0;
                if (dashDir.magnitude > 0.1f) dashDir.Normalize();
                else dashDir = transform.forward;
            }

            // Calcul point d'arrivée
            dashTarget = transform.position + dashDir * dashLength;
            dashTarget.y = 0.5f;

            // Rectangle visuel au sol
            Vector3 center = transform.position + dashDir * (dashLength / 2f);
            Vector3 up = Vector3.up * 0.02f;
            Vector3 perp = Vector3.Cross(Vector3.up, dashDir).normalized * (dashWidth / 2f);

            Vector3 p1 = center - dashDir * (dashLength / 2f) - perp;
            Vector3 p2 = center - dashDir * (dashLength / 2f) + perp;
            Vector3 p3 = center + dashDir * (dashLength / 2f) + perp;
            Vector3 p4 = center + dashDir * (dashLength / 2f) - perp;

            p1.y = p2.y = p3.y = p4.y = 0.51f;

            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, p1 + up);
                lineRenderer.SetPosition(1, p2 + up);
                lineRenderer.SetPosition(2, p3 + up);
                lineRenderer.SetPosition(3, p4 + up);
                lineRenderer.SetPosition(4, p1 + up);
            }

            // La cible du dash est le bord avant du rectangle
            dashTarget = (p3 + p4) / 2f;
            dashTarget.y = 0.5f;
        }
    }
}
