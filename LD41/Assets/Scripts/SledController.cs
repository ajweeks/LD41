using UnityEngine;
using UnityEngine.UI;

public class SledController : MonoBehaviour {

    public float TurnSpeed = 60.0f;
    public float DownhillSpeed = 500.0f;
    public float RollStrength = 10.0f; // How much to roll by when turning (rotation around z)
    public float FloatingHeight = 1.0f;
    public float SledAngleResistanceScale = 10.0f; // How much slower to go when facing sideways
    public float ReturnToVerticalForce = 10.0f;

    public SledFollowCam cam;

    private Rigidbody rb;

    public Image GameOverPanel;
    public Text FinalTimeText;

    private Text DebugText;


    private float startTime = 0.0f;
    private float endTime = 0.0f;

    [HideInInspector]
    public bool roundOver = false;

    private Quaternion startingRotation;

    void Start () {
        rb = GetComponent<Rigidbody>();
        DebugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<Text>();
    }
	
	void FixedUpdate () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float raycastLength = 1.0f;

        Debug.DrawLine(transform.position, transform.position - transform.up * raycastLength, Color.red);

        bool grounded = false;
        float distToGround = 10000.0f;
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, raycastLength))
        {
             distToGround = hitInfo.distance;
            grounded = true;
        }
        DebugText.text = "grounded: " + grounded;

        if (grounded && startingRotation == Quaternion.identity)
        {
            startingRotation = transform.localRotation;
            Debug.Log(startingRotation);
        }

        Vector3 force = Vector3.zero;

        transform.Rotate(transform.up, horizontal * Time.fixedDeltaTime * TurnSpeed);

        // Left-right angle
        float yaw = transform.localRotation.eulerAngles.y;
        if (yaw > 180.0f)
        {
            yaw = yaw - 360.0f;
        }
        // 0 facing forwards, +/-1 facing right/left
        float turnPercent = yaw / 100.0f; // Hopefully player doesn't turn more than 100 deg
        turnPercent = Mathf.Clamp(turnPercent, -1.0f, 1.0f);

        force += transform.forward * vertical * DownhillSpeed * Time.fixedDeltaTime;

        //Vector3 rotationEuler = transform.localRotation.eulerAngles;

        //transform.localRotation = Quaternion.Euler(rotationEuler);

        // We're airborne!
        if (!grounded)
        {
            // Value between 0 and 1 specifying up force
            float returnToVerticalAmount = ReturnToVerticalForce * Time.fixedDeltaTime;
            Quaternion targetRotation = startingRotation;
            Quaternion rotBefore = transform.localRotation;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, returnToVerticalAmount);

            Debug.Log("diff: " + (transform.localRotation.eulerAngles - rotBefore.eulerAngles));
        }


        Vector3 camForward = cam.transform.forward;

        // How much to resist downward movement based on sled angle
        float resistanceForce = turnPercent;
        force += transform.right * resistanceForce * SledAngleResistanceScale;
        //float invResistanceForce = 1.0f - Mathf.Abs(turnPercent);
        //force += transform.forward * resistanceForce * SledAngleResistanceScale;

        rb.AddForce(force);

        Vector3 offset = new Vector3(0, 1, 0);
        Debug.DrawLine(transform.position + offset, transform.position + force * 10.0f + offset, Color.yellow);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("finish"))
        {
            roundOver = true;
            endTime = Time.time;

            GameOverPanel.gameObject.SetActive(true);
            FinalTimeText.text = "Final time: " + endTime.ToString();
        }
    }
}
