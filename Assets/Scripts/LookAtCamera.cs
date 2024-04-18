using Cinemachine;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public static LookAtCamera Instance { get; private set; }

    StressReceiver stressReceiver;
    CinemachineFreeLook vCam;

    private void Awake()
    {
        Instance = this;
        stressReceiver = GetComponent<StressReceiver>();
        vCam = GetComponent<CinemachineFreeLook>();
    }

    public void SetPivot(Transform pivot, Transform lookAt)
    {
        vCam.Follow = pivot;
        vCam.LookAt = lookAt;
    }
    public void ShakeCamera()
    {
        stressReceiver.InduceStress(0.65f);
    }
}
