using UnityEngine;

public class OuvrirPorteInteraction : MonoBehaviour
{
    [Header("Configuration Porte")]
    public Animator porteAnimator;
    public GameObject monInterface;
    private bool isPlayerInZone = false;
    private bool isOpen = false;

    [Header("Configuration Hologramme")]
    public GameObject hologramme;          // 🔴 CHANGÉ : GameObject du hologramme
    public Animator holoAnimator;           // Animator du hologramme (apparition / disparition)

    [Header("Configuration Caméra & Robot")]
    public CameraFocusController camFocus;
    public MonoBehaviour robotMovementScript;
    public Transform maCibleCamera;

    void Start()
    {
        // 🔒 Sécurité : hologramme TOUJOURS désactivé au départ
        if (hologramme != null)
            hologramme.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerInZone) return;

        // OUVRIR la porte
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.X)) && !isOpen)
        {
            OuvrirSequence();
        }

        // FERMER la porte
        if (Input.GetKeyDown(KeyCode.Y) && isOpen)
        {
            FermerSequence();
        }
    }

    void OuvrirSequence()
    {
        isOpen = true;

        // Désactiver l'interface "Appuyez sur X"
        if (monInterface != null)
            monInterface.SetActive(false);

        // Animation porte
        porteAnimator.SetBool("isOpen", true);

        // Bloquer mouvement robot
        if (robotMovementScript != null)
            robotMovementScript.enabled = false;

        // Activer l'hologramme après ouverture porte
        Invoke(nameof(DeclencherHolo), 0.5f);
    }

    void DeclencherHolo()
    {
        // ✅ ACTIVER le GameObject hologramme
        if (hologramme != null)
            hologramme.SetActive(true);

        // Animation apparition
        if (holoAnimator != null)
            holoAnimator.SetTrigger("Apparition");

        // Focus caméra
        if (camFocus != null)
        {
            camFocus.positionHologramme = maCibleCamera;
            camFocus.ActiverFocus(true);
        }
    }

    void FermerSequence()
    {
        // Animation disparition hologramme
        if (holoAnimator != null)
            holoAnimator.SetTrigger("Disparition");

        // STOP AUDIO IMMÉDIAT
        if (GlobalAudioPlayer.Instance != null)
            GlobalAudioPlayer.Instance.Stop();

        // Retour caméra
        if (camFocus != null)
            camFocus.ActiverFocus(false);

        Invoke(nameof(FinaliserFermeture), 1.0f);
    }

    void FinaliserFermeture()
    {
        isOpen = false;

        // Fermer porte
        porteAnimator.SetBool("isOpen", false);

        // ❌ Désactiver hologramme
        if (hologramme != null)
            hologramme.SetActive(false);

        // Redonner contrôle robot
        if (robotMovementScript != null)
            robotMovementScript.enabled = true;

        if (monInterface != null)
            monInterface.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (!isOpen && monInterface != null)
                monInterface.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (monInterface != null)
                monInterface.SetActive(false);
        }
    }
}
