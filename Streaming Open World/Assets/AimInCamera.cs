using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimInCamera : MonoBehaviour
{
    private vThirdPersonCamera thirdPersonCamera;
    private const int LeftClick = 1;

    private void Awake()
    {
        thirdPersonCamera = FindObjectOfType<vThirdPersonCamera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(LeftClick))
        {
            //thirdPersonCamera.rightOffset = 0.4f;
            //thirdPersonCamera.defaultDistance = 1.24f;
            thirdPersonCamera.rightOffset = 1.06f;
            thirdPersonCamera.defaultDistance = 1.62f;
            
            // Activate crosshair
            // play idle or walking anim
        }
        else if (Input.GetMouseButtonUp(LeftClick))
        {
            thirdPersonCamera.rightOffset = 0.36f;
            thirdPersonCamera.defaultDistance = 2.87f;
        }
    }
}
