using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class OuvrirPorteInteraction : MonoBehaviour
{
    [Header("Boutons VR (Input Actions)")]
    // 'action du bouton A  pour déclencher l'ouverture.
    public InputActionProperty boutonA; 
    // action du bouton B pour déclencher la fermeture.
    public InputActionProperty boutonB; 

    [Header("Configuration Porte")]  
    public Animator porteAnimator;// Animator gérant les états "Ouvert/Fermé" de la porte
    public GameObject monInterface; // Canvas affichée uniquement quand le joueur est à proximité.

    private bool isPlayerInZone = false;//pour savoir si le robot est dans la zone de détection (Trigger).
   
    private bool isOpen = false;//pour suivre si la porte est actuellement ouverte.

    [Header("Configuration Hologramme")]
    // L'objet racine de l'hologramme qui apparaît après l'ouverture.
    public GameObject hologramme;          
    // Animator gérant les effets d'apparitionde l'hologramme.
    public Animator holoAnimator;           

    [Header("Configuration Caméra & Robot")]
    // Script gérant le déplacement de la caméra vers une cible précise.
    public CameraFocusController camFocus;
    // Référence au script de mouvement pour immobiliser le robot pendant l'interaction.
    public MonoBehaviour robotMovementScript;
    // Point de pivot  vers lequel la caméra doit regarder.
    public Transform maCibleCamera;

    [Header("Réglages Délais")]
    // Temps d'attente pour que la porte soit assez ouverte avant de montrer l'hologramme.
    public float delaiApparitionHolo = 0.5f;
    // Temps d'attente pour laisser l'animation de disparition finir avant de fermer la porte.
    public float delaiFermeturePorte = 1.0f;

    void Start()
    {
        // On s'assure que l'hologramme est masqué par défaut au lancement de la scène.
        if (hologramme != null)
            hologramme.SetActive(false);

        // Activation des actions d'entrée pour que le système XR écoute les manettes.
        if (boutonA.action != null) boutonA.action.Enable();
        if (boutonB.action != null) boutonB.action.Enable();
    }

    void Update()
    {
        // Si le joueur n'est pas dans la zone du Trigger, on ignore le reste du script.
        if (!isPlayerInZone) return;

        // On n'écoute le bouton A que si la porte est fermée.
        if (!isOpen && boutonA.action.WasPressedThisFrame())
        {
            OuvrirSequence();
        }

        // On écoute le bouton B uniquement si la porte est déjà ouverte.
        if (isOpen && boutonB.action.WasPressedThisFrame())
        {
            FermerSequence();
        }
    }

    void OuvrirSequence()
    {
        isOpen = true;
        
        // Désactivation de l'interface "Appuyer sur A" une fois l'action lancée.
        if (monInterface != null)
            monInterface.SetActive(false);

        // Déclenchement de l'animation de la porte dans l'Animator.
        if (porteAnimator != null)
            porteAnimator.SetBool("isOpen", true);

        // Désactive le script de mouvement du robot pour empêcher le joueur de partir pendant le focus.
        if (robotMovementScript != null)
            robotMovementScript.enabled = false;

        // Utilisation d'une Coroutine pour synchroniser l'arrivée de l'hologramme avec la porte.
        StartCoroutine(DeclencherHoloApresDelai());
    }

    IEnumerator DeclencherHoloApresDelai()
    {
        // Pause l'exécution pendant la durée réglée dans l'inspecteur.
        yield return new WaitForSeconds(delaiApparitionHolo);

        // Rend l'objet hologramme visible dans la scène.
        if (hologramme != null)
            hologramme.SetActive(true);

        // Lance l'animation Trigger "Apparition" définie dans l'animator de l'hologramme.
        if (holoAnimator != null)
            holoAnimator.SetTrigger("Apparition");

        // Oriente la vue du joueur vers l'hologramme pour une mise en scène immersive.
        if (camFocus != null)
        {
            camFocus.positionHologramme = maCibleCamera;
            camFocus.ActiverFocus(true);
        }
    }

    void FermerSequence()
    {
        // Lance l'animation de retrait (ex: fondu ou réduction) de l'hologramme.
        if (holoAnimator != null)
            holoAnimator.SetTrigger("Disparition");

        // Arrête la musique associée à cette session via le Singleton GlobalAudioPlayer.
        if (GlobalAudioPlayer.Instance != null)
            GlobalAudioPlayer.Instance.Stop();

        // Relâche le focus caméra pour redonner la vue libre au joueur.
        if (camFocus != null)
            camFocus.ActiverFocus(false);

        // Attend que l'hologramme ait disparu visuellement avant de refermer physiquement la porte.
        StartCoroutine(FinaliserFermetureApresDelai());
    }

    IEnumerator FinaliserFermetureApresDelai()
    {
        yield return new WaitForSeconds(delaiFermeturePorte);

        isOpen = false;
        
        // Joue l'animation de fermeture de la porte.
        if (porteAnimator != null)
            porteAnimator.SetBool("isOpen", false);

        // Désactive totalement l'objet pour optimiser les performances.
        if (hologramme != null)
            hologramme.SetActive(false);

        // Réactive le script de mouvement pour que le joueur puisse à nouveau déplacer le robot.
        if (robotMovementScript != null)
            robotMovementScript.enabled = true;

        // Si le joueur est toujours devant la porte, on réaffiche l'indice visuel "Appuyer sur A".
        if (monInterface != null && isPlayerInZone)
            monInterface.SetActive(true);
    }

    // Détection automatique quand le robot entre dans la zone d'interaction.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            // Affiche l'interface si la porte n'est pas déjà ouverte.
            if (!isOpen && monInterface != null)
                monInterface.SetActive(true);
        }
    }

    // Détection quand le robot s'éloigne de la porte.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            // Cache l'interface par sécurité.
            if (monInterface != null)
                monInterface.SetActive(false);
        }
    }
}