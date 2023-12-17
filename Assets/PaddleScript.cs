using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    void HandleAIControl()
    {


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
           ball.GetComponent<BallScript>().ServeBall();
           
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
            
             ball.GetComponent<BallScript>().ServeBall();
             
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
