using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionButtons : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private StyleSheet styleSheet;

    private VisualElement _root;
    
    private void Start()
    {
        StartCoroutine(Generate());
    }

    private void OnValidate()
    {
        if(Application.isPlaying) return;
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        yield return null;
        _root = document.rootVisualElement;
        _root.Clear();
        
        _root.styleSheets.Add(styleSheet);

        var container = UI.Create("container");
        _root.Add(container);
        
        var hostButton = UI.Create<Button>("host-button");
        hostButton.text = "HOST";
        hostButton.clicked += HostServer;
        container.Add(hostButton);

        var joinButton = UI.Create<Button>("join-button");
        joinButton.text = "JOIN";
        joinButton.clicked += JoinGame;
        container.Add(joinButton);
    }
    
    private void JoinGame()
    {
        Debug.Log("Join game");
        if (NetworkManager.Singleton.StartClient())
        {
            _root.visible = false;
        }
    }
    
    private void HostServer()
    {
        Debug.Log("Host game");
        if(NetworkManager.Singleton.StartHost())
        {
            _root.visible = false;
        }
    }
}
