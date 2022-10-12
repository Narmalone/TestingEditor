using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance { get; private set; }
    [SerializeField] public bool EnableSplitScreen;
    [SerializeField] private UIDocument uidoc;
    [SerializeField] private Camera maincamera;
    private void Awake()
    {
        instance = this;
        var rootELement = uidoc.rootVisualElement;
        if (EnableSplitScreen == false)
        {
            var playersInput = GetComponent<PlayerInputManager>();
            playersInput.splitScreen = false;
            maincamera.gameObject.SetActive(true);
            rootELement.style.display = DisplayStyle.None;
        }
        else
        {
            var playersInput = GetComponent<PlayerInputManager>();
            playersInput.splitScreen = true;
            maincamera.gameObject.SetActive(false);
            rootELement.style.display = DisplayStyle.Flex;
        }
    }
    public void PlayerJoinedEvent()
    {
        Debug.Log("un joueur s'est connecté");
    } 
    
    public void PlayerLeftEvent()
    {
        Debug.Log("un joueur s'est déconnecté");
    }
}
