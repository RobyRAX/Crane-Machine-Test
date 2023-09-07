using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public enum ClawState
{
    Open,
    Close,
    Rise,
}

public class ClawController : MonoBehaviour
{
    public static event Action OnStateReset;

    public ClawState currentState;

    [Header("Claws")]
    public GameObject Claw_L;
    public GameObject Claw_R;
    public GameObject Hanger;

    [Header("Move Parameter")]
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 moveArea;

    [Header("Open Parameter")]
    [SerializeField] float openClawAngle;
    [SerializeField] float resetDuration;

    [Header("Close Parameter")]
    [SerializeField] float dropClawAngle;
    [SerializeField] float openDuration;
    [SerializeField] float delayToClose;
    [SerializeField] float closeClawAngle;
    [SerializeField] float closeDuration;
    [SerializeField] float delayToRise;

    [Header("Rise Parameter")]
    [SerializeField] float riseDuration;
    [SerializeField] float delayToDeliver;
    [SerializeField] float deliverDuration;
    [SerializeField] float collectClawAngle;
    [SerializeField] float delayToCollect;
    [SerializeField] float collectDuration;    
    [SerializeField] float delayToReset;

    PlayerInputSystem playerInput;
    Rigidbody rb;

    bool isMove;
    Vector2 moveVectorValue;
    Vector3 initialPosition;
    Vector3 dropPosition;
    Vector3 initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInputSystem();
        playerInput.Enable();

        playerInput.ClawControl.Move.performed += MoveInputHandler;
        playerInput.ClawControl.Move.canceled += MoveInputHandler;
        playerInput.ClawControl.Drop.performed += DropInputHandler;

        initialPosition = transform.position;
        initialRotation = transform.localRotation.eulerAngles;

        UpdateClawState(ClawState.Open);        
    }

    // Update is called once per frame
    void Update()
    {
        MoveClaw();
    }

    private void LateUpdate()
    {
        if(isMove)
        {
            Hanger.transform.position = new Vector3(transform.position.x, Hanger.transform.position.y, transform.position.z);
        }
    }

    //------------------ SIMPLE STATE MACHINE ------------------//
    void UpdateClawState(ClawState newState)
    {
        currentState = newState;

        switch(currentState)
        {
            case ClawState.Open:
                ClawOpenHandler();
                break;
            case ClawState.Close:
                ClawDownHandler();
                break;
            case ClawState.Rise:
                ClawRiseHandler();
                break;
        }
    }

    void ClawOpenHandler()
    {
        StartCoroutine(OpenClawCo());
    }

    void ClawDownHandler()
    {        
        StartCoroutine(DropClawCo());
    }

    void ClawRiseHandler()
    {
        StartCoroutine(RiseClawCo());
    }
    //------------------ END ------------------//

    //------------------ CONTROL HANDLER ------------------//
    void MoveInputHandler(InputAction.CallbackContext context)
    {
        if (currentState == ClawState.Open && GameManager.Instance.currentState == GameState.Gameplay)
        {
            if(context.performed)
            {
                moveVectorValue = playerInput.ClawControl.Move.ReadValue<Vector2>();
            }       
            else if(context.canceled)
            {
                moveVectorValue = Vector2.zero;
            }
        }
    }

    void DropInputHandler(InputAction.CallbackContext context)
    {
        if (currentState == ClawState.Open  && GameManager.Instance.currentState == GameState.Gameplay)
        {
            if (context.performed)
            {
                UpdateClawState(ClawState.Close);
            }
        }
    }
    //------------------ END ------------------//

    //------------------ GAME LOGIC ------------------//
    IEnumerator OpenClawCo()
    {        
        isMove = true;
        //Set the initial form of the claw
        transform.DOMove(initialPosition, resetDuration).SetEase(Ease.InOutSine);
        Claw_L.transform.DOLocalRotate(new Vector3(0, 0, openClawAngle), resetDuration).SetEase(Ease.InOutSine);
        Claw_R.transform.DOLocalRotate(new Vector3(0, 0, -openClawAngle), resetDuration).SetEase(Ease.InOutSine);

        //Lock the rigidbody
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        yield return new WaitForSeconds(resetDuration);
        OnStateReset();
    }

    void MoveClaw()
    {
/*        if (transform.position.x > -moveArea.x && transform.position.x < moveArea.x && transform.position.z > -moveArea.z && transform.position.z < moveArea.z)*/
        transform.Translate(new Vector3(moveVectorValue.x, 0, moveVectorValue.y) * moveSpeed * Time.deltaTime);

        if (transform.position.x > moveArea.x)
            transform.position = new Vector3(moveArea.x, transform.position.y, transform.position.z);
        if(transform.position.x < -moveArea.x)
            transform.position = new Vector3(-moveArea.x, transform.position.y, transform.position.z);
        if (transform.position.z > moveArea.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, moveArea.z);
        if (transform.position.z < -moveArea.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, -moveArea.z);

        /*canMoveXPlus = (transform.position.x > -moveArea.x) ? true : false;
        canMoveXMinus = (transform.position.x < moveArea.x) ? true : false;
        canMoveZPlus = (transform.position.z > -moveArea.z) ? true : false;
        canMoveZMinus = (transform.position.z < moveArea.z) ? true : false;

        if (canMoveXPlus)
            transform.position += new Vector3(moveVectorValue.x, 0, 0) * moveSpeed * Time.deltaTime;
        if(canMoveXMinus)
            transform.position += new Vector3(moveVectorValue.x, 0, 0) * -moveSpeed * Time.deltaTime;*/
    }

    IEnumerator DropClawCo()
    {
        isMove = false;
        //Save the before-drop position (will be used later in the Rise State)
        dropPosition = transform.position;

        //Drop and open the Claw
        rb.isKinematic = false;
        Claw_L.transform.DOLocalRotate(new Vector3(0, 0, -dropClawAngle), openDuration).SetEase(Ease.InOutSine);
        Claw_R.transform.DOLocalRotate(new Vector3(0, 0, dropClawAngle), openDuration).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(openDuration + delayToClose);

        //Close the claw to grab some prize
        Claw_L.transform.DOLocalRotate(new Vector3(0, 0, closeClawAngle), closeDuration).SetEase(Ease.InOutSine);
        Claw_R.transform.DOLocalRotate(new Vector3(0, 0, -closeClawAngle), closeDuration).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(closeDuration + delayToRise);

        //Move to the Rise State
        UpdateClawState(ClawState.Rise);
    }

    IEnumerator RiseClawCo()
    {
        //Pull the claw to before-drop position
        transform.DOMove(dropPosition, riseDuration).SetEase(Ease.InOutSine);
        transform.DOLocalRotate(initialRotation, riseDuration).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(riseDuration);

        //Lock the Rigidbody after reach the top
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        yield return new WaitForSeconds(delayToDeliver);

        //Move the claw to the corner to collect the prize
        isMove = true;
        transform.DOMove(new Vector3(-moveArea.x, initialPosition.y, -moveArea.z), deliverDuration).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(deliverDuration + delayToCollect);

        //Open the claw to drop the prize
        Claw_L.transform.DOLocalRotate(new Vector3(0, 0, collectClawAngle), collectDuration).SetEase(Ease.InOutSine);
        Claw_R.transform.DOLocalRotate(new Vector3(0, 0, -collectClawAngle), collectDuration).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(collectDuration + delayToReset);

        //Reset to Open State
        UpdateClawState(ClawState.Open);       
    }
}
