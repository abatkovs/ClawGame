using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
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
        
        var roomLabel = UIExt.Create<Label>("room-label");
        if(Application.isPlaying) roomLabel.text = $"ROOM: {GameManager.Instance.RoomName}";
        container.Add(roomLabel);
        
        var coinLabel = UIExt.Create<Label>();
        coinLabel.text = "COINS";
        container.Add(coinLabel);
    }
}
