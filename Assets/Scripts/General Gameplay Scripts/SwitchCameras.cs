using Unity.Cinemachine;
using UnityEngine;

public class SwitchCameras : MonoBehaviour
{
    [SerializeField] private CinemachineCamera exploreCamera;
    [SerializeField] private CinemachineCamera aimCamera;

    [SerializeField] private PlayerController playerController;

    void OnEnable()
    {
        playerController.OnStateUpdated += SwitchCamera;
    }

    void OnDisable()
    {
        playerController.OnStateUpdated -= SwitchCamera;
    }

    private void SwitchCamera(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.EXPLORE:
                //Do this
                exploreCamera.Prioritize();
                break;
            case PlayerState.AIM:
                //Otherwise do this
                aimCamera.Prioritize();
                break;
            default:
                //Nothing to do here
                break;
        }
    }
}
