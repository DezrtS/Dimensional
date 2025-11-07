using UnityEngine;

public class SwingingLog : MonoBehaviour
{
    public enum SwingAxis { X, Z }

    [SerializeField] private SwingAxis swingAxis = SwingAxis.Z;
    [SerializeField] private float swingSpeed = 2f;
    [SerializeField] private float maxAngle = 30f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float angle = maxAngle * Mathf.Sin(Time.time * swingSpeed);
        Vector3 rotation = Vector3.zero;

        if (swingAxis == SwingAxis.X)
            rotation = new Vector3(angle, 0f, 0f);
        else if (swingAxis == SwingAxis.Z)
            rotation = new Vector3(0f, 0f, angle);

        transform.localRotation = startRotation * Quaternion.Euler(rotation);
    }
}
