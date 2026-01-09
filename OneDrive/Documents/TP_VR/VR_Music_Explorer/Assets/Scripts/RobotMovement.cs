using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public float gravity = -9.81f;

    private Vector3 velocity;

    void Update()
    {
        // Récupération des touches (Z,S,Q,D ou Flèches)
        float x = Input.GetAxis("Horizontal"); // Q et D
        float z = Input.GetAxis("Vertical");   // Z et S

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

        // Gestion des ANIMATIONS (Marche avant / Marche arrière / Idle)

        // Marche Avant : Si on pousse le joystick vers le haut (z > 0)
        bool isWalkingForward = (z > 0.1f);
        animator.SetBool("isWalking", isWalkingForward);

        // Marche Arrière : Si on tire le joystick vers le bas (z < -0.1)
        bool isBacking = (z < -0.1f);
        animator.SetBool("isBacking", isBacking);

        // Si on se déplace uniquement sur les côtés (Q ou D) sans avancer/reculer
        if (Mathf.Abs(x) > 0.1f && Mathf.Abs(z) < 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
    }
}