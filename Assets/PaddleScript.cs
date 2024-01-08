using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public enum ControlType
{
    Mouse,
    Gamepad,
    AI
}
public enum Side
{
    Left,
    Right
}
public class PaddleScript : MonoBehaviour
{
    //how to fix this code to work with the new input system

    //Make the controltype derive it's value from playerprefs
    public ControlType ControlType;
    public float rotationSpeed = 120;
    public float maxSpeed = 9;
    public Rigidbody2D body;
    private Vector3 mousePosition;
    public float moveSpeed = 100;
    public InputAction rotate;
    public InputAction moveX;
    public InputAction moveY;
    public InputAction serve;
    public GameObject ball;
    public bool serving;



    public Side side;



    // Start is called before the first frame update
    void Start()
    {
        //set the control type to the value in playerprefs

        //movement = new Vector2(44, 22);

        body = GetComponent<Rigidbody2D>();

        Debug.Log(body.name);

    }
    private void OnEnable()
    {
        rotate.Enable();
        moveX.Enable();
        moveY.Enable();
        serve.Enable();
    }
    private void OnDisable()
    {
        rotate.Disable();
        moveX.Disable();
        moveY.Disable();
        serve.Disable();
    }
    //change control type

    private void FixedUpdate()
    {

        //if controltype is mouse
        //do that code for me
        switch (ControlType)
        {
            case ControlType.Mouse:
                HandleMouseControl();
                break;
            case ControlType.Gamepad:
                HandlePadControl();
                break;
            case ControlType.AI:
                HandleAIControl();
                break;
            default:
                break;
        }

    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rotate.ReadValue<float>());   
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        //movement = mousePosition;
        //transform.position = mousePosition;
    }

    public enum AIState
    {
        // Idle,
        Prepare,
        PrepareServe,
        WaitStrike,
        Strike
    }

    private Vector3 targetPosition;
    private Vector3 lookAt;
    private AIState aiState = AIState.Prepare;
    private float waitUntil;

    private Vector3 strikePosition;

    private float aiSpeed;

    bool MoveToTarget()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(transform.position, targetPosition, Mathf.Abs(aiSpeed) * Time.fixedDeltaTime));
            return false;
        }
        Vector3 direction = (transform.position - lookAt).normalized;
        float targetRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (Mathf.Abs(transform.rotation.eulerAngles.z - targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetRotation), rotationSpeed * Time.fixedDeltaTime);
            return false;
        }
        return true;
    }

    void Strike()
    {
        Vector3 difference = strikePosition - ball.transform.position;
        float flightTime = 0.5f;
        Vector3 targetBallVelocity = new Vector3(difference.x / flightTime, difference.y / flightTime + 0.5f * Physics2D.gravity.y * flightTime * flightTime, 0f);
        Vector3 initialBallVelocity = ball.GetComponent<Rigidbody2D>().velocity;
        Vector3 targetPaddleVelocity = targetBallVelocity - initialBallVelocity;
        aiSpeed = targetPaddleVelocity.magnitude;
        targetPosition = ball.transform.position;
    }

    void HandleAIControl()
    {
        Vector2 servePosition = new Vector2(14f, -3f);
        lookAt = strikePosition;

        switch (aiState)
        {
            case AIState.Prepare:
                // Serving
                if (ball.GetComponent<BallScript>().serving && serving)
                {
                    aiState = AIState.PrepareServe;
                    targetPosition = servePosition + new Vector2(2f, 1.5f);
                    break;
                }
                string lastHit = ball.GetComponent<BallScript>().hits.LastOrDefault();
                if (lastHit == "" || lastHit == "Player2" || lastHit == "Right" || ball.transform.position.x < 0 || ball.GetComponent<Rigidbody2D>().velocity.y >= 0)
                {
                    break;
                }
                Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

                // Initial conditions
                float initialVelocityY = rb.velocity.y;
                float gravity = Physics2D.gravity.y;
                float initialY = ball.transform.position.y;
                float targetY = -5f;

                // Calculate the time to reach targetY (y = -5)
                float discriminant = initialVelocityY * initialVelocityY - 2 * gravity * (initialY - targetY);
                float timeToReachTargetY = 0f;
                float velocityYAtTargetY = 0f;

                if (discriminant >= 0)
                {
                    float timeY1 = (-initialVelocityY + Mathf.Sqrt(discriminant)) / gravity;
                    float timeY2 = (-initialVelocityY - Mathf.Sqrt(discriminant)) / gravity;

                    // Select the positive time
                    timeToReachTargetY = Mathf.Max(timeY1, timeY2);

                    // Calculate the velocity at targetY
                    velocityYAtTargetY = initialVelocityY + gravity * timeToReachTargetY;
                }
                else
                {
                    break;
                }

                float positionXAtTargetY = ball.transform.position.x + rb.velocity.x * timeToReachTargetY;

                // Assuming the ball bounces and moves upwards
                // Using a fraction of the velocity (like -1.5 * velocityYAtTargetY) to calculate desiredY
                float yMultiplier = 1.5f;
                float desiredY = -5 + -yMultiplier * velocityYAtTargetY; // This should be positive if velocityYAtTargetY is negative

                Debug.Log("DesiredY: " + desiredY);
                Debug.Log("velocityYAtTargetY: " + velocityYAtTargetY);

                // Calculate the time to reach the desired y position
                // This time, we consider the motion starting from targetY to desiredY
                float timeToDesiredY = (Mathf.Sqrt(velocityYAtTargetY * velocityYAtTargetY - 2 * -gravity * Mathf.Abs(targetY - desiredY)) + velocityYAtTargetY) / -gravity;
                float xAtDesiredY = positionXAtTargetY + rb.velocity.x * timeToDesiredY;

                Debug.Log("Time to desired y: " + timeToDesiredY);
                Debug.Log("X position at desired y: " + xAtDesiredY);


                strikePosition = new Vector3(-13f, -4, 0f);
                targetPosition = new Vector3(xAtDesiredY + 1f, desiredY, 0f);
                waitUntil = Time.timeSinceLevelLoad + timeToDesiredY + timeToReachTargetY;
                aiState = AIState.WaitStrike;

                break;
            case AIState.PrepareServe:
                strikePosition = new Vector3(8f, -4f, 0f);
                aiSpeed = moveSpeed;
                if (MoveToTarget())
                {
                    ServeBall();
                    aiState = AIState.WaitStrike;
                    waitUntil = Time.timeSinceLevelLoad + Random.Range(0.3f, 0.6f);
                    // (13, -5), 22.73, 
                }
                break;
            case AIState.WaitStrike:
                MoveToTarget();
                if (Time.timeSinceLevelLoad > waitUntil)
                {
                    Strike();
                    aiState = AIState.Strike;
                }
                break;
            case AIState.Strike:
                if (MoveToTarget())
                {
                    aiState = AIState.Prepare;
                }
                break;

        }

    }
    void ServeBall()
    {
        //if (ball.GetComponent<BallScript>().serving)
        //{ ball.GetComponent<BallScript>().ServeBall(); }
        ball.GetComponent<BallScript>().ServeBall();

    }
    void HandlePadControl()
    {
        Vector2 joyStickPos = new Vector2(moveX.ReadValue<float>(), moveY.ReadValue<float>());
        if (joyStickPos != Vector2.zero)
        {
            // Move the paddle
            body.MovePosition(body.position + joyStickPos);
        }
        Debug.Log(joyStickPos);
        joyStickPos = joyStickPos * moveSpeed * Time.fixedDeltaTime;
        //restrict movement to left side of the screen if paddle is on the left side
        //restrict movement to right side of the screen if paddle is on the right side
        if ((body.position.x + joyStickPos.x > 0 && side == Side.Left) ||
        (body.position.x + joyStickPos.x < 0 && side == Side.Right))
        {
            joyStickPos.x = 0 - body.position.x;
        }
        //if serve button is pressed
        if (serve.ReadValue<float>() > 0 && ball.GetComponent<BallScript>().serving && serving)
        {
            ServeBall();

        }

        body.MovePosition(body.position + joyStickPos);

        Rotate(rotate.ReadValue<float>());
    }
    void HandleMouseControl()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 position = Vector2.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);
        body.MovePosition(mousePosition);
        //restrict movement if mouse is on the right side of the screen

        if (mousePosition.x > 0 && side == Side.Left || mousePosition.x < 0 && side == Side.Right)
        {
            body.MovePosition(new Vector2(0, mousePosition.y));
        }

        if (Input.GetKeyDown(KeyCode.Space) && ball.GetComponent<BallScript>().serving && serving)
        {

            ServeBall();

        }
        //
        if (Input.GetMouseButton(0))
        {
            Rotate(-1);
        }
        if (Input.GetMouseButton(1))
        {
            Rotate(1);
        }
    }


    public void Move(Vector2 direction)
    {

    }
    public void Rotate(float direction)
    {

        body.MoveRotation(body.rotation + -direction * rotationSpeed * Time.fixedDeltaTime);

    }


}
