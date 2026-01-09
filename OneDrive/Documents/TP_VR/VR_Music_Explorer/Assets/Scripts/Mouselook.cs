using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 400f;
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        // Bloque le curseur pour ne pas sortir de la fenêtre de jeu
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Récupération des mouvements de la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation verticale (Caméra)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotation horizontale (Corps du robot)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}