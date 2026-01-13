using UnityEngine;
using TMPro;

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

    private int index = 0;
    private bool isActive = false;

    void Start()
    {
        if (infoText != null)
            infoText.gameObject.SetActive(false);

        UpdateAlbums();
    }

    void OnEnable()
    {
        isActive = true;
    }

    void OnDisable()
    {
        isActive = false;

        if (GlobalAudioPlayer.Instance != null)
            GlobalAudioPlayer.Instance.Stop();
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(KeyCode.D))
            NextAlbum();

        if (Input.GetKeyDown(KeyCode.A))
            PreviousAlbum();

        if (Input.GetKeyDown(KeyCode.X))
            PlayPause();

        if (Input.GetKeyDown(KeyCode.I))
            ToggleInfo();
    }

    // =========================
    // NAVIGATION ALBUMS
    // =========================

    void NextAlbum()
    {
        index = (index + 1) % albums.Length;
        UpdateAlbums();
        HideInfo();
    }

    void PreviousAlbum()
    {
        index--;
        if (index < 0) index = albums.Length - 1;
        UpdateAlbums();
        HideInfo();
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

    // =========================
    // AUDIO (GLOBAL)
    // =========================

    void PlayPause()
    {
        if (albums == null || albums.Length == 0) return;

        AudioClip clip = albums[index].audioClip;
        if (clip == null) return;

        GlobalAudioPlayer.Instance.Toggle(clip);
    }

    // =========================
    // INFO UI
    // =========================

    void ToggleInfo()
    {
        if (infoText == null) return;

        bool active = !infoText.gameObject.activeSelf;
        infoText.gameObject.SetActive(active);

        if (active)
        {
            infoText.text =
                albums[index].albumName + "\n\n" +
                albums[index].description;
        }
    }

    void HideInfo()
    {
        if (infoText != null)
            infoText.gameObject.SetActive(false);
    }
}
