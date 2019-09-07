using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCamera : MonoBehaviour
{
    private GameObject MC_FindPlayer;
    private Vector2 MC_PlayerStartingPosition;
    private bool MC_StartMoving;
    private Vector3 MC_Offset = new Vector3(0.5f,0.0f,0.0f);
    private Vector3 MC_SmoothSpeed = new Vector3(0.125f,0.0f,0.0f);
    public static bool MC_FollowingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (MC_FindPlayer = GameObject.Find("Player"))
        {
            
            //gameObject.transform.position = new Vector3(MC_FindPlayer.transform.position.x , MC_FindPlayer.transform.position.y +0.5f, gameObject.transform.position.z);
            MC_PlayerStartingPosition = gameObject.transform.position;
        }
        else
        {
            Debug.Log("Camera could not find player");
        }
        MC_FollowingPlayer = true;
        MC_StartMoving = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (MC_FollowingPlayer)
        {
            gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, new Vector3(MC_FindPlayer.gameObject.transform.position.x + MC_Offset.x,gameObject.transform.position.y,gameObject.transform.position.
                z),ref MC_SmoothSpeed, 0.5f);

        }

    }
}