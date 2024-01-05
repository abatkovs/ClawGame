using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace _MyAssets.Scripts.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private bool isBootstrap;
        [SerializeField] private UIDocument _document;
        [SerializeField] private StyleSheet _styleSheet;
        
        private TextField _joinCode;
        
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
            var root = _document.rootVisualElement;
            root.Clear();
            
            root.styleSheets.Add(_styleSheet);

            var container = UIExt.Create("main-menu", "centered");
            root.Add(container);

            if (isBootstrap)
            {
                var loadingLabel = UIExt.Create<Label>("loading-label");
                loadingLabel.text = "Loading...";
                loadingLabel.style.fontSize = 50f;
                loadingLabel.style.color = Color.white;
                container.Add(loadingLabel);
                yield break;
            }
            
            var hostButton = UIExt.Create<Button>("host-button");
            hostButton.text = "HOST";
            hostButton.clicked += HostServer;
            container.Add(hostButton);

            var joinButton = UIExt.Create<Button>("join-button");
            joinButton.text = "JOIN";
            joinButton.clicked += JoinGame;
            container.Add(joinButton);
            
            var lobbiesButton = UIExt.Create<Button>("lobbies-button");
            lobbiesButton.text = "LOBBIES";
            lobbiesButton.clicked += ListLobbies;
            container.Add(lobbiesButton);
            
            _joinCode = UIExt.Create<TextField>("join-code");
            _joinCode.value = "Join Code";
            _joinCode.label = "";
            container.Add(_joinCode);
        }

        private void ListLobbies()
        {
            Debug.Log("Lobbies");
        }

        private async void JoinGame()
        {
            Debug.Log("Start client and join game");
            await ClientSingleton.Instance.GameManager.StartClientAsync(_joinCode.value);
        }

        private async void HostServer()
        {
            Debug.Log("Try start Hosting");
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }
    }
}
