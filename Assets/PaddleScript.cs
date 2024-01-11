using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public enum ControlType
{
    Mouse,
    Gamepad,
    AI,
    None
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

    public float tableBounciness = 0.7f;

    public float targetBallSpeed = 10f;
    public float directionEasing = 100f;

    public float aimHeight = -3f;


    public Side side;

    private float dragCoefficient; // Adjust as needed

    // Start is called before the first frame update
    void Start()
    {
        //set the control type to the value in playerprefs

        //movement = new Vector2(44, 22);

        body = GetComponent<Rigidbody2D>();

        Debug.Log(body.name);

        dragCoefficient = ball.GetComponent<Rigidbody2D>().drag;

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

    private void Update()
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
            case ControlType.None:
                break;
            default:
                break;
        }

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

    void MoveToTarget()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(transform.position, targetPosition, Mathf.Abs(aiSpeed) * Time.fixedDeltaTime));
        }
        Vector3 direction = (transform.position - lookAt).normalized;
        float targetRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (side == Side.Right ? 0 : 180);
        if (Mathf.Abs(transform.rotation.eulerAngles.z - targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetRotation), rotationSpeed * Time.fixedDeltaTime);
        }
    }

    bool IsAtTarget()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            return false;
        }
        Vector3 direction = (transform.position - lookAt).normalized;
        float targetRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (side == Side.Right ? 0 : 180);
        return Mathf.Abs(transform.rotation.eulerAngles.z - targetRotation) < 0.1f;
    }

    void SetTarget(Vector3 target)
    {
        if (side == Side.Right && (target.x < 0 || target.x > 17f || target.y < -5f || target.y > 10f))
        {
            return;
        }
        if (side == Side.Left && (target.x > 0 || target.x < -17f || target.y < -5f || target.y > 10f))
        {
            return;
        }
        targetPosition = target;
    }

    void Strike()
    {
        Vector3 difference = strikePosition - ball.transform.position;
        float magnitude = difference.magnitude;
        float flightTime = magnitude / targetBallSpeed;

        // Apply a basic drag adjustment to the target velocity
        Vector3 targetBallVelocity = new Vector3(difference.x / flightTime, difference.y / flightTime + 0.5f * Physics2D.gravity.y * flightTime * flightTime, 0f) * (1 - dragCoefficient);

        Vector3 initialBallVelocity = ball.GetComponent<Rigidbody2D>().velocity;
        Vector3 targetPaddleVelocity = targetBallVelocity - initialBallVelocity;

        aiSpeed = targetPaddleVelocity.magnitude;
        SetTarget(ball.transform.position);
    }


    float CalculateTimeWithDrag(float initialVelocity, float acceleration, float distance, float dragCoefficient)
    {
        float deltaTime = 0.02f; // Small time step for the simulation, adjust as needed
        float elapsed = 0f; // Time elapsed
        float velocity = initialVelocity;
        float traveledDistance = 0f;

        while (true)
        {
            // Update position and time
            traveledDistance += velocity * deltaTime;
            elapsed += deltaTime;

            // Check if the target distance has been reached or exceeded
            if ((initialVelocity >= 0 && traveledDistance >= distance) ||
                (initialVelocity < 0 && traveledDistance <= distance))
            {
                break;
            }

            // Apply drag and acceleration to the velocity
            velocity *= (1 - dragCoefficient * deltaTime);
            velocity += acceleration * deltaTime;

            // Break if velocity becomes too low, indicating it won't reach the target
            if (Mathf.Abs(velocity) < 0.001f)
            {
                break;
            }

            // Safety check to avoid infinite loops
            if (elapsed > 5)
            { // Arbitrary large number for timeout
                Debug.LogError("CalculateTimeWithDrag: Timeout - loop ran too long");
                return -1f;
            }
        }

        return elapsed;
    }




    bool IsBallApproaching()
    {
        string lastHit = ball.GetComponent<BallScript>().hits.LastOrDefault();
        if (side == Side.Right && (lastHit == "" || lastHit == "Player2" || lastHit == "Right" || ball.transform.position.x < 0))
        {
            return false;
        }
        if (side == Side.Left && (lastHit == "" || lastHit == "Player1" || lastHit == "Left" || ball.transform.position.x > 0))
        {
            return false;
        }
        return true;
    }

    Vector2 CalculateVelocityWithDrag(Vector2 initialVelocity, float drag, float deltaTime)
    {
        // Euler's method to approximate velocity under drag
        return initialVelocity * (1 - drag * deltaTime);
    }

    Vector3 CalculatePositionWithDrag(Vector3 initialPosition, Vector2 initialVelocity, float drag, float timeToReach, float gravity)
    {
        Vector3 position = initialPosition;
        Vector2 velocity = initialVelocity;
        float deltaTime = 0.02f; // Small time step, adjust as needed

        for (float t = 0; t < timeToReach; t += deltaTime)
        {
            // Update position
            position += new Vector3(velocity.x, velocity.y, 0) * deltaTime;
            // Update velocity for drag
            velocity = CalculateVelocityWithDrag(velocity, drag, deltaTime);
            // Apply gravity
            velocity.y += gravity * deltaTime;
        }

        return position;
    }

    Vector3 AdjustLookAt(Vector3 currentLookAt, Vector2 ballVelocity)
    {
        if (Mathf.Abs(directionEasing) < 0.001f)
        {
            return currentLookAt;
        }

        // Calculate the trajectory angle of the ball
        float trajectoryAngle = Mathf.Atan2(ballVelocity.y, ballVelocity.x) * Mathf.Rad2Deg;

        float adjustmentFactor = Vector3.Distance(Vector3.zero, targetPosition) / directionEasing;

        // Adjust the lookAt position based on the trajectory angle
        // If the ball is moving downwards, adjust the lookAt position upwards, and vice versa
        float adjustment = trajectoryAngle * adjustmentFactor;

        return new Vector3(currentLookAt.x, currentLookAt.y + adjustment, currentLookAt.z);
    }

    float CalculateHighestPoint(Vector2 initialVelocity, float gravity, float drag, float initialY)
    {
        Vector2 velocity = initialVelocity;
        float yPosition = initialY;
        float deltaTime = 0.02f; // Small time step, adjust as needed

        while (velocity.y > 0)
        {
            // Update position
            yPosition += velocity.y * deltaTime;
            // Apply gravity
            velocity.y += gravity * deltaTime;
            // Apply drag
            velocity.y *= (1 - drag * deltaTime);
        }

        return yPosition;
    }

    Vector2 CalculateVelocityWithDrag(Vector2 initialVelocity, float gravity, float dragCoefficient, float timeToReach)
    {
        float deltaTime = 0.02f; // Small time step, adjust as needed
        Vector2 velocity = initialVelocity;
        float elapsedTime = 0f;

        while (elapsedTime < timeToReach)
        {
            // Apply drag to the velocity
            velocity *= (1 - dragCoefficient * deltaTime);
            // Apply gravity to the vertical component of the velocity
            velocity.y += gravity * deltaTime;
            // Update elapsed time
            elapsedTime += deltaTime;
        }

        return velocity;
    }


    bool adjust = false;

    void HandleAIControl()
    {
        MoveToTarget();
        Vector2 servePosition = side == Side.Right ? new Vector2(14f, -3f) : new Vector2(-14f, -3f);
        lookAt = strikePosition;

        switch (aiState)
        {
            case AIState.Prepare:
                aiSpeed = moveSpeed;
                SetTarget(side == Side.Right ? new Vector3(13f, -2f, 0f) : new Vector3(-13f, -2f, 0f));
                // Serving
                if (ball.GetComponent<BallScript>().serving && serving)
                {
                    aiState = AIState.PrepareServe;
                    SetTarget(servePosition + (side == Side.Right ? new Vector2(2f, 2f) : new Vector2(-2f, 2f)));
                    break;
                }

                if (!IsBallApproaching() || ball.GetComponent<Rigidbody2D>().velocity.y >= -0.1f)
                {
                    break;
                }
                Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

                // Initial conditions
                float gravity = Physics2D.gravity.y;
                Vector2 initialVelocity = rb.velocity;
                Vector3 initialPosition = ball.transform.position;

                // Calculate the time to reach table
                float tableY = -5f;
                float timeToReachTable = CalculateTimeWithDrag(initialVelocity.y, gravity, tableY - initialPosition.y, dragCoefficient);
                if (timeToReachTable < 0)
                {
                    Debug.LogWarning("Time to reach table is negative");
                    break;
                }
                // Vector3 positionAtTable = new Vector3(initialPosition.x + initialVelocity.x * timeToReachTable, tableY, 0f);
                Vector3 positionAtTable = CalculatePositionWithDrag(initialPosition, initialVelocity, dragCoefficient, timeToReachTable, gravity);

                if ((side == Side.Right && positionAtTable.x > 14f) || (side == Side.Left && positionAtTable.x < -14f))
                {
                    Debug.LogWarning("Ball will not reach table");
                    break;
                }
                // Vector2 velocityAtTable = new Vector2(initialVelocity.x * tableBounciness, (initialVelocity.y + gravity * timeToReachTable) * tableBounciness);
                // Vector2 velocityAtTable = new Vector2(initialVelocity.x * tableBounciness, -(initialVelocity.y + gravity * timeToReachTable) * tableBounciness);

                Vector2 velocityAtTable = CalculateVelocityWithDrag(initialVelocity, gravity, dragCoefficient, timeToReachTable);
                velocityAtTable = new Vector2(velocityAtTable.x * tableBounciness, -velocityAtTable.y * tableBounciness);

                // Assuming the ball bounces and moves upwards
                // float desiredY = tableY + (velocityAtTable.y * velocityAtTable.y) / (2 * -gravity);
                float desiredY = CalculateHighestPoint(velocityAtTable, gravity, dragCoefficient, tableY) - 0.1f;

                Debug.Log("DesiredY: " + desiredY);
                Debug.Log("velocityAtTable: " + velocityAtTable);

                // Calculate the time to reach the desired y position
                // This time, we consider the motion starting from targetY to desiredY
                float timeToDesiredY = CalculateTimeWithDrag(velocityAtTable.y, gravity, desiredY - positionAtTable.y, dragCoefficient);
                Debug.Log("Time to desired y: " + timeToDesiredY);

                if (timeToDesiredY < 0)
                {
                    Debug.LogWarning("Time to desired y is negative");
                    break;
                }
                // Vector3 positionAtDesired = new Vector3(positionAtTable.x + velocityAtTable.x * timeToDesiredY, desiredY, 0f);
                Vector3 positionAtDesired = CalculatePositionWithDrag(positionAtTable, velocityAtTable, dragCoefficient, timeToDesiredY, gravity);

                Debug.Log("positionAtDesired: " + positionAtDesired);

                strikePosition = side == Side.Right ? new Vector3(-6f, aimHeight, 0f) : new Vector3(6f, aimHeight, 0f);
                SetTarget(new Vector3(positionAtDesired.x, desiredY - 0.4f, 0f));
                waitUntil = Time.timeSinceLevelLoad + timeToReachTable + timeToDesiredY - 0.3f;
                aiState = AIState.WaitStrike;
                adjust = true;

                break;
            case AIState.PrepareServe:
                strikePosition = side == Side.Right ? new Vector3(10f, -5f, 0f) : new Vector3(-10f, -5f, 0f);
                aiSpeed = moveSpeed;
                adjust = false;
                if (IsAtTarget())
                {
                    ServeBall();
                    aiState = AIState.WaitStrike;
                    waitUntil = Time.timeSinceLevelLoad + Random.Range(0.2f, 0.4f);
                    // (13, -5), 22.73, 
                }
                break;
            case AIState.WaitStrike:
                Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
                if (adjust)
                    lookAt = AdjustLookAt(strikePosition, ballRb.velocity);
                if (Time.timeSinceLevelLoad > waitUntil)
                {
                    Strike();
                    aiState = AIState.Strike;
                    waitUntil = Time.timeSinceLevelLoad + 1f;
                }
                break;
            case AIState.Strike:
                if (IsAtTarget() || Time.timeSinceLevelLoad > waitUntil)
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
