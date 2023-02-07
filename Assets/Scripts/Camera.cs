using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float cameraSpeed = 10;

    private GameObject[] taxis;
    private int followingTaxis = 0;
    private bool followingCar = false;

    private void Start()
    {
        taxis = GameObject.FindGameObjectsWithTag("Car");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)) 
        {
            transform.position += new Vector3(1, 0, 1) * Time.deltaTime * cameraSpeed;
            followingCar = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(-1, 0, -1) * Time.deltaTime * cameraSpeed;
            followingCar = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, -1) * Time.deltaTime * cameraSpeed;
            followingCar = false;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0, 1) * Time.deltaTime * cameraSpeed;
            followingCar = false;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            followingCar = true;

            if (followingTaxis < taxis.Length)
            {
                followingTaxis++;
            }else
            {
                followingTaxis = 1;
            }
        }

        if (followingCar)
        {
            Vector3 newPosition = taxis[followingTaxis - 1].transform.position;
            Vector3 offset = newPosition + new Vector3(-10, 0 , - 10);

            transform.position = new Vector3(offset.x, transform.position.y, offset.z);
        }
    }
}
