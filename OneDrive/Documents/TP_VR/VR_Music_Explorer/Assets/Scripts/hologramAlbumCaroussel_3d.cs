using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class HologramAlbumCarousel_3D : MonoBehaviour
{
    [Header("Albums Data")]
    public AlbumData[] albums;

    [Header("VR Inputs")]
    public InputActionProperty triggerDroit; 
    public InputActionProperty boutonA;       
    public InputActionProperty boutonX;       
    public Transform mainDroite;            

    [Header("Renderers (Visuels)")]
    public Renderer albumGauche;
    public Renderer albumCentre;
    public Renderer albumDroite;

    [Header("UI System")]
    public GameObject canvasDetails; //  Canvas à afficher/cacher
    public TMP_Text detailText;      // Le texte à l'intérieur du Canvas Details

    [Header("Paramètres Swipe")]
    public float seuilBalayage = 0.15f;

    private int index = 0;
    private bool isSwiping = false;
    private Vector3 startSwipePos;

    void OnEnable()
    {
        if (triggerDroit.action != null) triggerDroit.action.Enable();
        if (boutonA.action != null) boutonA.action.Enable();
        if (boutonX.action != null) boutonX.action.Enable();
        
        if (canvasDetails != null) canvasDetails.SetActive(false);
        UpdateAlbums();
    }

    void Update()
    {
        //  SWIPE
        if (triggerDroit.action != null && triggerDroit.action.IsPressed())
        {
            if (!isSwiping) { isSwiping = true; startSwipePos = mainDroite.position; }
            else { CheckSwipe(); }
        }
        else { isSwiping = false; }

        //  MUSIQUE (A)
        if (boutonA.action != null && boutonA.action.triggered) { PlayPause(); }

        // INFOS : Affiche/Cache le Canvas Details
        if (boutonX.action != null && boutonX.action.triggered) { ToggleInfo(); }
    }

    void CheckSwipe()
    {
        if (mainDroite == null) return;
        float deltaX = mainDroite.position.x - startSwipePos.x;
        if (Mathf.Abs(deltaX) > seuilBalayage)
        {
            if (deltaX > 0) NextAlbum(); else PreviousAlbum();
            startSwipePos = mainDroite.position; 
        }
    }

    public void NextAlbum()
    {
        index = (index + 1) % albums.Length;
        UpdateAlbums();
        if (canvasDetails != null) canvasDetails.SetActive(false); // Cache les infos au changement
    }

    public void PreviousAlbum()
    {
        index--;
        if (index < 0) index = albums.Length - 1;
        UpdateAlbums();
        if (canvasDetails != null) canvasDetails.SetActive(false);
    }

    void UpdateAlbums()
    {
        if (albums == null || albums.Length == 0) return;
        int left = (index - 1 + albums.Length) % albums.Length;
        int right = (index + 1) % albums.Length;

        albumGauche.material.mainTexture = albums[left].coverTexture;
        albumCentre.material.mainTexture = albums[index].coverTexture;
        albumDroite.material.mainTexture = albums[right].coverTexture;
    }

    public void PlayPause()
    {
        if (albums[index].audioClip != null && GlobalAudioPlayer.Instance != null)
            GlobalAudioPlayer.Instance.Toggle(albums[index].audioClip);
    }

    public void ToggleInfo()
    {
        if (canvasDetails == null || detailText == null) return;

        bool isCurrentlyActive = canvasDetails.activeSelf;
        canvasDetails.SetActive(!isCurrentlyActive);

        if (!isCurrentlyActive) // Si on vient de l'allumer
        {
            detailText.text = "<b>" + albums[index].albumName + "</b>\n\n" + albums[index].description;
        }
    }
}