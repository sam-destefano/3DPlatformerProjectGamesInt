using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveInDirection : MonoBehaviour
{
    [Tooltip("Applies a constant force in this direction.")]
    public Vector3 moveDirection;
    public bool scriptEnabled = true;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(scriptEnabled)
        {
            rigidbody.AddForce(moveDirection);
        }
    }
}
