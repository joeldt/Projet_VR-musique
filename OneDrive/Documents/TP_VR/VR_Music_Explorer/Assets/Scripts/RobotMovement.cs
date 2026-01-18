using UnityEngine;
using UnityEngine.InputSystem;

public class RobotMovement : MonoBehaviour
{
    [Header("Composants de Base")]
    // Le CharacterController gère les collisions et le déplacement physique
    public CharacterController controller;
    // L'Animator contrôle les transitions entre Idle, Marche et Marche arrière.
    public Animator animator;

    [Header("Paramètres Physiques")]
    public float speed = 5f;          // Vitesse de déplacement au sol.
    public float gravity = -9.81f;    // Force de la pesanteur appliquée au robot.

    [Header("Configuration VR & Inputs")]
    // Référence à la caméra pour orienter le robot dans la direction du regard.
    public Transform cameraTransform;
    // Référence à l'action de mouvement (Joystick) définie dans l'Input Action Asset.
    public InputActionProperty moveAction;

    [Header("Réglages Rotation (Anti-Tournis)")]
    // Vitesse de rotation pour un suivi fluide de la tête.
    public float rotationSmoothing = 5f;
    // le robot ne tourne que si la tête pivote de plus de X degrés
    public float rotationThreshold = 15f;

    // Stocke la force verticale actuelle (pour la gravité et les sauts).
    private Vector3 velocity;


    void OnEnable()
    {
        // On active l'écoute de l'action de mouvement.
        if (moveAction.action != null)
            moveAction.action.Enable();
    }

    
    /// On désactive l'écoute pour éviter des erreurs de mémoire ou des mouvements fantômes.
    void OnDisable()
    {
        if (moveAction.action != null)
            moveAction.action.Disable();
    }

    void Update()
    {
        // On vérifie à chaque frame que l'action est active pour éviter le blocage.
        if (moveAction.action != null && !moveAction.action.enabled)
        {
            moveAction.action.Enable();
        }

        // GESTION DE LA ROTATION 
        if (cameraTransform != null)
        {
            // On calcule l'écart d'angle entre le corps du robot et la direction de la caméra.
            float angleDifference = Mathf.Abs(
                Mathf.DeltaAngle(transform.eulerAngles.y, cameraTransform.eulerAngles.y)
            );

            // Si l'écart dépasse le seuil, le robot pivote pour s'aligner sur le regard du joueur.
            if (angleDifference > rotationThreshold)
            {
                Vector3 direction = cameraTransform.forward;
                direction.y = 0; // On ignore l'axe vertical pour éviter que le robot ne penche.

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    // Slerp permet une rotation douce, indispensable pour éviter le mal de mer en VR.
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotationSmoothing * Time.deltaTime
                    );
                }
            }
        }

        // LECTURE DU JOYSTICK 
        // On récupère les valeurs X et Y (entre -1 et 1) envoyées par la manette.
        Vector2 input = Vector2.zero;
        if (moveAction.action != null)
        {
            input = moveAction.action.ReadValue<Vector2>();
        }

        // Log de Debug : pour vérifier dans le fichier "Player.log" du build si le joystick répond.
        Debug.Log("Joystick input : " + input);


        // On calcule la direction par rapport à l'orientation actuelle du robot .
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        // On applique le mouvement au CharacterController.
        controller.Move(move * speed * Time.deltaTime);

        // GESTION DE LA GRAVITÉ 
        if (controller.isGrounded)
        {
            // Si on touche le sol, on applique une petite force constante vers le bas.
            velocity.y = -2f; 
        }
        else
        {
            // Sinon, on accélère la chute au fil du temps.
            velocity.y += gravity * Time.deltaTime;
        }

        // Application finale de la gravité.
        controller.Move(velocity * Time.deltaTime);

        // SYNCHRONISATION DES ANIMATIONS 
        // On vérifie si le joystick est poussé au-delà d'un petit seuil.
        bool isMoving = Mathf.Abs(input.y) > 0.1f || Mathf.Abs(input.x) > 0.1f;
        
        // On met à jour les paramètres de l'Animator pour déclencher les transitions visuelles.
        animator.SetBool("isWalking", isMoving);
        // Si input.y est négatif, on active l'animation de recul.
        animator.SetBool("isBacking", input.y < -0.1f);
    }
}