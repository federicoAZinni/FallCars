using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    [Header("Player Ref")]
    public NetworkObject playerPrefab;
    public GameObject localPlayerGameObject;
    [SerializeField] CinemachineVirtualCamera camCineMachine;
    [SerializeField] FixedJoystick joystick;
    [SerializeField] CarController carController;


    [Space(5)]
    [Header("Spawn Ref")]
    public Transform[] spawnPoints;
    
    

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost) SpawnPlayers();
    }

    void SpawnPlayers()
    {
        int index = 0;
        List<NetworkObject> players = new List<NetworkObject>();
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            players.Add(NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, player.ClientId, false, true, false,spawnPoints[index].position));
            index++;
        }

        SetLocalPlayerVairableClientRpc();
    }

    [ClientRpc]
    void SetLocalPlayerVairableClientRpc()
    {
        foreach (var item in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            if (item.IsOwner) localPlayerGameObject = item.gameObject;
        }

        SetVirtualCamera();
        SetPlayerReferences();
    }

    void SetVirtualCamera()
    {
        camCineMachine.Follow = localPlayerGameObject.transform;
        camCineMachine.LookAt = localPlayerGameObject.transform;
    }

    void SetPlayerReferences()
    {
        if(localPlayerGameObject.TryGetComponent<CarController>(out carController))
        {
            carController.joystick = joystick;
        }
    }

    public void Accelerated(bool n)
    {
        carController.AccelerateBtn(n);
    }
    public void Break(bool n)
    {
        carController.BreakBtn(n);
    }
}
