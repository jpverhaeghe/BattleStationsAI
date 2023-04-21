using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBot : MonoBehaviour
{
    // constant values used by this script
    private const float BOT_Y_OFFSET = 20;

    // private variable used only by this script
    private GameObject botToFollow;

    /// <summary>
    /// Update is called once per frame to update the camera's position so it is over the current bot it is following
    /// TODO: Will need to add a transition so it smoothly moves to another bot (lerp)
    /// </summary>
    void Update()
    {
        // change the position of this camera based on the bot it is currently following
        Vector3 newPosition = botToFollow.transform.position;
        newPosition.y += BOT_Y_OFFSET;
        transform.position = newPosition;
    }

    /// <summary>
    /// Sets the bot to follow for this camera
    /// </summary>
    /// <param name="botToFollow"></param>
    public void SetBotToFollow(GameObject botToFollow)
    {
        this.botToFollow = botToFollow;

    } // end SetBotToFollow
}
