using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class PlayerController : MonoBehaviour
{
    #region Variables
    private Camera m_Camera;

    //Movement
    private Rigidbody rb;
    private float m_horizontalInput, m_verticalInput, velocity;

    public bool showCharacterSettings;
    [ConditionalField("showCharacterSettings")]
    [SerializeField]
    private float normalSpeed, runSpeed, sideSpeed, CrouchSpeed, jumpForce;

    [ConditionalField("showCharacterSettings")]
    [SerializeField]
    private AnimationCurve staminaReduceEffect;

    [ConditionalField("showCharacterSettings")]
    [SerializeField]
    private Vector3 checkGroundBoxSize;

    [ConditionalField("showCharacterSettings")]
    [SerializeField]
    private LayerMask lm_Environment;

    private GameObject gunHolder;
    private bool isGrounded;
    float pit;


    #endregion

    void Start()
    {
        m_Camera = Camera.main;
        rb = GetComponent<Rigidbody>();
        gunHolder = GameObject.Find("FPSController/FirstPersonCharacter/GunHolder");
        velocity = normalSpeed;
        feet.pitch = 0;

        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = m_Camera.transform.localRotation;
    }

    void Update()
    {
        if (!ScreenController.isPaused)
        {
            RotateView();
            Movement();
        }
        else
        {
            feet.pitch = 0;
            GetComponent<AudioSource>().pitch = 0;
        }
    }

    private void Movement()
    {
        if (rb.IsSleeping())
            rb.WakeUp();

        m_verticalInput = Input.GetAxisRaw("Vertical") * staminaReduceEffect.Evaluate(SaveManager.Instance.state.stamina) / 100;
        m_horizontalInput = Input.GetAxisRaw("Horizontal") * staminaReduceEffect.Evaluate(SaveManager.Instance.state.stamina) / 100;

        if (Mathf.Floor(rb.velocity.y) == 0 || Mathf.Floor(rb.velocity.y) == -1)
            rb.AddRelativeForce(m_horizontalInput * sideSpeed * Time.deltaTime, 0f, m_verticalInput * velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddRelativeForce(Vector3.up * jumpForce * staminaReduceEffect.Evaluate(SaveManager.Instance.state.stamina) / 100, ForceMode.Impulse);
            
        }

        if (!inWater)
        {
            if (Input.GetButton("Run"))
                velocity = runSpeed;
            else
                velocity = normalSpeed;
        }
        else
        {
            velocity = normalSpeed * 0.8f;
        }

        if (isGrounded && feet.clip == walkingSound)
        {
            if (rb.velocity.magnitude > 0.3f && rb.velocity.magnitude <= 5f)
            {
                feet.pitch = 0.8f;
                pit = 1f;
            }
            if (rb.velocity.magnitude > 5f)
            {
                feet.pitch = 1.2f;
                pit = 1f;
            }
            if (rb.velocity.magnitude <= 0.3f)
            {
                pit -= 0.001f;
                feet.pitch = 0;
            }
        }

        if (inWater)
            feet.pitch = 0;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            if (!feet.isPlaying)
            {
                feet.loop = true;
                feet.clip = walkingSound;
                feet.Play();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        feet.loop = false;
        StartCoroutine("SoundEco");
        isGrounded = false;
    }

    IEnumerator SoundEco()
    {
        yield return new WaitForSeconds(0.1f);
        feet.clip = landingJump;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground") && !isGrounded)
        {
            feet.pitch = 1;
            feet.Play();
        }
    }

    public bool soundVariables;
    [ConditionalField("soundVariables")]
    [SerializeField]
    AudioClip enteringWater, walkingWater, landingJump, walkingSound;
    [ConditionalField("soundVariables")]
    [SerializeField]
    AudioSource feet;
    [ConditionalField("soundVariables")]
    [SerializeField]
    GameObject waterSplashEffect;

    bool inWater;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            inWater = true;
            if (rb.velocity.magnitude >= 1)
            {
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().pitch = 1;
                    GetComponent<AudioSource>().clip = walkingWater;
                    GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            GetComponent<AudioSource>().pitch = 1;
            GetComponent<AudioSource>().clip = enteringWater;
            Instantiate(waterSplashEffect, other.ClosestPoint(transform.position), Quaternion.identity);
            GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        inWater = false;
        if (other.gameObject.CompareTag("Water"))
        {
            GetComponent<AudioSource>().pitch = -1;
            GetComponent<AudioSource>().clip = enteringWater;
            GetComponent<AudioSource>().Play();
        }
    }

    #region RotateView
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    public bool showMouseSettings;

    [ConditionalField("showMouseSettings")] public float XSensitivity = 2f;
    [ConditionalField("showMouseSettings")] public float YSensitivity = 2f;
    [ConditionalField("showMouseSettings")] public float MinimumX = -80F;
    [ConditionalField("showMouseSettings")] public float MaximumX = 80F;

    private void RotateView()
    {
        float yRot = Input.GetAxis("Mouse X") * XSensitivity * staminaReduceEffect.Evaluate(SaveManager.Instance.state.stamina) / 100;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity * staminaReduceEffect.Evaluate(SaveManager.Instance.state.stamina) / 100;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);


        transform.localRotation = m_CharacterTargetRot;
        m_Camera.transform.localRotation = m_CameraTargetRot;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
    #endregion
}
