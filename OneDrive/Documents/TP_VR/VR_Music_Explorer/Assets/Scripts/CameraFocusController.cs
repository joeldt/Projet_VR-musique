using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    [Header("Réglages Focus VR")]
    public Transform cameraOffset;       // Camera Offset du XR Origin
    public Transform positionNormale;    // Point de repos du Camera Offset
    public Transform positionHologramme; // Le point envoyé par la porte
    public float vitesseTransition = 5f;

    [SerializeField] private bool focusOnHolo = false;

    void LateUpdate()
    {
        // On choisit la cible 
        Transform cible = focusOnHolo ? positionHologramme : positionNormale;

        if (cible != null && cameraOffset != null)
        {
            // On déplace Camera Offset pour que la caméra suive
            cameraOffset.position = Vector3.Lerp(cameraOffset.position, cible.position, Time.deltaTime * vitesseTransition);
            cameraOffset.rotation = Quaternion.Slerp(cameraOffset.rotation, cible.rotation, Time.deltaTime * vitesseTransition);
        }
    }

    public void ActiverFocus(bool etat)
    {
        focusOnHolo = etat;
        Debug.Log("Focus Camera VR : " + etat);
    }
}