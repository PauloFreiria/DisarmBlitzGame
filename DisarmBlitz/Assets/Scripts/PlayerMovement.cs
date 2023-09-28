using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;


    [Header("Player Movement")]
    [SerializeField] private float speed;
    [SerializeField] private GameObject dashStun;
    [SerializeField] private Rigidbody rigidBody;
    private float maxCollisionSpeed = 13.70f;
    private float normalSpeed;
    private bool canDash = true;

    [Header("Player UI")]
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject useButton;
    [SerializeField] private GameObject interactButton;
    [SerializeField] private GameObject dashButton;
    [SerializeField] private GameObject disarmScreen;

    [Header("Desarms")]
    [SerializeField] private GameObject wordDesarm;
    [SerializeField] private GameObject numberDesarm;
    [SerializeField] private GameObject blind;
    [SerializeField] private int QTE;
    private bool desarmNum;
    private bool desarmWord;
    private bool bearTrap;

    private bool escapingBearTrap;
    private int escapeButtonPresses = 0;
    private int requiredEscapePresses = 5;

    private Vector2 myInput;
    private Transform playerTransform;

    private bool isWordDisarmed = false;
    private bool isNumberDisarmed = false;

    AudioManager audioManager;
    [SerializeField] private GameObject timer;

    [SerializeField] private GameObject endMissionText;
    [SerializeField] private GameObject missionText;
    [SerializeField] private GameObject buttonsText;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        playerTransform = GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody>();
        normalSpeed = speed;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioManager.PlaySFX(audioManager.startGame);
    }

    void Start()
    {
        StartCoroutine(MissionText());
    }

    public void HandleInput(InputAction.CallbackContext value)
    {
        myInput = value.ReadValue<Vector2>();
    }

    public void Interact(InputAction.CallbackContext value)
    {
        if (value.performed && desarmNum == true && isNumberDisarmed == false)
        {
            audioManager.PlaySFX(audioManager.disarmBegin);
            numberDesarm.gameObject.SetActive(true);
            joystick.gameObject.SetActive(false);
            useButton.gameObject.SetActive(false);
            interactButton.gameObject.SetActive(false);
            dashButton.gameObject.SetActive(false);
            desarmNum = false;
        }

        else if (value.performed && desarmWord == true && isWordDisarmed == false)
        {
            audioManager.PlaySFX(audioManager.disarmBegin);
            wordDesarm.gameObject.SetActive(true);
            joystick.gameObject.SetActive(false);
            useButton.gameObject.SetActive(false);
            interactButton.gameObject.SetActive(false);
            dashButton.gameObject.SetActive(false);
            desarmWord = false;
        }
    }

    public void Use(InputAction.CallbackContext value)
    {
        if (value.performed && bearTrap && !escapingBearTrap)
        {
            escapingBearTrap = true;
        }

        if (value.canceled && escapingBearTrap)
        {
            escapeButtonPresses++;
            print(escapeButtonPresses);

            if (escapeButtonPresses >= requiredEscapePresses)
            {
                bearTrap = false;
                escapingBearTrap = false;
                speed = normalSpeed;
            }
        }
    }

    public void CallDash()
    {
        if (canDash == true)
        {
            StartCoroutine(Dash());

        }
    }

    private void Update()
    {
        MovePlayer();

        if (isNumberDisarmed && isWordDisarmed)
        {
            endMissionText.gameObject.SetActive(true);
        }
    }

    void MovePlayer()
    {
        // Get input from the gamepad
        Vector3 gamepadInput = new Vector3(myInput.x, 0, myInput.y);

        // Get input from the keyboard
        Vector3 keyboardInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Combine the inputs to determine the movement direction
        Vector3 movementInput = gamepadInput + keyboardInput;

        // Normalize the input to ensure consistent speed in all directions
        movementInput = movementInput.normalized;

        // Calculate the desired velocity
        Vector3 desiredVelocity = movementInput * speed;

        // Apply the desired velocity to the Rigidbody
        rigidBody.velocity = new Vector3(desiredVelocity.x, rigidBody.velocity.y, desiredVelocity.z);
    }

    private void StepSound()
    {
        audioManager.PlaySFX(audioManager.steps);
    }

    //DESARMS
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "NumberDesarm")
        {
            desarmNum = true;
        }

        else if (collision.gameObject.tag == "WordDesarm")
        {
            desarmWord = true;
        }

        else if (collision.gameObject.tag == "Blind")
        {
            audioManager.PlaySFX(audioManager.blind);
            StartCoroutine(Blind());
        }

        else if (collision.gameObject.tag == "BearTrap")
        {
            bearTrap = true;
            speed = 0;
            audioManager.PlaySFX(audioManager.trap);

        }

        else if (collision.gameObject.tag == "HoneySlow")
        {
            speed = normalSpeed / 2;
            audioManager.PlaySFX(audioManager.slow);

        }

        else if (collision.gameObject.CompareTag("FinalDisarm") && isWordDisarmed && isNumberDisarmed)
        {
            disarmScreen.gameObject.SetActive(true);
            audioManager.StopMusic();
            audioManager.PlaySFX(audioManager.winGame);
            timer.gameObject.SetActive(false);
            endMissionText.gameObject.SetActive(false);
        }

        // Verifique se a velocidade é maior que o limite.
        if (rigidBody.velocity.magnitude > maxCollisionSpeed)
        {
            // Calcule a força contrária necessária para reduzir a velocidade.
            Vector3 oppositeForce = -rigidBody.velocity.normalized * (rigidBody.velocity.magnitude - maxCollisionSpeed);

            // Aplique a força contrária ao Rigidbody do personagem.
            rigidBody.AddForce(oppositeForce, ForceMode.VelocityChange);
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "NumberDesarm")
        {
            desarmNum = false;
        }

        else if (collision.gameObject.tag == "WordDesarm")
        {
            desarmWord = false;
        }

        if (collision.gameObject.tag == "HoneySlow")
        {
            speed = normalSpeed;
        }
    }

    IEnumerator Dash()
    {
        audioManager.PlaySFX(audioManager.dash);
        dashStun.SetActive(true);
        canDash = false;
        speed *= 2f;
        yield return new WaitForSeconds(.6f);
        speed = normalSpeed;
        dashStun.SetActive(false);
        yield return new WaitForSeconds(5f);
        canDash = true;
    }

    //SABOTAGENS
    IEnumerator DashStun()
    {
        speed = 0;
        yield return new WaitForSeconds(2f);
        speed = normalSpeed;
    }

    IEnumerator Blind()
    {
        blind.SetActive(true);
        yield return new WaitForSeconds(2f);
        blind.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DashStun")
        {
            audioManager.PlaySFX(audioManager.dashCollision);
            StartCoroutine(DashStun());
        }
    }

    IEnumerator MissionText()
    {
        yield return new WaitForSeconds(1f);
        missionText.SetActive(true);
        yield return new WaitForSeconds(5f);
        missionText.SetActive(false);
    }
    
    IEnumerator ButtonsText()
    {
        yield return new WaitForSeconds(5.5f);
        buttonsText.SetActive(true);
        yield return new WaitForSeconds(5f);
        buttonsText.SetActive(false);
    }




    public void SetNumberDisarm(bool isDisarmed)
    {
        isNumberDisarmed = isDisarmed;
    }

    public void SetWordDisarm(bool isDisarmed)
    {
        isWordDisarmed = isDisarmed;
    }

}
