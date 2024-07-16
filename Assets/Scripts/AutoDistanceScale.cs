using UnityEngine;

public class AutoDistanceScale : MonoBehaviour
{
    public float FixedSize = .005f;

    void Update()
    {
        var distance = (Camera.main.transform.position - transform.position).magnitude;
        var size = distance * FixedSize * Camera.main.fieldOfView;
        transform.localScale = Vector3.one * size;
    }
}