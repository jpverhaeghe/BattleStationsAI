using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // constant values used by this script
    private const float ZOOM_LEFT_CLAMP = (38 - 100);
    private const float ZOOM_RIGHT_CLAMP = (100 - 38);
    private const float ZOOM_IN_CLAMP = 30;
    private const float ZOOM_OUT_CLAMP = 90;
    private const float ZOOM_DOWN_CLAMP = (12 - 70);
    private const float ZOOM_UP_CLAMP = (70 - 12);
    private const float BOT_Y_OFFSET = 20;

    // private variable used only by this script
    private GameObject botToFollow;
    private float moveSpeed = 10f;

    /// <summary>
    /// Update is called once per frame to update the camera's position so it is over the current bot it is following
    /// TODO: Will need to add a transition so it smoothly moves to another bot (lerp)
    /// </summary>
    void Update()
    {
        Vector3 newPosition;

        // If following a bot: change the position of this camera based on the bot it is currently following
        if (botToFollow != null)
        {
            newPosition = botToFollow.transform.position;
            newPosition.y += BOT_Y_OFFSET;
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            float zoomInput = Input.GetAxis("Zoom");
            
            // Allow for zooming in, but clamp to no closer than what would be a bot distance and probably no higher than 100?
            newPosition = transform.position;
            newPosition += new Vector3(horizontalInput, zoomInput, verticalInput) * moveSpeed * Time.deltaTime;

            // keep the camera in a specific viewport so we don't lose sight of the ships
            newPosition.x = Mathf.Clamp(newPosition.x, ZOOM_LEFT_CLAMP, ZOOM_RIGHT_CLAMP);
            newPosition.y = Mathf.Clamp(newPosition.y, ZOOM_IN_CLAMP, ZOOM_OUT_CLAMP);
            newPosition.z = Mathf.Clamp(newPosition.z, ZOOM_DOWN_CLAMP, ZOOM_UP_CLAMP);
        }
            
        transform.position = newPosition;

    } // end Update

    /// <summary>
    /// Sets the bot to follow for this camera
    /// </summary>
    /// <param name="botToFollow"></param>
    public void SetBotToFollow(GameObject botToFollow)
    {
        this.botToFollow = botToFollow;

    } // end SetBotToFollow

    /// <summary>
    /// Stops follwing a bot and returns camera to user controls
    /// </summary>
    public void RemoveFollowBot()
    {
        botToFollow = null;

    } // RemoveFollowBot
}
