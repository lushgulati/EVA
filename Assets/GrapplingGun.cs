using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public bool bulletPresent;
    public bool shotgrapple;
    private bool grappleconnect;
    public Animator animator;

    void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        bulletPresent = false;
        StartGrapple();
        shotgrapple = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StartCoroutine(StopGrapple());
        }
        if (grappleconnect == true)
        {
            shotgrapple = false;
            GameEvents.current.EndGrapple();

        }
        if (shotgrapple == false)
        {
            GameEvents.current.EndGrapple();
        }
        

    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            if (hit.collider.CompareTag("Room") || hit.distance == maxDistance) 
            {
                grapplePoint = hit.point;
                lr.positionCount = 2;
                currentGrapplePosition = gunTip.position;

                Invoke("LateStop", 0.01f*hit.distance);
            }
            
            else
            {
                grapplePoint = hit.point;
                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;

                float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

                //The distance grapple will try to keep from grapple point. 
                joint.maxDistance = distanceFromPoint * 0.6f;
                joint.minDistance = distanceFromPoint * 0.1f;
                
                //Adjust these values to fit your game.
                joint.spring = 17f;
                joint.damper = 2.5f;
                joint.massScale = 4.5f;
                //shotgrapple = false;
                grappleconnect = true;
                lr.positionCount = 2;
                currentGrapplePosition = gunTip.position;
                bulletPresent = true;
                Invoke("GrapplePull", 0.3f);
            }

            
           

            
        }
    }


    void GrapplePull()
    {
        animator.SetTrigger("NotConfirm");
    }
    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    IEnumerator StopGrapple()
    {
        animator.SetTrigger("NotConfirm");
        shotgrapple = false;


        
        bulletPresent = false;
       
        yield return new WaitForSeconds(0.02f);
        lr.positionCount = 0;
        Destroy(joint);
        this.gameObject.SetActive(false);

    }
    void LateStop()
    {
        StartCoroutine(StopGrapple());
    }


    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        //if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 20f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}