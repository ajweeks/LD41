using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    // If true all player controls are disabled
    [HideInInspector]
    public bool roundOver = false;

    public float AirMoveScale = 0.1f;

    public float MaxForwardSpeed = 40.0f;

    public float TubeWidth = 20.0f;
    //public float TubeLength = 400.0f;
    public float TubeAngleDeg = 15.0f;

    public FollowCamera followCamera;

    public float SidewaysMoveForce = 11000.0f;
    public float ForwardMoveForce = 2000.0f;

    public float SlowDownSpeed = 30.0f;

    private float startTime = 0.0f;
    private float endTime = 0.0f;

    public Image GameOverPanel;
    public Text FinalTimeText;

    [HideInInspector]
    public Rigidbody rb;

    void Start () {
        GameOverPanel.gameObject.SetActive(false);

        rb = GetComponent<Rigidbody>();
        startTime = Time.time;
    }
	
	void FixedUpdate () {
        if (roundOver)
        {
            rb.AddForce(-rb.velocity * Time.fixedDeltaTime * SlowDownSpeed);
        }
        else
        {
            float tubeLocation = transform.position.x / TubeWidth;
            tubeLocation = Mathf.Clamp(tubeLocation, -1.0f, 1.0f);

            Vector3 tubeNormal = new Vector3(-tubeLocation, 1.0f - Mathf.Abs(tubeLocation), 0.0f).normalized;
            Vector3 tubeForward = new Vector3(
                0.0f, 
                -Mathf.Sin(Mathf.Deg2Rad * TubeAngleDeg), 
                Mathf.Cos(Mathf.Deg2Rad * TubeAngleDeg)).normalized;
            Vector3 tubeRight = Vector3.Cross(tubeNormal, tubeForward);

            tubeNormal = Vector3.Cross(tubeForward, tubeRight);

            Debug.DrawLine(transform.position, transform.position + tubeNormal * 10.0f, Color.green);
            Debug.DrawLine(transform.position, transform.position + tubeForward * 10.0f, Color.blue);
            Debug.DrawLine(transform.position, transform.position + tubeRight * 10.0f, Color.red);

            bool grounded = Physics.Raycast(transform.position, -tubeNormal, 4.0f);

            float moveScale = 1.0f;
            if (!grounded)
            {
                moveScale = AirMoveScale;
            }

            Vector3 force = new Vector3();

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 forward = tubeForward;
            Vector3 right = tubeRight;

            force += right * horizontal * SidewaysMoveForce * Time.fixedDeltaTime * moveScale;
            force += forward * vertical * ForwardMoveForce * Time.fixedDeltaTime * moveScale;

            rb.AddForce(force);

            if (Vector3.Dot(rb.velocity, forward) > MaxForwardSpeed)
            {
                Debug.Log(rb.velocity.magnitude);
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, MaxForwardSpeed);
                Debug.Log(rb.velocity.magnitude);
            }

            if (Mathf.Approximately(Mathf.Abs(tubeLocation), 1.0f))
            {
                // At edge of tube we might have residual velocity outwards which would cause us to 
                // fall out of pipe!
                rb.velocity = new Vector3(0.0f, rb.velocity.y, rb.velocity.z);
            }


            GameObject.FindGameObjectWithTag("DebugText").GetComponent<Text>().text = grounded ? "grounded: true" : "grounded: false";

            //Debug.DrawLine(transform.position, transform.position + forward * 10, Color.blue);
            //Debug.DrawLine(transform.position, transform.position + right * 10, Color.red);
        }
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
