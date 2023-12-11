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
public class PaddleScript : MonoBehaviour
{
    //how to fix this code to work with the new input system

    public ControlType ControlType = ControlType.Mouse;

    public float rotationSpeed = 120;
    public float maxSpeed = 9;
    public Rigidbody2D body;
    private Vector3 mousePosition;
    public float moveSpeed = 100;
    public InputAction rotate;
    public InputAction moveX;
    public InputAction moveY;

   
    
    
    // Start is called before the first frame update
    void Start()
    {
        //movement = new Vector2(44, 22);
        ControlType = ControlType.Gamepad;
        body = GetComponent<Rigidbody2D>();

        Debug.Log(body.name);
        
    }
    private void OnEnable()
    {
        rotate.Enable();
        moveX.Enable();
        moveY.Enable();
    }
    private void OnDisable()
    {
        rotate.Disable();
        moveX.Enable();
        moveY.Enable();
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
     void HandlePadControl()
    {
        Vector2 joyStickPos = new Vector2(moveX.ReadValue<float>(), moveY.ReadValue<float>());
        Debug.Log(joyStickPos);
        joyStickPos = joyStickPos * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(body.position + joyStickPos);
        Rotate(rotate.ReadValue<float>());
    }
    void HandleMouseControl()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 position = Vector2.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);
        body.MovePosition(mousePosition);
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
