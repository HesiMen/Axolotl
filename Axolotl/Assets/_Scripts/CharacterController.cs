using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// For my character controller, I am going to make the movement be world based. - this got to be camera forward -felt better. 
/// Rotations will be according to the direction its heading
/// if pressing right, the character will look to the right and start moving.
/// Movement should simulate an axolotl => Swim/walk should feel light and will slerp towards a point in front of the the character
/// 
/// </summary>
/// 

public class CharacterController : MonoBehaviour
{
    [SerializeField] private TailManager _tailManager;
    [Header("Movement")]
    [SerializeField] private float _rotationSpeed = 2f;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _movementTime = 2f;
    [SerializeField] public float _distanceToJump = 2f;
    [SerializeField] private float _rockSpeed = 2f;
    [SerializeField] private float _MaxJumpHeight = 3.5f;
    private float currJumpHeigt;
    private float currDistanceJump;
    [SerializeField] private Transform _parentChar;
    [SerializeField] private Transform _completeCharacter;
    [SerializeField] private Transform _head;
    [Header("Anim Curves Movement")]
    [SerializeField] private AnimationCurve jumpLerp;
    [SerializeField] private AnimationCurve headAnimationJump;
    [SerializeField] private AnimationCurve swimAnimation;
    [SerializeField] private float _setHeight = .5f;
    [SerializeField] private LayerMask floorOnly;
    [SerializeField] private float _upHillSlope = .8f;
    [SerializeField] private Transform forwardCamera;
    [SerializeField] private float _fallTime = 1f;
    private float _originalSpeed;
    private bool jumping = false;
    private bool isFalling = false;

    float curveDelta = 0;
    private bool isUpHill = false;

    public UnityEvent OnJump;
    float speedCollects = 0f;
    private void Start()
    {
        _originalSpeed = _movementSpeed;
        currJumpHeigt = _MaxJumpHeight;
        currDistanceJump = _distanceToJump;
    }
    void Update()
    {
        float zMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");




        if (!jumping)
        {
            //Rotation Movement
            float stepRotationSpeed = _rotationSpeed * Time.deltaTime;
            Vector3 targetDirection = ((_completeCharacter.position + forwardCamera.right * .1f) - _completeCharacter.position) * xMovement
                + ((_completeCharacter.position + forwardCamera.forward * .1f) - _completeCharacter.position) * zMovement; // sharp turns

            Vector3 newDirection = Vector3.RotateTowards(_completeCharacter.forward, targetDirection, stepRotationSpeed, 0f);
            _completeCharacter.rotation = Quaternion.LookRotation(newDirection);

            //Character Movement
            Vector3 movement = forwardCamera.right * xMovement + forwardCamera.forward * zMovement;
            if (movement != Vector3.zero)
            {
                curveDelta += Time.deltaTime;
                // Debug.Log(curveDelta);
                float stepMovementSpeed = _movementSpeed * Time.deltaTime * swimAnimation.Evaluate(curveDelta);

                _parentChar.Translate(movement * stepMovementSpeed);
                if (curveDelta > 1f) curveDelta = 0;
            }
            else
            {
                curveDelta = 0;
            }

            float yaw;

            yaw = movement != Vector3.zero ? Mathf.PingPong(Time.time * _rockSpeed, 15f) : Mathf.PingPong(Time.time, 15f);
            //HeadBobbing
            _head.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);
            //jump
            // wanna make the collection clearer - speed up depending on tail collect.
          
            switch (_tailManager.currState)
            {
                case SlowTailDamage.AOETail.slow:
                    speedCollects = .25f;
                    break;
                case SlowTailDamage.AOETail.tailoff:
                    speedCollects = .3f;
                    break;
                case SlowTailDamage.AOETail.adapt:
                    speedCollects = _tailManager.growthPercent / 30f;
                    break;
                case SlowTailDamage.AOETail.none:
                    switch (_tailManager.counter)
                    {

                        case -1:
                            currJumpHeigt = _MaxJumpHeight / 4f;
                            currDistanceJump = _distanceToJump / 4f;
                            speedCollects = 1f;
                            break;
                        case 0:
                            currJumpHeigt = _MaxJumpHeight * 2F / 4f;
                            currDistanceJump = _distanceToJump * 2F / 4f;
                            speedCollects = 1.2f;
                            break;

                        case 1:
                            currJumpHeigt = (_MaxJumpHeight * 3f) / 4f;
                            currDistanceJump = (_distanceToJump * 3f) / 4f;
                            speedCollects = 1.35f;
                            break;

                        case 2:
                            currJumpHeigt = _MaxJumpHeight;
                            currDistanceJump = _distanceToJump;
                            speedCollects = 1.5f;
                            break;
                        default:
                            break;

                    }
                    break;
                default:
                    break;
            }




            Vector3 target = (_completeCharacter.position + Vector3.up * currJumpHeigt) + _completeCharacter.forward * currDistanceJump;
            if (Input.GetKeyDown(KeyCode.Space) && !isFalling && _tailManager.currState != SlowTailDamage.AOETail.tailoff)
                StartCoroutine(JumpNow(target, _movementTime));

            if (!isFalling)
            {
                Vector3 movingVectorFloor = CheckForFloor(_completeCharacter.position + _completeCharacter.forward * .1f) - _completeCharacter.forward * .1f;
                if (movingVectorFloor != Vector3.zero)
                    _completeCharacter.position = movingVectorFloor;
            }



            if (isUpHill)
                _movementSpeed = 0f;
            else
            {
               
                    _movementSpeed = _originalSpeed * speedCollects;
            }



            IsDownHill();
        }




    }


    IEnumerator JumpNow(Vector3 target, float time)
    {

        jumping = true;
        OnJump?.Invoke();
        float counter = 0;
        Vector3 startPos = _completeCharacter.position;
        Vector3 startHeadPos = _head.localPosition;
        float anim;
        float headAnim;
        Vector3 endPos = CheckForFloor(target);
        while (counter < time)
        {
            float t = Mathf.Clamp01(counter / time);
            anim = jumpLerp.Evaluate(t);
            headAnim = headAnimationJump.Evaluate(t);
            //Debug.Log(anim);
            if (endPos != Vector3.zero)
                _completeCharacter.position = Vector3.Lerp(startPos, endPos, anim);

            //headJumpAnimation
            _head.localPosition = Vector3.Lerp(startHeadPos, new Vector3(0f, headAnim * currJumpHeigt, 0f), t);

            //headbob
            float yaw = Mathf.PingPong(Time.time * _rockSpeed, 10f);
            // Debug.Log(yaw);
            _head.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);

            yield return new WaitForEndOfFrame();
            counter += Time.deltaTime;

        }

        jumping = false;
    }

    Vector3 prevPos;
    private Vector3 CheckForFloor(Vector3 from)
    {
        Vector3 isThereFloor;

        RaycastHit hit;

        if (Physics.Raycast(from, _completeCharacter.TransformDirection(Vector3.down), out hit, Mathf.Infinity, floorOnly))
        {
            Debug.DrawRay(from, _completeCharacter.TransformDirection(Vector3.down) * hit.distance, Color.green);
            //Debug.Log(hit.distance);
            Quaternion newQdir = Quaternion.FromToRotation(transform.up, hit.normal) * _head.rotation;
            _head.rotation = newQdir;
            _movementSpeed = _originalSpeed;
            // Debug.Log(hit.distance);
            Vector3 offSetFloot = new Vector3(hit.point.x, hit.point.y + _setHeight, hit.point.z);
            isThereFloor = offSetFloot;
            //Falling
            if (hit.distance > 2.5f && !isFalling)
            {
                if (!jumping)
                {

                    StartCoroutine(FallingNow(_completeCharacter.forward * 5f + offSetFloot, _fallTime));
                }
            }
            prevPos = _completeCharacter.position;
        }
        else
        {
            Debug.DrawRay(_head.position, _head.TransformDirection(Vector3.down) * 1000, Color.red);
            isThereFloor = prevPos;
            _movementSpeed = 0f;
        }

        return isThereFloor;
    }

    IEnumerator FallingNow(Vector3 target, float time)
    {
        isFalling = true;
        float counter = 0;
        Vector3 startPos = _completeCharacter.position;
        Vector3 endPos = CheckForFloor(target);
        while (counter < time)
        {
            float t = Mathf.Clamp01(counter / time);

            _completeCharacter.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForEndOfFrame();
            counter += Time.deltaTime;

        }

        isFalling = false;
    }
    private void IsDownHill()
    {
        RaycastHit frontHit;
        if (Physics.Raycast(_completeCharacter.position, _completeCharacter.TransformDirection(Vector3.down), out frontHit, Mathf.Infinity, floorOnly))//front 
        {
            Debug.DrawRay(_completeCharacter.position, _completeCharacter.TransformDirection(Vector3.down) * frontHit.distance, Color.cyan);
            RaycastHit rearHit;
            if (Physics.Raycast(_completeCharacter.position - _completeCharacter.forward, _completeCharacter.TransformDirection(Vector3.down), out rearHit, Mathf.Infinity, floorOnly))//back 
            {
                Debug.DrawRay(_completeCharacter.position - _completeCharacter.forward, _completeCharacter.TransformDirection(Vector3.down) * rearHit.distance, Color.cyan);
            }
            //Debug.Log($"FrontDistance is : {frontHit.distance} and BackDistance is: {rearHit.distance} and the difference is {rearHit.distance - frontHit.distance}");
            isUpHill = rearHit.distance - frontHit.distance > _upHillSlope ? true : false;
        }

    }
}
