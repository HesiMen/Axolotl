using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// For my character controller, I am going to make the movement be world based.
/// Rotations will be according to the direction its heading
/// if pressing right, the character will look to the right and start moving.
/// Movement should simulate an axolotl => Swim/walk should feel light and will slerp towards a point in front of the the character
/// 
/// </summary>
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 2f;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _movementTime = 2f;
    [SerializeField] private float _distanceToMove = 2f;
    [SerializeField] private float _rockSpeed = 2f;
    [SerializeField] private float _jumpHeight = 3.5f;
    [SerializeField] private Transform _parentChar;
    [SerializeField] private Transform _completeCharacter;
    [SerializeField] private Transform _head;
    [SerializeField] private AnimationCurve jumpLerp;
    [SerializeField] private AnimationCurve headAnimationJump;
    private bool jumping = false;

    void Update()
    {
        float zMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");




        if (!jumping)
        {
            //Rotation Movement
            float stepRotationSpeed = _rotationSpeed * Time.deltaTime;
            Vector3 targetDirection = ((_completeCharacter.position + Vector3.right * .1f) - _completeCharacter.position) * xMovement
                + ((_completeCharacter.position + Vector3.forward * .1f) - _completeCharacter.position) * zMovement; // sharp turns

            Vector3 newDirection = Vector3.RotateTowards(_completeCharacter.forward, targetDirection, stepRotationSpeed, 0f);
            _completeCharacter.rotation = Quaternion.LookRotation(newDirection);

            //Character Movement
            Vector3 movement = new Vector3(xMovement, 0f, zMovement);
            float stepMovementSpeed = _movementSpeed * Time.deltaTime;
            _parentChar.Translate(movement * stepMovementSpeed);

            //HeadBobbing
            if (movement != Vector3.zero)
            {
                float yaw = Mathf.PingPong(Time.time * _rockSpeed, 15f);
                //Debug.Log(yaw);
                _head.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);
            }

            Vector3 target = _completeCharacter.position + _completeCharacter.forward * _distanceToMove;
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(JumpNow(target, _movementTime));
            if (!CheckForFloor())
            {
                if (_completeCharacter.gameObject.activeSelf)
                    _completeCharacter.gameObject.SetActive(false);
            }

        }




    }


    IEnumerator JumpNow(Vector3 target, float time)
    {
        jumping = true;
        float counter = 0;
        Vector3 startPos = _completeCharacter.position;
        Vector3 startHeadPos = _head.localPosition;
        float anim;
        float headAnim;
        while (counter < time)
        {
            float t = Mathf.Clamp01(counter / time);
            anim = jumpLerp.Evaluate(t);
            headAnim = headAnimationJump.Evaluate(t);
            //Debug.Log(anim);
            _completeCharacter.position = Vector3.Lerp(startPos, target, anim);

            //headJumpAnimation
            _head.localPosition = Vector3.Lerp(startHeadPos, new Vector3(0f, headAnim * _jumpHeight, 0f), t);

            //headbob
            float yaw = Mathf.PingPong(Time.time * _rockSpeed, 10f);
            // Debug.Log(yaw);
            _head.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);

            yield return new WaitForEndOfFrame();
            counter += Time.deltaTime;

        }
        if (!CheckForFloor())
        {
            if (_completeCharacter.gameObject.activeSelf)
                _completeCharacter.gameObject.SetActive(false);
        }
        jumping = false;
    }

    private bool CheckForFloor()
    {
        bool isThereFloor = false;
        RaycastHit hit;

        if (Physics.Raycast(_head.position, _head.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(_head.position, _head.TransformDirection(Vector3.down) * hit.distance, Color.green);
            isThereFloor = true;
        }
        else
        {
            Debug.DrawRay(_head.position, _head.TransformDirection(Vector3.down) * 1000, Color.red);
        }

        return isThereFloor;
    }
}
