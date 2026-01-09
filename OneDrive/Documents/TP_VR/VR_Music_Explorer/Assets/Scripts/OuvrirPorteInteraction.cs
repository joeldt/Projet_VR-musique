using UnityEngine;

public class OuvrirPorteInteraction : MonoBehaviour
{
    [Header("Configuration Porte")]
    public Animator porteAnimator;
    public GameObject monInterface;
    private bool isPlayerInZone = false;
    private bool isOpen = false;

    [Header("Configuration Hologramme")]
    public Animator holoAnimator; // L'Animator de l'objet Hologramme_Session

    [Header("Configuration Caméra & Robot")]
    public CameraFocusController camFocus; // Le script sur la Main Camera
    public MonoBehaviour robotMovementScript; // Glisse ici le script de marche du robot pour le couper

    void Update()
    {
        if (isPlayerInZone)
        {
            // Touche Entrée ou X pour OUVRIR
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.X)) && !isOpen)
            {
                OuvrirSequence();
            }

            // Touche Y pour FERMER et QUITTER le mode Focus
            if (Input.GetKeyDown(KeyCode.Y) && isOpen)
            {
                FermerSequence();
            }
        }
    }

    void OuvrirSequence()
    {
        isOpen = true;

        // 1. Désactiver l'interface "Appuyer sur X"
        if (monInterface != null) monInterface.SetActive(false);

        // 2. Lancer l'animation de la porte
        porteAnimator.SetBool("isOpen", true);

        // 3. Bloquer le mouvement du robot
        if (robotMovementScript != null) robotMovementScript.enabled = false;

        // 4. Lancer l'hologramme et le focus caméra (avec un léger retard pour laisser la porte s'ouvrir)
        Invoke("DeclencherHolo", 0.5f);
    }

    void DeclencherHolo()
    {
        if (holoAnimator != null) holoAnimator.SetTrigger("Apparition");
        if (camFocus != null) camFocus.ActiverFocus(true);
    }

    void FermerSequence()
    {
        // 1. Faire rentrer l'hologramme
        if (holoAnimator != null) holoAnimator.SetTrigger("Disparition");

        // 2. Remettre la caméra sur le robot
        if (camFocus != null) camFocus.ActiverFocus(false);

        // 3. Attendre que l'hologramme disparaisse pour fermer la porte et libérer le robot
        Invoke("FinaliserFermeture", 1.0f);
    }

    void FinaliserFermeture()
    {
        isOpen = false;
        porteAnimator.SetBool("isOpen", false);

        // Redonner le contrôle au joueur
        if (robotMovementScript != null) robotMovementScript.enabled = true;
        if (monInterface != null) monInterface.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (!isOpen && monInterface != null) monInterface.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (monInterface != null) monInterface.SetActive(false);
        }
    }
}