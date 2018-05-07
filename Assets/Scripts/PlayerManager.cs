#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && ! UNITY_5_3) || UNITY_2017
#define UNITY_MIN_5_4
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Photon.PunBehaviour, IPunObservable
{
    public GameObject laserBeams;
    [Tooltip("The local player instance. Is the local player present in the scene?")]
    public static GameObject localPlayerInstance;
    public GameObject PlayerUIPrefab;
    [Tooltip("Current health of the player.")]
    public float health = 100f;

    private bool isFiring;

    private void Awake()
    {
        if (photonView.isMine)
        {
            localPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
        if(laserBeams == null)
        {
            Debug.LogError("<Color=Red><a>Missing laser beams.</a></Color>",this);
        }
        else
        {
            laserBeams.SetActive(false);
        }
    }

    private void Start()
    {
        CameraWork cam = this.GetComponent<CameraWork>();

        if(cam != null)
        {
            if (photonView.isMine)
            {
                cam.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing camera work</a></Color>", this);
        }

#if UNITY_MIN_5_4
        //subscribing to event that the scene was loaded
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loading) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif

        if(PlayerUIPrefab != null)
        {
            GameObject uiGameObj = Instantiate(PlayerUIPrefab) as GameObject;
            uiGameObj.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.Log("No player ui prefab");
        }
    }

#if !UNITY_MIN_5_4
  
    private void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }

#endif

    private void CalledOnLevelWasLoaded(int buildIndex)
    {
        //if nothing is below us
        if(!Physics.Raycast(transform.position, Vector3.down, 5f))
        {
            transform.position = new Vector3(0, 5, 0);
        }
        GameObject uiGameObj = Instantiate(PlayerUIPrefab) as GameObject;
        uiGameObj.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    /// <summary>
    /// Process the inputs of the player.
    /// </summary>
    private void ProcessInputs()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isFiring =  true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isFiring = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            ProcessInputs();
        }

        if (laserBeams != null && isFiring != laserBeams.activeInHierarchy)
        {
            laserBeams.SetActive(isFiring);
        }

        if(health <= 0f)
        {
            //we died so we rage quit
            //this also assume a game manager game object with script is in the scene
            GameMangerScript.Manager.LeaveRoom();
        }
    }

    /// <summary>
    /// Our eye beams are hitting another player. decrease health
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.isMine)
        {
            return;
        }
        //if this is true it is not an eyebeam we are colliding with
        if (!other.name.Contains("Beam"))
        {
            return;
        }

        health--;
    }

    /// <summary>
    /// Our eye beams are still damaging another player
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //do nothing if we are not local player
        if (!photonView.isMine)
        {
            return;
        }

        //if this is true it is not an eyebeam we are colliding with
        if (!other.name.Contains("Beam"))
        {
            return;
        }

        health -= 1f * Time.deltaTime; //multiply damage affect so it is consistent regardless of frame rate
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //we own this player, send data to other players
            stream.SendNext(isFiring);
            stream.SendNext(health);
        }
        else
        {
            //otherwise its a different player receive data
            this.isFiring = (bool)stream.ReceiveNext();
            this.health = (float)stream.ReceiveNext();
            //receive next just gives raw data, need to know the type when receiving
        }
    }
}
