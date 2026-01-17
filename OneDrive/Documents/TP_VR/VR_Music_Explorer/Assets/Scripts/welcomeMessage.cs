using UnityEngine;

public class WelcomeMessage : MonoBehaviour
{
    [Header("Objet à cacher (ex: WelcomeCanvas)")]
    public GameObject welcomeUI;

    [Header("Durée d'affichage en secondes")]
    public float showDuration = 6f;

    void Start()
    {
        if (welcomeUI != null)
        {
            welcomeUI.SetActive(true);
            Invoke(nameof(HideWelcome), showDuration);
        }
    }

    void HideWelcome()
    {
        if (welcomeUI != null)
            welcomeUI.SetActive(false);
    }
}
