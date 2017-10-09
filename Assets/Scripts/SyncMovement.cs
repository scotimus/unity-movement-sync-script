using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncMovement : NetworkBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private float lerpTime;
    public float tickRate = 30; //sets how many times per second updates should be sent


    void Start()
    {
        lerpTime = 0;

        if (isLocalPlayer)
        {
            StartCoroutine(_transmitLoop());
        }

    }

    void Update()
    {
        //increment lerpTime using the time since the last frame
        lerpTime = Mathf.Clamp(lerpTime += (Time.deltaTime * tickRate), 0, 1);

        //then we use lerp (linear interpolation) to smoothly move player between lastPosition and the target
        if (!isLocalPlayer)
        {
            transform.position = Vector3.Lerp(lastPosition, targetPosition, lerpTime);
            transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, lerpTime);
        }
    }

    private IEnumerator _transmitLoop()
    {
        //loop will run on the local player to trigger updates at intervals specified by the tick rate 
        while (true)
        {
            //only send updates if the player has moved to conserve bandwidth
            if (targetPosition != transform.position || targetRotation != transform.rotation)
            {
                //the local player will send their position to the server
                CmdSendPostionToServer(transform.position, transform.rotation);
            }

            yield return new WaitForSeconds(1 / tickRate);
        }
    }


    [Command]
    void CmdSendPostionToServer(Vector3 pos, Quaternion rot)
    {
        //on recieving an update, the server will update all clients via RPC
        RpcUpdateClients(pos, rot);
    }

    [ClientRpc]
    void RpcUpdateClients(Vector3 pos, Quaternion rot)
    {
        //the previous target position becomes the new last position
        lastPosition = targetPosition;
        lastRotation = targetRotation;

        //then we set the new targets
        targetPosition = pos;
        targetRotation = rot;

        //and reset the lerp timer
        lerpTime = 0;
    }


}