using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player name input field. Let the user input their name.
/// Appears above player in game.
/// </summary>
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{

    private const string playerUsernamePrefKey = "PlayerUsername";
    // Use this for initialization
    void Start()
    {
        string defaultName = "Your username here";
        InputField input = this.GetComponent<InputField>();
        if(input != null)
        {
            if (PlayerPrefs.HasKey(playerUsernamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerUsernamePrefKey);
                input.text = defaultName;
            }
        }

        PhotonNetwork.playerName = defaultName;
    }

    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs
    /// </summary>
    /// <param name="name">The name of the player</param>
    public void SetPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("name shouldn't be null");
        }
        PhotonNetwork.playerName = name + " "; //trailing space forces name to update

        PlayerPrefs.SetString(playerUsernamePrefKey, name);
    }
}
