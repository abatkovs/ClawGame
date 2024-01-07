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
        Debug.Log("Crane spawned");
        GameStarted();
    }

    private void Update()
    {
        //TODO: make it so that the crane can only move when the game has started and has a player to chase
        if(selectedPlayerToChase == null) return;
        
        if (IsServer)
        {
            var position = transform.position;
            movementVelocity = selectedPlayerToChase.position - position;
            movementVelocity.Normalize();
            movementVelocity.y = 0;
            movementVelocity *= speed * Time.deltaTime;
            position += movementVelocity;
            transform.position = position;
        }
        
    }

    [ContextMenu("StartGameTest")]
    private void GameStarted()
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