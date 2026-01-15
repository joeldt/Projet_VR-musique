using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Indispensable pour les nouvelles actions
using UnityEngine.XR.Interaction.Toolkit;

public class HologramAlbumCarousel_3D : MonoBehaviour
{
    [Header("Albums")]
    public AlbumData[] albums;

    [Header("Renderers")]
    public Renderer albumGauche;
    public Renderer albumCentre;
    public Renderer albumDroite;

    [Header("UI")]
    public TMP_Text infoText;

    [Header("VR Inputs (Mains)")]
    public InputActionProperty triggerAction; // Gâchette Droite (Maintien)
    public InputActionProperty swipeAction;   // Joystick Droit (Balayage)

    [Header("Actions Boutons Spécifiques")]
    public InputActionProperty buttonAAction; // Bouton A (Infos)
    public InputActionProperty buttonYAction; // Bouton Y (Fermer)

    [Header("Lien Porte")]
    public OuvrirPorteInteraction scriptPorte; // Pour fermer la porte via Y

    private int index = 0;
    private bool isActive = false;
    private bool hasSwiped = false;

    void Start()
    {
        if (infoText != null) infoText.gameObject.SetActive(false);
        UpdateAlbums();
    }

    void OnEnable() { isActive = true; }
    void OnDisable()
    {
        isActive = false;
        if (GlobalAudioPlayer.Instance != null) GlobalAudioPlayer.Instance.Stop();
    }

    void Update()
    {
        if (!isActive) return;

        // 1. GESTION DU BALAYAGE (TRIGGER + JOYSTICK DROIT)
        HandleSwipeNavigation();

        // 2. DETECTION DU BOUTON A (Main Droite)
        if (buttonAAction.action.WasPressedThisFrame())
        {
            ToggleInfo();
        }

        // 3. DETECTION DU BOUTON Y (Main Gauche)
        if (buttonYAction.action.WasPressedThisFrame())
        {
            OnPressY();
        }

        // On garde le bouton X via l'interactable ou on peut l'ajouter ici aussi :
        // if (Input.GetKeyDown(KeyCode.X)) PlayPause(); 
    }

    void HandleSwipeNavigation()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        Vector2 swipeValue = swipeAction.action.ReadValue<Vector2>();

        if (triggerValue > 0.5f) // Si gâchette maintenue
        {
            if (!hasSwiped)
            {
                if (swipeValue.x > 0.7f) { NextAlbum(); hasSwiped = true; }
                else if (swipeValue.x < -0.7f) { PreviousAlbum(); hasSwiped = true; }
            }
        }
        else
        {
            hasSwiped = false; // Reset quand on relâche
        }
    }

    public void OnPressX() // Appelée par l'événement Select Entered de la zone
    {
        if (!isActive) return;
        PlayPause();
    }

    public void OnPressY()
    {
        if (!isActive) return;
        if (scriptPorte != null) scriptPorte.OnPressY();
    }

    // --- LOGIQUE INTERNE ---
    void NextAlbum() { index = (index + 1) % albums.Length; UpdateAlbums(); HideInfo(); }
    void PreviousAlbum() { index--; if (index < 0) index = albums.Length - 1; UpdateAlbums(); HideInfo(); }

    void UpdateAlbums()
    {
        if (albums == null || albums.Length == 0) return;
        int left = (index - 1 + albums.Length) % albums.Length;
        int right = (index + 1) % albums.Length;
        albumGauche.material.mainTexture = albums[left].coverTexture;
        albumCentre.material.mainTexture = albums[index].coverTexture;
        albumDroite.material.mainTexture = albums[right].coverTexture;
    }

    void PlayPause()
    {
        if (albums == null || albums.Length == 0) return;
        AudioClip clip = albums[index].audioClip;
        if (clip != null) GlobalAudioPlayer.Instance.Toggle(clip);
    }

    void ToggleInfo()
    {
        if (infoText == null) return;
        bool active = !infoText.gameObject.activeSelf;
        infoText.gameObject.SetActive(active);
        if (active) infoText.text = albums[index].albumName + "\n\n" + albums[index].description;
    }

    void HideInfo() { if (infoText != null) infoText.gameObject.SetActive(false); }
}