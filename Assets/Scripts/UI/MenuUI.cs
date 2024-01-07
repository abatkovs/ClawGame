using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private StyleSheet _styleSheet;
        [Space(10)] [SerializeField] private MainMenuState mainMenuState = MainMenuState.MainMenu;

        private TextField _joinCode;
        private VisualElement _lobbyListContainer;
        private bool _isRefreshing;
        private bool _isJoining;

        private void Start()
        {
            StartCoroutine(Generate());
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            StartCoroutine(Generate());
        }

        private IEnumerator Generate()
        {
            yield return null;
            var root = _document.rootVisualElement;
            root.Clear();

            root.styleSheets.Add(_styleSheet);

            switch (mainMenuState)
            {
                case MainMenuState.Bootstrap:
                    GenerateBootstrap(root);
                    break;
                case MainMenuState.MainMenu:
                    GenerateMainMenu(root);
                    break;
                case MainMenuState.Lobbies:
                    GenerateLobbies(root);
                    break;
                case MainMenuState.Options:
                    break;
            }
        }

        private void GenerateBootstrap(VisualElement root)
        {
            var container = UIExt.Create("main-menu", "centered");
            root.Add(container);

            var loadingLabel = UIExt.Create<Label>("loading-label");
            loadingLabel.text = "Loading...";
            loadingLabel.style.fontSize = 50f;
            loadingLabel.style.color = Color.white;
            container.Add(loadingLabel);
        }

        private void GenerateLobbies(VisualElement root)
        {
            var container = UIExt.Create("main-menu", "centered");
            container.style.alignItems = Align.Stretch;
            root.Add(container);

            var top = UIExt.Create("top");
            top.style.flexDirection = FlexDirection.Row;
            container.Add(top);

            var backButton = UIExt.Create<Button>("back-button");
            backButton.text = "BACK";
            backButton.clicked += Back;
            top.Add(backButton);

            var refreshButton = UIExt.Create<Button>("refresh-button");
            refreshButton.text = "REFRESH";
            refreshButton.clicked += Refresh;
            top.Add(refreshButton);

            _lobbyListContainer = UIExt.Create("lobby-list-container");
            container.Add(_lobbyListContainer);


        }

        private void GenerateMainMenu(VisualElement root)
        {

            var container = UIExt.Create("main-menu", "centered");
            root.Add(container);

            var hostButton = UIExt.Create<Button>("host-button");
            hostButton.text = "HOST";
            hostButton.clicked += HostServer;
            container.Add(hostButton);

            var lobbiesButton = UIExt.Create<Button>("lobbies-button");
            lobbiesButton.text = "LOBBIES";
            lobbiesButton.clicked += ListLobbies;
            container.Add(lobbiesButton);

            var joinContainer = UIExt.Create("join-container", "inline");
            container.Add(joinContainer);

            var joinButton = UIExt.Create<Button>("join-button");
            joinButton.text = "JOIN";
            joinButton.clicked += JoinGame;
            joinContainer.Add(joinButton);

            _joinCode = UIExt.Create<TextField>("join-code");
            _joinCode.value = "Join Code";
            _joinCode.label = "";
            joinContainer.Add(_joinCode);
        }

        private void ListLobbies()
        {
            Debug.Log("Lobbies");
            mainMenuState = MainMenuState.Lobbies;
            StartCoroutine(Generate());
            RefreshLobbyList();
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


        private void Refresh()
        {
            RefreshLobbyList();
        }

        private void Back()
        {
            mainMenuState = MainMenuState.MainMenu;
            StartCoroutine(Generate());
        }

        private async void RefreshLobbyList()
        {
            if (_isRefreshing) return;
            _isRefreshing = true;

            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 10;
                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT,
                        value: "0"),
                    new QueryFilter(field: QueryFilter.FieldOptions.IsLocked, op: QueryFilter.OpOptions.EQ, value: "0")
                };

                QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
                _lobbyListContainer.Clear();

                foreach (var lobby in lobbies.Results)
                {
                    var lobbyItem = UIExt.Create<LobbyItem>("lobby-item");
                    lobbyItem.Initialize(lobby);
                    lobbyItem.clicked += delegate { JoinLobbyAsync(lobby); };
                    _lobbyListContainer.Add(lobbyItem);
                }
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
            }

            _isRefreshing = false;
        }
        
        private async void JoinLobbyAsync(Lobby lobby)
        {
            if(_isJoining) return;
            _isJoining = true;
        
            try
            {
                Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                //string joinCode = joiningLobby.LobbyCode;
                string joinCode = joiningLobby.Data["joinCode"].Value;
                Debug.Log($"Joining lobby: {joinCode}");
                await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);

            }   
            catch (LobbyServiceException e)
            {
                Debug.Log($"Failed to join lobby Exception: {e} ");
                //Workaround for joining lobby without changing any profile data on local machine
                //Will not update lobby data though
                //TODO: add proper authentication by username at least
                await ClientSingleton.Instance.GameManager.StartClientAsync(lobby.Name);
            }
            _isJoining = false;
        }
    }
}

