using UnityEngine;
using UnityEngine.InputSystem;

public class ForceLocomotionMapOnStart : MonoBehaviour
{
    public InputActionAsset actions;       // XRI Default Input Actions
    public string mapName = "XRI RightHand Locomotion"; // adapte au nom exact

    void Awake()
    {
        if (actions != null)
        {
            var map = actions.FindActionMap(mapName, true);
            map.Enable();
        }
    }
}

