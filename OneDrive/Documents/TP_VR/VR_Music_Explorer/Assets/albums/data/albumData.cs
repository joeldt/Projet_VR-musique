using UnityEngine;

[CreateAssetMenu(menuName = "VR Music Explorer/Album Data")]
public class AlbumData : ScriptableObject
{
    public string albumName;

    [TextArea(3, 10)]
    public string description;

    public Texture coverTexture;
    public AudioClip audioClip;
}
