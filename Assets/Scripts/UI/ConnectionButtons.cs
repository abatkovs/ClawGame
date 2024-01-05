using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionButtons : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;

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
        _root = _document.rootVisualElement;
        _root.Clear();
        
        _root.styleSheets.Add(_styleSheet);

        var container = Create("container");
        _root.Add(container);
        
        var hostButton = Create<Button>("host-button");
        hostButton.text = "HOST";
        hostButton.clicked += HostServer;
        container.Add(hostButton);

        var joinButton = Create<Button>("join-button");
        joinButton.text = "JOIN";
        joinButton.clicked += JoinGame;
        container.Add(joinButton);
    }

    //TODO: Create static method for creating UIElements
    private VisualElement Create(params string[] className)
    {
        return Create<VisualElement>(className);
    }
    
    //Create UIElement and assign style classes to it
    T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        var uiElement = new T();
        foreach (var className in classNames)
        {
            uiElement.AddToClassList(className);
        }
        return uiElement;
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
