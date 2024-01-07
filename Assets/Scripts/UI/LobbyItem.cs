using Unity.Services.Lobbies.Models;
using UnityEngine.UIElements;

namespace UI
{
    public class LobbyItem : Button
    {
        public void Initialize(Lobby lobby)
        {
            text = $"Lobby: {lobby.Name} ({lobby.Players.Count}/{lobby.MaxPlayers})";
        }
    }
}