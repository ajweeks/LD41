using UnityEngine;

public class SledFollowCam : MonoBehaviour {

    public SledController sled;

    public float FollowSpeed = 1.1f;
    public float ViewVerticalOffset = 5.0f;

    private float maxDist = 30.0f;
    private Vector3 playerOffset;

    void Start () {
        playerOffset = transform.position - sled.transform.position;

    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
        transform.position,
        sled.transform.position + playerOffset, // + -vel * VelocityScale * Time.fixedDeltaTime,
        Time.fixedDeltaTime * 65.0f); // Don't allow sudden movements

        Vector3 dPos = sled.transform.position - transform.position;
        if (dPos.sqrMagnitude > maxDist * maxDist)
        {
            transform.position = sled.transform.position - dPos.normalized * maxDist;
        }

        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        Quaternion.LookRotation(
            (sled.transform.position + new Vector3(0, ViewVerticalOffset, 0)) - transform.position,
            Vector3.up),
            Time.deltaTime * FollowSpeed);
    }
}
