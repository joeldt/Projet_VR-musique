using UnityEngine;

public class GlobalAudioPlayer : MonoBehaviour
{
    //  permet d'accéder à ce script depuis n'importe quel autre script de la scène.
    public static GlobalAudioPlayer Instance;

    // Composant Unity qui va diffuser le son dans le monde virtuel.
    private AudioSource audioSource;
    // Stocke le morceau de musique actuellement chargé 
    private AudioClip currentClip;

    void Awake()
    {
        //  on s'assure qu'il n'y a qu'un seul lecteur audio dans tout le jeu.
        if (Instance == null)
        {
            Instance = this;
            // On récupère automatiquement le composant AudioSource attaché à cet objet.
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            // Si un autre lecteur existe déjà, on détruit celui-ci pour éviter les doublons sonores.
            Destroy(gameObject);
        }
    }

    // Fonction principale pour Gérer la lecture (Play / Pause ).
    public void Toggle(AudioClip clip)
    {
        //  si aucun son n'est fourni ou si le lecteur est manquant, on ne fait rien.
        if (clip == null || audioSource == null) return;

        // Cas 1 : On appuie sur le bouton de la musique qui est déjà en train de passer.
        if (currentClip == clip)
        {
            if (audioSource.isPlaying)
                // Si la musique joue, on la met en pause.
                audioSource.Pause();
            else
                // Si la musique est en pause, on la relance là où elle s'était arrêtée.
                audioSource.UnPause();

            return;
        }

        // Cas 2 : On choisit une nouvelle musique (différente de la précédente).
        // On arrête l'ancienne musique.
        audioSource.Stop();
        // On charge le nouveau fichier audio dans le lecteur.
        audioSource.clip = clip;
        // On lance la lecture.
        audioSource.Play();
        // On mémorise quel est le nouveau morceau actuel.
        currentClip = clip;
    }

    // Fonction pour arrêter totalement le son (utilisée par exemple quand on ferme une porte).
    public void Stop()
    {
        if (audioSource == null) return;

        // Arrêt physique du son.
        audioSource.Stop();
        // On vide la mémoire du morceau actuel pour qu'une prochaine lecture reparte du début.
        currentClip = null;
    }
}