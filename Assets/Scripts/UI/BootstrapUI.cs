using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BootstrapUI : MonoBehaviour
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
    
    IEnumerator Generate()
    {
        yield return null;
        _root = document.rootVisualElement;
        _root.Clear();
        
        _root.styleSheets.Add(styleSheet);

        var container = UIExt.Create("container");
        _root.Add(container);
        
        var loadingLabel = UIExt.Create<Label>("loading-label");
        loadingLabel.text = "Loading...";
        loadingLabel.style.fontSize = 50;
        loadingLabel.style.color = Color.white;
        container.Add(loadingLabel);
    }
}
