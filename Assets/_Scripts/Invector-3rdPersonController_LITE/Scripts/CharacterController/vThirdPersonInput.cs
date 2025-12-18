using UnityEngine;
using UnityEngine.InputSystem;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";

        public InputSystem_Actions Actions;
        public InputAction inputAction_Move;
        public InputAction inputAction_Jump;
        public InputAction inputAction_Sprint;

        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";
        public float _defaultRotationSpeed;

        [HideInInspector]
        public vThirdPersonController cc;

        [HideInInspector]
        public vThirdPersonCamera tpCamera;

        [HideInInspector]
        public Camera cameraMain;

        #endregion

        protected virtual void OnEnable() { }

        protected virtual void OnDisable()
        {
            Actions.Disable();
        }

        protected virtual void Start()
        {
            Actions = new InputSystem_Actions();
            Actions.Enable();
            cameraMain = Camera.main;

            inputAction_Move = Actions.Player.Move;
            inputAction_Jump = Actions.Player.Jump;
            inputAction_Sprint = Actions.Player.Sprint;

            InitilizeController();
            // InitializeTpCamera();

            _defaultRotationSpeed = cc.freeSpeed.rotationSpeed;
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor(); // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType(); // handle the controller locomotion type and movespeed
            cc.ControlRotationType(); // handle the controller rotation type

            // if (Actions.Player.Zoom.IsPressed())
            // {
            //   print("Zoom Condition Activated");
            //   CAMERASingleton.i.primaryFreelook.enabled = false;
            // }
            // else
            // {
            //   CAMERASingleton.i.primaryFreelook.enabled = true;
            // }
        }

        protected virtual void Update()
        {
            InputHandle(); // update the input methods
            cc.UpdateAnimator(); // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
        }

        public virtual void MoveInput()
        {
            cc.input.x = inputAction_Move.ReadValue<Vector2>().x;
            cc.input.z = inputAction_Move.ReadValue<Vector2>().y;

            // cc.input.x = Input.GetAxis(horizontalInput);
            // cc.input.z = Input.GetAxis(verticallInput);
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main)
                    Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (Actions.Player.Strafe.IsPressed())
                cc.Strafe();
            // if (Input.GetKeyDown(strafeInput))
            //   cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            // if (Input.GetKeyDown(sprintInput))
            //   cc.Sprint(true);
            // else if (Input.GetKeyUp(sprintInput))
            //   cc.Sprint(false);
            cc.Sprint(inputAction_Sprint.IsPressed());
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded
                && cc.GroundAngle() < cc.slopeLimit
                && !cc.isJumping
                && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump
        /// </summary>
        protected virtual void JumpInput()
        {
            if (inputAction_Jump.WasPerformedThisFrame() && JumpConditions())
            {
                cc.Jump();
                cc.freeSpeed.rotationSpeed = cc.freeSpeed.airborneRotationSpeed;
            }
            else
            {
                cc.freeSpeed.rotationSpeed = _defaultRotationSpeed;
            }
            // if (Input.GetKeyDown(jumpInput) && JumpConditions())
            //   cc.Jump();
        }

        #endregion
    }
}
