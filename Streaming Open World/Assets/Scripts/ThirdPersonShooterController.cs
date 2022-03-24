using System;
using System.Collections;
using System.Collections.Generic;
using Chilli.Ai.Zombies;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform bloodSplatterHitEffect;
    [SerializeField] private Transform muzzleFlash;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private List<GameObject> bloodSplatterEffectsList = new List<GameObject>();
    private Animator animator;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }
        
        if (starterAssetsInputs.aim) // Right mouse click
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1.0f, Time.deltaTime * 10.0f));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20.0f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0.0f, Time.deltaTime * 10.0f));
        }

        if (starterAssetsInputs.shoot) // Left mouse click
        {
            if (hitTransform != null)
            {
                StartCoroutine(PlayMuzzleFlashEffect());
                
                if (hitTransform.GetComponent<Zombie>() != null)
                {
                    Transform effect = Instantiate(bloodSplatterHitEffect, mouseWorldPosition, hitTransform.rotation * new Quaternion(180,0,180,0));
                    bloodSplatterEffectsList.Add(effect.gameObject);
                }
            }
            
            starterAssetsInputs.shoot = false;
        }
    }

    private IEnumerator PlayMuzzleFlashEffect()
    {
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.gameObject.SetActive(false);
    }
}
