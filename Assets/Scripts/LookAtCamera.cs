using Cinemachine;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public static LookAtCamera Instance { get; private set; }
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        Instance = this;
    }

    public void SetPosition(Transform pivot, Transform lookAt)
    {
        cam.transform.position = pivot.position;
        cam.transform.LookAt(lookAt);
    }
}
