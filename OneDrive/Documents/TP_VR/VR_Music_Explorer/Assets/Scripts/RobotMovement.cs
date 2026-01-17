using UnityEngine;
using UnityEngine.InputSystem;

public class RobotMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Configuration VR")]
    public Transform cameraTransform; 
    public InputActionProperty moveAction; 

    [Header("Réglages Rotation (Anti-Tournis)")]
    public float rotationSmoothing = 5f; 
    public float rotationThreshold = 15f; 

    private Vector3 velocity;

    void Update()
    {
        // ROTATION (Suivi de la tête)
        if (cameraTransform != null)
        {
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, cameraTransform.eulerAngles.y));

            if (angleDifference > rotationThreshold)
            {
                Vector3 direction = cameraTransform.forward;
                direction.y = 0; 

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
                }
            }
        }

        //  MOUVEMENT 
        // Lit la valeur du joystick
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        
        // Test 
        if (input != Vector2.zero) Debug.Log("Joystick : " + input);

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * speed * Time.deltaTime);

        //  GRAVITÉ
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ANIMATIONS
        bool isMoving = (Mathf.Abs(input.y) > 0.1f || Mathf.Abs(input.x) > 0.1f);
        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isBacking", (input.y < -0.1f));
    }
}