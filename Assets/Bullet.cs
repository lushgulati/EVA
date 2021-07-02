using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public LayerMask Enemies;
    //Rigidbody rb;

    public GameObject Impact;
    public float mSpeed = 10.0f;
    Vector3 mPrevPos;

    // Start is called before the first frame update
    void Start()
    {
        mPrevPos = transform.position;
    }


    private void Update()
    {
        
        //transform.Translate(0.0f, 0.0f, -mSpeed * Time.deltaTime);
        RaycastHit[] hits = Physics.RaycastAll(new Ray(mPrevPos, (transform.position - mPrevPos).normalized), (transform.position - mPrevPos).magnitude);
        mPrevPos = transform.position;
        for (int i = 0; i < hits.Length;i++)
            {
            Debug.Log(hits[i].collider.gameObject.name);
            Vector3 pos = hits[i].point;
            GameObject hit=Instantiate(Impact, pos, Quaternion.LookRotation(hits[i].normal));
            Destroy(this.gameObject);
            Destroy(hit, 4);
        }
        Destroy(this.gameObject, 5);
    }





    // Update is called once per frame
    /* void Update()
     {

         RaycastHit hit;
         if(Physics.Raycast(this.transform.position,-this.transform.forward,out hit, 3.5f,Enemies))
         {
             Debug.Log(hit.collider.gameObject.name);
             Destroy(this.gameObject);

         }
         Destroy(this.gameObject, 5);
     }*/

}
