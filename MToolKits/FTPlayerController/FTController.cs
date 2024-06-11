using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTController : MonoBehaviour
{
    [Header("----------------------角色----------------------------")]
    public Transform playerCamera;
    CharacterController controller;
    public float walkSpeed = 4;
    public float gravity = -19.8f;
    public float groundRadius = 0.1f;
    public float footOffset;
    public LayerMask groundMask;

    public float jumpHeight = 1.0f;

    [Tooltip("转身速度")]
    public float trunSpeed = 0.1f;
    private float smothValue;
    bool isGrounded = false;
    Vector3 velocity = Vector3.zero;
    float speed = 2;
   
    [Header("----------------------相机----------------------------")]
    [Tooltip("第1人称按键")]
    public KeyCode KeyOne;
    [Tooltip("第3人称按键")]
    public KeyCode KeyThird;

    [HideInInspector] public Vector3 cameraAngles = Vector3.zero;
    [HideInInspector] public Vector3 cameraOrigin = Vector3.zero;
    public bool holdMouse = true;

    public float cameraSensitivity = 1f;
    public Vector2 verticalRestraint = new Vector2(-90f, 90f);
    Vector2 thirdPersonClamp = new Vector2(-50f, 85f);

    public float mouseWheelSensitivity = 30;
    public float MinViewDistance = 2;
    public float MaxViewDistance = 10;

    [HideInInspector] public float cameraDistance = 5f;

    public float targetHeight = 1;
    float m_zoomLerp;
    float perspective = 0f;
    RaycastHit m_hit;
    public bool rigidbodyOcclusion = true;
    public LayerMask cameraOccluders = ~0;
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        speed = walkSpeed;
    }

   
    void Update()
    {
        Falling();
        Jumping();
        Moving();
        if (holdMouse)
        {
            if (Input.GetMouseButton(1))
            {
                RotateCamera();
            }
        }
        else
        {
            RotateCamera();
        }

        if (Input.GetKeyDown(KeyOne))
        {
            perspective = 0;
        }
        if (Input.GetKeyDown(KeyThird))
        {
            perspective = 1;
        }

        if (perspective == 1)
        {
            ZoomCamera();
        }
    }

    void Falling() {
        isGrounded = Physics.CheckSphere(transform.position - Vector3.up * footOffset, groundRadius, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
        velocity.y += gravity * Time.deltaTime;
    }
    void Jumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    void Moving()
    {

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        speed = walkSpeed;

        Vector3 screenRight = playerCamera.right;             //以屏幕为参考系移动
        Vector3 screenForward = playerCamera.forward;
        screenForward.y = 0;                            //不能有竖直分量

        Vector3 sumVector = screenForward * v + screenRight * h;                //矢量之和

        var dir = sumVector.normalized;

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smothValue, trunSpeed);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            controller.Move(sumVector * speed * Time.deltaTime);
        }

        controller.Move(velocity * Time.deltaTime);
    }
    private void LateUpdate()
    {
        MoveCamera();
    }

    private void FixedUpdate()
    {
        
    }
    void RotateCamera()
    {
        var X = Input.GetAxis("Mouse X");
        var Y = Input.GetAxis("Mouse Y");
        m_zoomLerp = RLerp(m_zoomLerp, 1f, 0.1f);
        cameraAngles += new Vector3(-Y * cameraSensitivity, X * cameraSensitivity);
        Vector2 clampGoal = Vector2.Lerp(verticalRestraint, thirdPersonClamp, perspective);

        cameraAngles.x = Mathf.Clamp(cameraAngles.x, clampGoal.x, clampGoal.y);

        playerCamera.rotation = Quaternion.Euler(cameraAngles);
    }

    void MoveCamera() {
        Vector3 m_camPos = Vector3.zero;

        Vector3 aimPos = transform.position + Vector3.up * targetHeight;
        var camRotation = Quaternion.Euler(cameraAngles.x, cameraAngles.y, 0f);
        var lerp = Vector3.Lerp(new Vector3(0.5f, 0.5f, -cameraDistance * 0.5f), new Vector3(0f, 0f, -cameraDistance), m_zoomLerp);
        m_camPos = aimPos + camRotation * lerp;
        var maxDistance = Mathf.Lerp(new Vector3(0.5f, 0.5f, -cameraDistance * 0.5f).magnitude, cameraDistance, m_zoomLerp);
        if (Physics.SphereCast(aimPos, 0.15f, camRotation * lerp.normalized, out m_hit, maxDistance, cameraOccluders, QueryTriggerInteraction.Ignore))
        {

            if (rigidbodyOcclusion)
            {
                m_camPos = m_hit.point + m_hit.normal * 0.2f;
            }
            else
            {
                if (m_hit.collider.GetComponent<Rigidbody>() != null)
                {
                    if (m_hit.collider.GetComponent<Rigidbody>().isKinematic)
                        m_camPos = m_hit.point + m_hit.normal * 0.2f;
                }
                else
                    m_camPos = m_hit.point + m_hit.normal * 0.2f;
            }

        }
        playerCamera.position = Vector3.Lerp(transform.position, m_camPos, perspective);
    }

    void ZoomCamera() {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) return;
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * mouseWheelSensitivity;
        cameraDistance = Mathf.Clamp(cameraDistance, MinViewDistance, MaxViewDistance);
    }
    public float RLerp(float a, float b, float t)
    {
        t = 1f - Mathf.Pow(1f - t, Time.unscaledDeltaTime * 60f);

        return (Mathf.Abs(a - b) > 0.001f) ? (a + (b - a) * t) : b;
    }
}
