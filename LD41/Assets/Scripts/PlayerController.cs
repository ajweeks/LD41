using UnityEngine;

public class PlayerController : MonoBehaviour {

    public FollowCamera followCamera;

    public float SidewaysMoveForce = 10000.0f;
    public float ForwardMoveForce = 100.0f;

    [HideInInspector]
    public Rigidbody rb;

    void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	void FixedUpdate () {
        Vector3 force = new Vector3();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = followCamera.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        Vector3 right = followCamera.transform.right;

        force += right * horizontal * SidewaysMoveForce * Time.fixedDeltaTime;
        force += forward * vertical * ForwardMoveForce * Time.fixedDeltaTime;

        rb.AddForce(force);

        Debug.DrawLine(transform.position, transform.position + forward * 10, Color.blue);
        Debug.DrawLine(transform.position, transform.position + right * 10, Color.red);
    }
}
