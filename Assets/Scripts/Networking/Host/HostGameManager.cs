using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation _allocation;
    private string _joinCode;
    private string _lobbyId;
    
    private const int MaxConnections = 10;
    private const string GameSceneName = "Game";
    
    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }
        
        try
        {
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log($"Join code: {_joinCode}");
            GameManager.Instance.SetRoomName(_joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }
        //Swap network transport protocol to relay server
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //TODO: try to use dtls might not work in some cases (more secure)
        RelayServerData relayServerData = new RelayServerData(_allocation, "udp");
        unityTransport.SetRelayServerData(relayServerData);
        
        if (await CreateLobby()) return;
        
        NetworkManager.Singleton.StartHost();
        
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private async Task<bool> CreateLobby()
    {
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                //Set join code as lobby data
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        "joinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member, value: _joinCode)
                    }
                }
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{_joinCode}", MaxConnections, lobbyOptions);
            _lobbyId = lobby.Id;
            //Recomended heartbeat time is 15 seconds
            //https://support.unity.com/hc/en-us/articles/4408402562580-Understanding-and-Implementing-Lobby-Heartbeats#
            HostSingleton.Instance.StartCoroutine(LobbyHeartbeat(15f));
        }
        catch (LobbyServiceException lobbyException)
        {
            Debug.Log(lobbyException);
            return true;
        }

        return false;
    }


    /// <summary>
    /// Send hearbeat to lobby service to keep lobby alive
    /// </summary>
    /// <param name="waitTimeSeconds"></param>
    /// <returns></returns>
    IEnumerator LobbyHeartbeat(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return delay;
        }
    }
}
