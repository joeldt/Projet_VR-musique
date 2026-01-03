using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public float rotationSpeed = 100f;

    void Update()
    {
        // Récupérer les touches (Z,S,Q,D ou flèches)
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");

        // Rotation du robot
        transform.Rotate(0, moveHorizontal * rotationSpeed * Time.deltaTime, 0);

        // Déplacement avant/arrière
        Vector3 move = transform.forward * moveVertical;
        controller.Move(move * speed * Time.deltaTime);

        // Envoyer l'information à l'Animator pour lancer la marche
        bool isWalking = (moveVertical != 0);
        animator.SetBool("isWalking", isWalking);
    }
}
