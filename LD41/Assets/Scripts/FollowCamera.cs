using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public PlayerController pc;

    // How quickly to rotate the camera towards the player
    public float FollowSpeed = 1.1f;

    // How much above the player to look at
    public float ViewVerticalOffset = 5.0f;
    public float VelocityScale = 1.0f;

    public float EndCamSpinSpeed = 500.0f;

    private Vector3 pcOffset;

    private Vector3 vel;

    private float maxDist = 30.0f;
    private float sqMaxDist;

	void Start () {
        pcOffset = transform.position - pc.transform.position;
        sqMaxDist = maxDist * maxDist;
    }
	
	void FixedUpdate () {
        vel = pc.rb.velocity;

        if (pc.roundOver)
        {
            transform.position = pc.transform.position + pcOffset + transform.right * EndCamSpinSpeed * Time.fixedDeltaTime;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                pc.transform.position + pcOffset + -vel * VelocityScale * Time.fixedDeltaTime,
                Time.fixedDeltaTime * 65.0f); // Don't allow sudden movements

            Vector3 dPos = pc.transform.position - transform.position;
            if (dPos.sqrMagnitude > sqMaxDist)
            {
                transform.position = pc.transform.position - dPos.normalized * maxDist;
            }
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation((pc.transform.position + new Vector3(0, ViewVerticalOffset, 0)) - transform.position, Vector3.up),
            Time.fixedDeltaTime * FollowSpeed);
	}
}
