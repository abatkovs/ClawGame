using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation _allocation;
    private string _joinCode;
    
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
        
        NetworkManager.Singleton.StartHost();
        
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
}
