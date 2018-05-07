using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Photon.PunBehaviour
{
    #region Public Vars
    [Tooltip("How many players per room? When it hits max number a new room will be made.")]
    public byte MaxPlayersPerRoom = 4;
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    public GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    public GameObject progressLabel;

    public PhotonLogLevel logLevel = PhotonLogLevel.Informational;
    #endregion

    #region Private Vars
    private string gameVersion = "1";

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// i.e. If the player is trying to connect, do one thing, if photon
    /// is automatically calling this callback do another.
    /// </summary>
    private bool isConnecting;
    #endregion

    private void Awake()
    {
        //Dont want to automatically join into a lobby
        PhotonNetwork.autoJoinLobby = false;

        //this is so we can use PhotonNetwork.LoadLevel() on the master client
        //and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        //Change log level to desired
        PhotonNetwork.logLevel = logLevel;
    }

    // Use this for initialization
    void Start()
    {
        //Connect();
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    /// <summary>
    /// Start connection process.
    /// If we are connected, attempt to join random room
    /// If we are not connected, connect this application instance to Photon Network
    /// </summary>
    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        //player initiated a connection they want and should actually connect to a room
        isConnecting = true;
        //if we are connected, join a random room
        if (PhotonNetwork.connected)
        {
            //try to join a room, if this fails it will notify on callback OnPhotonRandomJoinFailed
            PhotonNetwork.JoinRandomRoom();
        }
        //otherwise we connect using our settings
        else
        {
            PhotonNetwork.ConnectUsingSettings(gameVersion);
        }
    }

    /// <summary>
    /// This is called when we connect to the photon network successfully.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Was connected to server by PUN");
        if (isConnecting)
        {
            //now we attempt to join existing room
            PhotonNetwork.JoinRandomRoom();
            //TODO: probably where we would instead put the option for a lobby room
            //then list avaiable games or ability to make new game
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("We were disconnected from PUN");
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    /// <summary>
    /// Called when all rooms are full or none available.
    /// Need to make a new room
    /// </summary>
    /// <param name="codeAndMsg"></param>
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("Failed to join random room, creating a new one");
        //creation of a new room
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Success in joining a room.");
        //if we are the only player we need to load the first arena
        if(PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("Loading first arena map.");

            //StartCoroutine(LoadOurScene("RoomFor1")); //Could do this for asynchronous loading like load screen
            PhotonNetwork.LoadLevel("RoomFor1"); //or this for syncronous loading
        }
    }

    private IEnumerator LoadOurScene(string sceneName)
    {
        AsyncOperation asyncOperation = PhotonNetwork.LoadLevelAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
