using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerWASD : MonoBehaviour
{
    [Header("Player Control Data")]
    InputController inputController;
    Vector2 movementInput;
    Vector2 cameraInput;
    //访问按键
    //public InputElement northInput = new InputElement();

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
    [Range(1, 20)]
    public float cameraMoveSpeed;
    public float cameraMaxAngle;
    public float cameraMinAngle;
    public Vector2 cameraAngles;
    Vector3 newPosition= new Vector3(0, 3, -20);

    [Header("Player Movement Data")]
    private CharacterController controller;
    public Vector3 moveDirection;
    [Range(0, 10)]
    public float rotationSpeed;
    [Range(0, 10)]
    public float movementSpeed;
    public bool isThirdPerson=true;
    public float jumpSpeed;
    public float gravity=9.8f;
    private bool IsGrounded=true;
    private bool ReadyToJump=true;
    //垂直速度
    float VerticalSpeed=-1f;

    private Animator anim;
    private CharacterStats characterStats;
    private float lastAttackTime;
    private bool isDead;
    public bool MouseLeftClicked = false;
    public bool isDizzy;
    public bool ObserverFlag;//是否已广播死亡
    

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        cameraSystem = GameObject.FindGameObjectsWithTag("CameraSystem")[0].transform;
        cameraPivot = GameObject.FindGameObjectsWithTag("CameraPivot")[0].transform;
        cameraObject = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }

    #region inputControls
    private void OnEnable()
    {
        GameManager.Instance.RigisterPlayer(characterStats);
        if (inputController == null)
        {
            inputController = new InputController();
            inputController.Movement.Move.performed += inputController => movementInput = inputController.ReadValue<Vector2>();
            inputController.Movement.Camera.performed += inputController => cameraInput = inputController.ReadValue<Vector2>();

            //inputController.Actions.ChangeView.started += inputController => northInput.risingEdge = true;
            //inputController.Actions.ChangeView.performed += inputController => northInput.longPress = true;
            //inputController.Actions.ChangeView.canceled += inputController => northInput.releaseEdges();

            inputController.Attack.Attack1.performed += inputController => MouseLeftClicked = true;
            inputController.Attack.Attack1.canceled += inputController => MouseLeftClicked = false;
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
        isThirdPerson=true;//不写就成false了,但是初始化的明明是true,不知道为什么,离谱
        SaveManager.Instance.LoadPlayerData();
    }
    void Update()
    {
        //Debug.Log(isThirdPerson);

        isDead = characterStats.CurrentHealth == 0;
        if (isDead&&!ObserverFlag)
        {
            //广播角色的死亡
            GameManager.Instance.NotifyObservers();
            ObserverFlag = true;
        }
        else
        {
            //眩晕时也要计算重力
            if (!isDizzy)
            {
                if (controller.enabled == true)
                {
                    //处理运动
                    HandleMovement();
                    //处理旋转的方式
                    MovementRotation();
                    if (MouseLeftClicked)
                    {
                        Attack();
                    }
                }
            }
            else
            {
                //TODO:眩晕时也要计算重力
                controller.Move(Vector3.zero);
            }
        }
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", controller.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    Vector3 normalVector = Vector3.up;//暂定
    //角色前进
    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * movementInput.y;
        moveDirection += cameraObject.right * movementInput.x;
        //水平速度方向
        Vector3 HorizontalVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        HorizontalVelocity.Normalize();
        //水平速度
        HorizontalVelocity *= movementSpeed;
        

        if(!Keyboard.current.spaceKey.wasPressedThisFrame&&IsGrounded)
        {
            ReadyToJump=true;
        }
        if(IsGrounded)
        {
            //垂直速度
            VerticalSpeed=gravity*0.3f*-1;
            if(Keyboard.current.spaceKey.wasPressedThisFrame&&ReadyToJump)
            {
                VerticalSpeed=jumpSpeed;
                IsGrounded=false;
                ReadyToJump=false;
            }
        }
        else
        {
            if(!Keyboard.current.spaceKey.wasPressedThisFrame&&VerticalSpeed>0.0f)
            {
                VerticalSpeed-=10f*Time.deltaTime;
            }
            if(Mathf.Approximately(VerticalSpeed,0f))
            {
                VerticalSpeed=0f;
            }
            //在空中则施加重力
            VerticalSpeed-=gravity*Time.deltaTime;
        }
        
        if(IsGrounded)
        {
            RaycastHit hit;
            Ray ray =new Ray(transform.position+Vector3.up*0.5f,Vector3.down);
            if(Physics.Raycast(ray,out hit,1f,Physics.AllLayers,QueryTriggerInteraction.Ignore))
            {
                HorizontalVelocity=Vector3.ProjectOnPlane(HorizontalVelocity, hit.normal);
            }
            else
            {
                Debug.Log("不会吧不会吧,不会真能else吧");
            }
        }
        else
        {
            HorizontalVelocity=movementSpeed*transform.forward;
        }

        Vector3 movement=HorizontalVelocity*Time.deltaTime;;
        movement+=VerticalSpeed*Vector3.up*Time.deltaTime;
        controller.Move(movement);   
        IsGrounded=controller.isGrounded;
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
        cameraSystem.position = Vector3.Lerp(cameraSystem.position, cameraFollowTarget.position, Time.deltaTime * cameraFollowSpeed * cameraMoveSpeed);
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
    //相机切换
    private void CameraChangeView()
    {
        //这里是长按v时有一个类似刺客信条鹰眼的第三人称窥视效果,也可以改成和英灵殿一样的短按长久切换
        //if (northInput.fallingEdge)//第一人称
        if(Keyboard.current.vKey.wasPressedThisFrame)
        {
            if(isThirdPerson== true)
            {
                isThirdPerson = false; 
                newPosition = new Vector3(0, 1.03f, 0.16f);
            }
            else
            {
                isThirdPerson = true;
                newPosition = new Vector3(0, 3, -20);
            }
            
        }
        //cameraPivot.localPosition = Vector3.Lerp(cameraPivot.localPosition, newPosition, Time.deltaTime * cameraFollowSpeed*2f);
        cameraObject.localPosition = Vector3.Lerp(cameraObject.localPosition, newPosition, Time.deltaTime * cameraFollowSpeed * 2f);
    }

    private void Attack()
    {
        if (lastAttackTime < 0)
        {
            //计算暴击
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }
    //Animation Event
    void Hit()
    {
        var colliders = Physics.OverlapSphere(transform.position, characterStats.attackData.attackRange);
        if (colliders != null)
        {
            foreach (var target in colliders)
            {
                if (transform.IsFacingTarget(target.transform))
                {
                    if (target.CompareTag("Attackable"))
                    {
                        if (target.GetComponent<Rock>() && target.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
                        {
                            target.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                            target.GetComponent<Rigidbody>().velocity = Vector3.one;
                            target.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
                        }
                    }
                    else if (target.CompareTag("Enemy"))
                    {
                        var targetStats = target.GetComponent<CharacterStats>();
                        targetStats.TakeDamage(characterStats, targetStats);
                    }
                }
            }
        }     
    }
    private void LateUpdate()
    {
        //相机切换角度,一三人称切换的时候用
        CameraChangeView();
        //相机移动
        CameraMovement();
    }

    private bool WasGround(){
        float radius=controller.radius*0.9f;
        float overLapCapsuleOffset=0.2f;
        Vector3 pointBottom = transform.position + transform.up * radius-transform.up*overLapCapsuleOffset;
        Vector3 pointTop = transform.position + transform.up * controller.height - transform.up * radius;
        LayerMask ignoreMask = ~LayerMask.GetMask("Player");
 
        Collider[] colliders = Physics.OverlapCapsule(pointBottom, pointTop, radius, ignoreMask);
        Debug.DrawLine(pointBottom, pointTop,Color.green);
        if (colliders.Length!=0)
        {
            return true;
        }
        else
        {

            return false;
        }
    }

}
