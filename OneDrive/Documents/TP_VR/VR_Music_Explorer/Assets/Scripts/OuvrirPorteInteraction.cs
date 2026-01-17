using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class OuvrirPorteInteraction : MonoBehaviour
{
    [Header("Boutons VR (Input Actions)")]
    public InputActionProperty boutonA; // Uniquement pour OUVRIR
    public InputActionProperty boutonB; // Uniquement pour FERMER

    [Header("Configuration Porte")]
    public Animator porteAnimator;
    public GameObject monInterface; // Texte "Appuyer sur A"
    private bool isPlayerInZone = false;
    private bool isOpen = false;

    [Header("Configuration Hologramme")]
    public GameObject hologramme;          
    public Animator holoAnimator;           

    [Header("Configuration Caméra & Robot")]
    public CameraFocusController camFocus;
    public MonoBehaviour robotMovementScript;
    public Transform maCibleCamera;

    [Header("Réglages Délais")]
    public float delaiApparitionHolo = 0.5f;
    public float delaiFermeturePorte = 1.0f;

    void Start()
    {
        //  hologramme TOUJOURS désactivé au départ
        if (hologramme != null)
            hologramme.SetActive(false);

        // Activation des actions VR
        if (boutonA.action != null) boutonA.action.Enable();
        if (boutonB.action != null) boutonB.action.Enable();
    }

    void Update()
    {
        if (!isPlayerInZone) return;

        // On n'écoute le bouton A que si la porte est fermée
        if (!isOpen && boutonA.action.WasPressedThisFrame())
        {
            OuvrirSequence();
        }

        //On écoute le bouton B  si la porte est ouverte
        if (isOpen && boutonB.action.WasPressedThisFrame())
        {
            FermerSequence();
        }
        
    }

    void OuvrirSequence()
    {
        isOpen = true;
        
        // Cacher l'interface de proximité
        if (monInterface != null)
            monInterface.SetActive(false);

        // Animation de la porte
        if (porteAnimator != null)
            porteAnimator.SetBool("isOpen", true);

        // Bloquer mouvement robot pour le focus
        if (robotMovementScript != null)
            robotMovementScript.enabled = false;

        // Lancement de l'hologramme après le délai d'ouverture
        StartCoroutine(DeclencherHoloApresDelai());
    }

    IEnumerator DeclencherHoloApresDelai()
    {
        yield return new WaitForSeconds(delaiApparitionHolo);

        // ACTIVER le  hologramme
        if (hologramme != null)
            hologramme.SetActive(true);

        // Déclencher l'animation d'apparition
        if (holoAnimator != null)
            holoAnimator.SetTrigger("Apparition");

        // Focus caméra (Camera Offset) vers l'hologramme
        if (camFocus != null)
        {
            camFocus.positionHologramme = maCibleCamera;
            camFocus.ActiverFocus(true);
        }
    }

    void FermerSequence()
    {
        // Disparition visuelle de l'hologramme
        if (holoAnimator != null)
            holoAnimator.SetTrigger("Disparition");

        // STOP AUDIO 
        if (GlobalAudioPlayer.Instance != null)
            GlobalAudioPlayer.Instance.Stop();

        //  Rétablir la caméra normale
        if (camFocus != null)
            camFocus.ActiverFocus(false);

        //  Attendre la fin de l'anim hologramme avant de fermer la porte
        StartCoroutine(FinaliserFermetureApresDelai());
    }

    IEnumerator FinaliserFermetureApresDelai()
    {
        yield return new WaitForSeconds(delaiFermeturePorte);

        isOpen = false;
        
        // Fermer physiquement la porte
        if (porteAnimator != null)
            porteAnimator.SetBool("isOpen", false);

        // Désactiver totalement l'objet hologramme
        if (hologramme != null)
            hologramme.SetActive(false);

        // Redonner le contrôle au robot
        if (robotMovementScript != null)
            robotMovementScript.enabled = true;

        // Réafficher l'interface si on est encore dans la zone
        if (monInterface != null && isPlayerInZone)
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