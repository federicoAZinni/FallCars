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
    public ulong localId;

    [Space(5)]
    [Header("Spawn Ref")]
    public Transform[] spawnPoints;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost) SpawnPlayers();
        localId = NetworkManager.Singleton.LocalClientId;
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
            if (item.IsOwner) //Set localPlayer Properties
            {
                localPlayerGameObject = item.gameObject;
                localPlayerGameObject.tag = "Player";
            }
        }

        SetVirtualCamera();
        SetPlayerReferences();
    }
    #region SetLocalPlayer
    void SetVirtualCamera()
    {
        camCineMachine.Follow = localPlayerGameObject.transform;
        camCineMachine.LookAt = localPlayerGameObject.transform;
    }
    void SetPlayerReferences()
    {
        // if(localPlayerGameObject.TryGetComponent<CarController>(out carController))
        // {
        //     carController.joystick = joystick;
        // }
    }
    public void Accelerated(bool n)
    {
        // carController.AccelerateBtn(n);
    }
    public void Break(bool n)
    {
        // carController.BreakBtn(n);
    }
    #endregion

    [ServerRpc(RequireOwnership = false)]
    public void FinishRaceServerRpc(ulong idPlayer)
    {
        FinishRaceClientRpc(idPlayer);
    }

    [ClientRpc]
    public void FinishRaceClientRpc(ulong idPlayerWin)
    {
        Debug.Log(idPlayerWin);

        if(idPlayerWin == NetworkManager.LocalClientId)
        {
            Debug.Log("You Win"); //Poner la pantalla de ganador
        }else
        {
            Debug.Log("You Lose");//Poner la pantalla de perdedor
        }
    }

}
