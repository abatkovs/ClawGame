using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crane : NetworkBehaviour
{
    
    [SerializeField] private Transform selectedPlayerToChase;

    [SerializeField] private Vector3 movementVelocity;
    [SerializeField] private float speed = 5f;

    public override void OnNetworkSpawn()
    {
        Debug.Log("Crane spawned OnNetworkSpawn");
        
    }

    private void Update()
    {
        //TODO: make it so that the crane can only move when the game has started and has a player to chase
        //Requires at least 3 players to start chasing
        if(!IsServer) return;
        if(NetworkManager.Singleton.ConnectedClients.Count <= 2) return;
        if (selectedPlayerToChase == null)
        {
            StartGame();
            return;
        }

        var position = transform.position;
        movementVelocity = selectedPlayerToChase.position - position;
        movementVelocity.Normalize();
        movementVelocity.y = 0;
        movementVelocity *= speed * Time.deltaTime;
        position += movementVelocity;
        transform.position = position;
    }

    [ContextMenu("StartGameTest")]
    private void StartGame()
    {
        if (IsServer)
        {
            SelectPlayerToChase();
        }
    }

    private void SelectPlayerToChase()
    {
        var activePlayers = GameManager.Instance.Characters.FindAll(character => character);
        var randomPlayer = activePlayers[Random.Range(0, activePlayers.Count)];
        selectedPlayerToChase = randomPlayer.transform;
    }
}
