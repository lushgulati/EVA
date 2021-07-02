using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public Animator animator;

    bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayhit;
    public LayerMask enemy;
    private Vector3 destination;
    public GameObject projectile;
    public float bulletForce = 1000f;
    private GameObject bullet;
    public Rigidbody playerRB;
    Vector3 aimspot;
    public Transform cam;
    public GameObject Grappling;
    GrapplingGun grapple;
    Vector3 refPos;
    public GameObject Player;
    PlayerMovement playerMovement;
    public ParticleSystem muzzle;
    private void Awake()
    {
        grapple = Grappling.GetComponent<GrapplingGun>();
        grapple.shotgrapple = false;
        bulletsLeft = magazineSize;
        readyToShoot = true;
        InvokeRepeating("Look", 0f, 0.07f);
        refPos = transform.localPosition;
        playerMovement = Player.GetComponent<PlayerMovement>();
        
    }

    private void Update()
    {
        
        Quaternion lookOnLook =
    Quaternion.LookRotation(aimspot - transform.position);

        transform.rotation =
        Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime);
        
        MyInput();
        if (Grappling)
        {
            if (grapple.shotgrapple == true) {
                animator.SetTrigger("Unequip");
                
                PutDown();
            }
        }
        if(!playerMovement.grounded)
        {
            AirShake();
        }
        else if(playerMovement.grounded)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition,refPos,Time.deltaTime*5);
        }
       
        
    }

    void AirShake()
    {
        float dx = 0.5f * (Mathf.PerlinNoise(Time.time * 1.2f, 1f) - 0.5f);
        float dy = 0.5f * (Mathf.PerlinNoise(1f, Time.time * 1.2f) - 0.5f);
        Vector3 pos = new Vector3(refPos.x, refPos.y, refPos.z);
        pos = pos + transform.up * dy;
        pos = pos + transform.right * dx;
        transform.localPosition = pos;
    }

    void PutDown()
    {
        
        
        this.gameObject.SetActive(false);
    }

    void Look()
    {
        
        aimspot = cam.position;
        aimspot += cam.forward * 50.0f;
        
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            ReadyToShoot();
        }
    }

    private void ReadyToShoot()
    {
        readyToShoot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point+new Vector3(x,y,0);
        else
            destination = ray.GetPoint(1000)+new Vector3(x, y, 0);
        Shoot();

    }

    void Shoot()
    {
        bullet = Instantiate(projectile, attackPoint.position, attackPoint.rotation);
        muzzle.Play();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Bullet bul = bullet.GetComponent<Bullet>();
        bul.damage = damage;

        rb.velocity = (destination - attackPoint.position).normalized * bulletForce + playerRB.velocity ;
        animator.SetTrigger("Fire");
        CinemachineShake.Instance.ShakeCamera(2f, 0.1f);
        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("ReadyToShoot", timeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (grapple.bulletPresent == false)
        {
            reloading = true;
            animator.SetTrigger("Reload");
            Invoke("ReloadFinished", reloadTime);
        }
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
