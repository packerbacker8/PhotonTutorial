using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMangerScript : Photon.PunBehaviour
{
    static public GameMangerScript Manager;
    public GameObject playerPrefab;

    private void Start()
    {
        Manager = this;

        if(playerPrefab == null)
        {
            Debug.LogError("No player prefab provided", this);
        }
        else
        {
            if(PlayerManager.localPlayerInstance == null)
            {
                Debug.Log("Instatiating player for " + SceneManager.GetActiveScene().name);
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.Log("Do not need to load this player prefab, already exists");
            }
        }

    }

    /// <summary>
    /// Called when the local player left the room.
    /// Load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }

    /// <summary>
    /// Wrapper method to leave room of photon network room.
    /// Added functionality for:
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    /// <summary>
    /// Called on everyone who is connected to this Photon network
    /// when a player joins.
    /// </summary>
    /// <param name="newPlayer">The new player that joined.</param>
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Player was connected: " + newPlayer.NickName);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("This player is the master client.");

            LoadArena();
        }
    }

    /// <summary>
    /// Called on everyone who is connected to this Photon network
    /// when a player leaves.
    /// </summary>
    /// <param name="otherPlayer">The player that just left.</param>
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Player was disconnected: " + otherPlayer.NickName);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("this is the master client");

            LoadArena();
        }
    }

    private void LoadArena()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("We are trying to load a level but are not the master client.");
        }
        Debug.Log("PhotonNetwork loading level of " + PhotonNetwork.room.PlayerCount);
        PhotonNetwork.LoadLevel("RoomFor" + PhotonNetwork.room.PlayerCount);
    }
}
