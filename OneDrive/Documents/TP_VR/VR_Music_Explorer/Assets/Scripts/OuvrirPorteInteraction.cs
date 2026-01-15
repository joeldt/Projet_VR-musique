using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Ajout nécessaire pour la VR

public class OuvrirPorteInteraction : MonoBehaviour
{
    [Header("Configuration Porte")]
    public Animator porteAnimator;
    public GameObject monInterface;
    private bool isPlayerInZone = false;
    private bool isOpen = false;

    [Header("Configuration Hologramme")]
    public GameObject hologramme;
    public Animator holoAnimator;

    [Header("Configuration Caméra & Robot")]
    public CameraFocusController camFocus;
    public MonoBehaviour robotMovementScript;
    public Transform maCibleCamera;

    void Start()
    {
        if (hologramme != null)
            hologramme.SetActive(false);
    }

    // FONCTIONS POUR LA VR 

    // Cette fonction sera liée au bouton X (Select)
    public void OnPressX()
    {
        if (isPlayerInZone && !isOpen)
        {
            OuvrirSequence();
        }
    }

    // Cette fonction sera liée au bouton Y (Cancel)
    public void OnPressY()
    {
        if (isPlayerInZone && isOpen)
        {
            FermerSequence();
        }
    }

    //LOGIQUE DE SEQUENCE 

    void Update()
    {
        // On garde le clavier pour le débug sur PC
        if (!isPlayerInZone) return;

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.X)) && !isOpen)
        {
            OuvrirSequence();
        }

        if (Input.GetKeyDown(KeyCode.Y) && isOpen)
        {
            FermerSequence();
        }
    }

    void OuvrirSequence()
    {
        isOpen = true;
        if (monInterface != null) monInterface.SetActive(false);
        porteAnimator.SetBool("isOpen", true);

        if (robotMovementScript != null)
            robotMovementScript.enabled = false;

        Invoke(nameof(DeclencherHolo), 0.5f);
    }

    void DeclencherHolo()
    {
        if (hologramme != null) hologramme.SetActive(true);
        if (holoAnimator != null) holoAnimator.SetTrigger("Apparition");

        if (camFocus != null)
        {
            camFocus.positionHologramme = maCibleCamera;
            camFocus.ActiverFocus(true);
        }
    }

    void FermerSequence()
    {
        if (holoAnimator != null) holoAnimator.SetTrigger("Disparition");

        if (GlobalAudioPlayer.Instance != null)
            GlobalAudioPlayer.Instance.Stop();

        if (camFocus != null)
            camFocus.ActiverFocus(false);

        Invoke(nameof(FinaliserFermeture), 1.0f);
    }

    void FinaliserFermeture()
    {
        isOpen = false;
        porteAnimator.SetBool("isOpen", false);
        if (hologramme != null) hologramme.SetActive(false);

        if (robotMovementScript != null)
            robotMovementScript.enabled = true;

        if (monInterface != null) monInterface.SetActive(true);
    }

    //  DETECTION DE ZONE 

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