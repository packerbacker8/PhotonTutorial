using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    [Tooltip("UI text to display player's name")]
    public Text usernameText;
    [Tooltip("UI slider to displayer players health")]
    public Slider playerhealthSlider;
    [Tooltip("Pixel offset from the player target")]
    public Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    private PlayerManager target;
    private float characterControllerHeight = 0f;
    private Transform targetTransform;
    private Vector3 targetPosition;

    public void SetTarget(PlayerManager target)
    {
        if(target == null)
        {
            Debug.Log("Sent target was null");
            return;
        }
        this.target = target;
        CharacterController charController = this.target.GetComponent<CharacterController>();
        if(charController != null)
        {
            this.characterControllerHeight = charController.height;
        }
        if(usernameText != null)
        {
            usernameText.text = this.target.photonView.owner.NickName;
        }
    }

    private void Awake()
    {
        this.transform.parent = GameObject.Find("Canvas").transform;
    }

    private void Update()
    {
        if(playerhealthSlider != null)
        {
            playerhealthSlider.value = this.target.health;
        }
        if(target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void LateUpdate()
    {
        if(targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
}
