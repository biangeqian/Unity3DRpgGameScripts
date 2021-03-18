using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControllerWASD : MonoBehaviour
{
    [Header("Player Control Data")]
    InputController inputController;
    Vector2 movementInput;
    Vector2 cameraInput;
    //访问按键
    public InputElement northInput = new InputElement();

    [Header("Player Camera Data")]
    public Transform cameraSystem;
    public Transform cameraPivot;
    public Transform cameraObject;
    //相机跟随的物体
    public Transform cameraFollowTarget;
    [Range(0, 10)]
    public float cameraFollowSpeed;
    [Range(0, 10)]
    public float cameraRotateSpeed;
    public float cameraMaxAngle;
    public float cameraMinAngle;
    public Vector2 cameraAngles;

    [Header("Player Movement Data")]
    public CharacterController controller;
    public Vector3 moveDirection;
    [Range(0, 10)]
    public float rotationSpeed;
    [Range(0, 10)]
    public float movementSpeed;
    public bool isThirdPerson;

    #region inputControls
    private void OnEnable()
    {
        if (inputController == null)
        {
            inputController = new InputController();
            inputController.Movement.Move.performed += inputController => movementInput = inputController.ReadValue<Vector2>();
            inputController.Movement.Camera.performed += inputController => cameraInput = inputController.ReadValue<Vector2>();

            inputController.Actions.ChangeView.started += inputController => northInput.risingEdge = true;
            inputController.Actions.ChangeView.performed += inputController => northInput.longPress = true;
            inputController.Actions.ChangeView.canceled += inputController => northInput.releaseEdges();
        }
        inputController.Enable();
    }
    private void OnDisable()
    {
        inputController.Disable();
    }
    #endregion
    void Start()
    {
        
    }
    void Update()
    {
        //处理运动
        HandleMovement();
        //处理旋转的方式
        MovementRotation();
        //character follows camera rotation
        //character follows camera and input
        //focus/lock on camera
        //相机切换角度,一三人称切换的时候用
        CameraChangeView();
        //相机移动
        CameraMovement();
    }

    Vector3 normalVector = Vector3.up;//暂定
    //角色前进
    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * movementInput.y;
        moveDirection += cameraObject.right * movementInput.x;
        moveDirection.y = 0;
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        projectedVelocity.Normalize();
        projectedVelocity *= movementSpeed;
        controller.Move(projectedVelocity * Time.deltaTime);
    }
    //角色转向
    private void MovementRotation()
    {
        Vector3 targetDir = Vector3.zero;
        if (isThirdPerson)
        {
            targetDir = cameraObject.forward * movementInput.y;
            targetDir += cameraObject.right * movementInput.x;
        }
        else
        {
            targetDir = cameraObject.forward;
        }
        
        if (targetDir == Vector3.zero)
        {
            targetDir = transform.forward;//原地不动
        }
        Vector3 projectedDirection = Vector3.ProjectOnPlane(targetDir, normalVector);
        projectedDirection.Normalize();

        Quaternion targetDirection = Quaternion.LookRotation(projectedDirection);
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetDirection, rotationSpeed * Time.deltaTime);
        transform.rotation = smoothRotation;
    }
    //相机移动
    private void CameraMovement()
    {
        cameraSystem.position = Vector3.Lerp(cameraSystem.position, cameraFollowTarget.position, Time.deltaTime * cameraFollowSpeed);
        cameraAngles.x += (cameraInput.x * cameraFollowSpeed) * Time.fixedDeltaTime;
        cameraAngles.y -= (cameraInput.y * cameraFollowSpeed) * Time.fixedDeltaTime;
        if (isThirdPerson)
        {
            cameraAngles.y = Mathf.Clamp(cameraAngles.y, cameraMinAngle, cameraMaxAngle);
        }
        else
        {
            cameraAngles.y = Mathf.Clamp(cameraAngles.y, cameraMinAngle*1.5f, cameraMaxAngle*1.5f);
        }
        Vector3 rotation = Vector3.zero;
        rotation.y = cameraAngles.x;
        cameraSystem.rotation = Quaternion.Euler(rotation);

        rotation = Vector3.zero;
        rotation.x = cameraAngles.y;
        cameraPivot.localRotation = Quaternion.Euler(rotation);
    }
    //相机
    private void CameraChangeView()
    {
        //这里是长按v时有一个类似刺客信条鹰眼的第三人称窥视效果,也可以改成和英灵殿一样的短按长久切换
        if (northInput.longPress)
        {
            isThirdPerson = true;
            Vector3 newPosition = new Vector3(0, 3, -20);
            //cameraPivot.localPosition = Vector3.Lerp(cameraPivot.localPosition, newPosition, Time.deltaTime * cameraFollowSpeed*2f);
            cameraObject.localPosition = Vector3.Lerp(cameraObject.localPosition, newPosition, Time.deltaTime * cameraFollowSpeed * 2f);
        }
        else
        {
            isThirdPerson = false;
            Vector3 newPosition = new Vector3(0, 1.03f, 0.16f);
            //cameraPivot.localPosition = Vector3.Lerp(cameraPivot.localPosition, newPosition, Time.deltaTime * cameraFollowSpeed * 2f);
            cameraObject.localPosition = Vector3.Lerp(cameraObject.localPosition, newPosition, Time.deltaTime * cameraFollowSpeed * 2f);
        }
    }
    private void LateUpdate()
    {
        //每一帧结束时重置
        northInput.resetEdges();
    }
}
