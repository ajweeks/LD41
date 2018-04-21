using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public PlayerController pc;

    // How quickly to rotate the camera towards the player
    public float FollowSpeed = 1.1f;

    // How much above the player to look at
    public float ViewVerticalOffset = 5.0f;
    public float VelocityScale = 1.0f;

    private Vector3 pcOffset;

    private Vector3 vel;

	void Start () {
        pcOffset = transform.position - pc.transform.position;
    }
	
	void FixedUpdate () {

        vel = pc.rb.velocity;

        transform.position = pc.transform.position + pcOffset + -vel * VelocityScale * Time.fixedDeltaTime;

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation((pc.transform.position + new Vector3(0, ViewVerticalOffset, 0)) - transform.position, Vector3.up),
            Time.fixedDeltaTime * FollowSpeed);
	}
}
