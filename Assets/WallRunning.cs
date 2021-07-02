using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    [SerializeField] private float _wallRunUpForce;
    [SerializeField] private float _wallRunPushForce;
    //<<Summary>> Boolean that is used for adding forces when jumping off the walls, used to determine which direction.
    private bool isRightWall;
    private bool isLeftWall;

    //Used for effects etc.
    public static bool isWallRunning;
    //<<Summary>> Checks the distance from walls and takes the wall that is the closest to the player
    private float distanceFromLeftWall;
    private float distanceFromRightWall;

    //Used to add forces.
    private Rigidbody rb;
    public Transform cam;
    public Transform head;
    PlayerMovement player;
    bool setnormal;
    public Animator animator;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerMovement>();
       
    }
    private void wallChecker()
    {
        RaycastHit rightRaycast;
        RaycastHit leftRaycast;

        if (Physics.Raycast(head.transform.position, head.transform.right, out rightRaycast))
        {
            distanceFromRightWall = Vector3.Distance(head.transform.position, rightRaycast.point);
            if (distanceFromRightWall <= 3f)
            {
                isRightWall = true;
                isLeftWall = false;
            }
        }
        if (Physics.Raycast(head.transform.position, -head.transform.right, out leftRaycast))
        {
            distanceFromLeftWall = Vector3.Distance(head.transform.position, leftRaycast.point);
            if (distanceFromLeftWall <= 3f)
            {
                isRightWall = false;
                isLeftWall = true;
            }
        }

    }

    private void Update()
    {
        wallChecker();
        
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("RunnableWall"))
        {
            isWallRunning = true;
            rb.useGravity = false;
            player.doubleJump = false;
            animator.SetFloat("Speed", 2f);
            animator.SetBool("Grounded", true);
            
            if (isLeftWall)
            {
                //cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(0, 0, -10), 0.2f);
                StartCoroutine(AnimateRotationTowards(cam, Quaternion.Euler(0,0,-15), 0.2f));
            }
            if (isRightWall)
            {
                //cam.transform.localEulerAngles = new Vector3(0f, 0f, 10f);
                //cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(0, 0, 10), 0.2f);
                StartCoroutine(AnimateRotationTowards(cam, Quaternion.Euler(0, 0, 15), 0.2f));
            }



        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("RunnableWall"))
        {
           
            rb.AddForce(head.forward * _wallRunPushForce, ForceMode.Acceleration);
            


        }
    }
    private void OnCollisionExit(Collision collision)
    {

        animator.SetFloat("Speed", 0f);
        animator.SetBool("Grounded", false);
        if (collision.transform.CompareTag("RunnableWall"))
        {
            if (isLeftWall)
            {
                rb.AddForce(Vector3.up * _wallRunUpForce, ForceMode.Impulse);
                rb.AddForce(head.transform.right * _wallRunUpForce, ForceMode.Impulse);
                
            }
            if (isRightWall)
            {
                rb.AddForce(Vector3.up * _wallRunUpForce, ForceMode.Impulse);
                rb.AddForce(-head.transform.right * _wallRunUpForce, ForceMode.Impulse);
            }
            //cam.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

            // cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(0, 0, 0), 0.2f);
            StartCoroutine(AnimateRotationTowards(cam, Quaternion.identity, 0.2f));
            isWallRunning = false;
            rb.useGravity = true;
            player.doubleJump = true;
            
            

        }
    }
    IEnumerator AnimateRotationTowards(Transform target, Quaternion rot, float dur)
    {
        float t = 0f;
        Quaternion start = target.localRotation;
       
        while (t < dur)
        {
            target.localRotation = Quaternion.Slerp(start,rot , t / dur);
            yield return null;
            t += Time.deltaTime;
        }
        target.localRotation = rot;
    }



}
