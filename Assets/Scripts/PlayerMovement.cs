using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    //just quick movement script for demo purposes
    //nothing interesting here

    public float maxSpeed;
    private float currentSpeed;

    public void Update()
    {
        if (isLocalPlayer)
        {
            transform.Translate(currentSpeed * Time.deltaTime, 0, 0);
            currentSpeed = Mathf.Clamp(currentSpeed - (Time.deltaTime * 5), 0, maxSpeed);

            accelerate(Input.GetAxis("Vertical"));
            turn(Input.GetAxis("Horizontal"));
        }
    }

    public void accelerate(float axis)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + (Mathf.Clamp(axis, 0, 1) * Time.deltaTime * 20), 0, maxSpeed);
    }


    public void turn(float axis)
    {
        transform.Rotate(0, (axis * 100 * Time.deltaTime), 0);
    }
}
