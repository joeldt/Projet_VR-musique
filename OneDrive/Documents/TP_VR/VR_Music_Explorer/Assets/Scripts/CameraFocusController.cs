using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    public Transform positionNormale;    // Doit être un point attaché au Robot
    public Transform positionHologramme; // Le point devant l'hologramme
    public float vitesseTransition = 5f;

    [SerializeField] private bool focusOnHolo = false;

    void LateUpdate() // On utilise LateUpdate pour éviter les saccades
    {
        // Choisir la cible
        Transform cible = focusOnHolo ? positionHologramme : positionNormale;

        if (cible != null)
        {
            // Déplacement fluide vers la cible
            transform.position = Vector3.Lerp(transform.position, cible.position, Time.deltaTime * vitesseTransition);
            transform.rotation = Quaternion.Slerp(transform.rotation, cible.rotation, Time.deltaTime * vitesseTransition);
        }
    }

    public void ActiverFocus(bool etat)
    {
        focusOnHolo = etat;
        Debug.Log("Focus Camera : " + etat); // Vérifie dans la console si ça s'affiche quand tu appuies sur Y
    }
}