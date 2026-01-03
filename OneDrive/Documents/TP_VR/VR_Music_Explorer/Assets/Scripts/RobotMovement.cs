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
        // 1. Récupération des entrées clavier (Z, S, Q, D)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 2. Création du mouvement relatif à l'orientation du robot
        Vector3 move = transform.right * x + transform.forward * z;

        // 3. Application du mouvement
        controller.Move(move * speed * Time.deltaTime);

        // 4. Gestion de la gravité
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. Mise à jour de l'animation (isWalking)
        bool isWalking = (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f);
        animator.SetBool("isWalking", isWalking);
    }
}