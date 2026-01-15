using UnityEngine;
using UnityEngine.InputSystem; // Indispensable pour les joysticks VR

public class RobotMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Configuration Joysticks VR")]
    public InputActionProperty moveAction; // On lie le joystick ici

    private Vector3 velocity;

    void Update()
    {
        // On récupère la valeur du joystick (Vector2 : x = gauche/droite, y = avant/arrière)
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float x = input.x;
        float z = input.y;

        // Calcul du mouvement relatif à l'orientation du robot
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Gravité
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Gestion des ANIMATIONS
        bool isWalkingForward = (z > 0.1f);
        animator.SetBool("isWalking", isWalkingForward);

        bool isBacking = (z < -0.1f);
        animator.SetBool("isBacking", isBacking);

        // Animation si déplacement latéral pur
        if (Mathf.Abs(x) > 0.1f && Mathf.Abs(z) < 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
    }
}