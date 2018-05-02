using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FrogController : MonoBehaviour {

    public Rigidbody rb;
    Animator animator;

    Vector3 movementVector;

    public bool charging = false;
    public float MinCharge = 20;
    public float curCharge = 0;
    public float maxCharge = 50;
    public float chargeSpeed = 2;
    public float forwardModifier = 1;
    public float upwardsModifier = 1;
    public float backwardsTopSpeed = 5;
    public float backwardsSpeed = 3;

    Quaternion rotation = Quaternion.identity;
    private float groundedDistance;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rotation = Quaternion.identity;
        curCharge = MinCharge;
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out hit);
        groundedDistance = hit.distance;
    }

    bool isGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, groundedDistance);
        if(Vector3.Distance(hit.point, transform.position) < 0.1f)
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update () {
        float HSpeed = Input.GetAxis("Horizontal");
        float VSpeed = Input.GetAxis("Vertical");
        bool jumpDown = Input.GetKey(KeyCode.Space);
        animator.SetFloat("Speed",1);
        animator.SetBool("IsGrounded", isGrounded());
        if (isGrounded())
        {
            animator.SetFloat("Speed", 0);
            rotation *= Quaternion.Euler(new Vector3(0, HSpeed, 0));
            rb.MoveRotation(rotation);

            if (jumpDown)
            {
                if (curCharge < maxCharge)
                {
                    curCharge += Time.deltaTime * chargeSpeed;
                }

                charging = true;
            }
            else if (charging)
            {
                rb.AddForce(Vector3.up * curCharge * upwardsModifier, ForceMode.Impulse);
                rb.AddForce(transform.forward * curCharge * forwardModifier, ForceMode.Impulse);
                charging = false;
                curCharge = MinCharge;
            }else if(rb.velocity.magnitude < backwardsTopSpeed && VSpeed < 0)
            {
                rb.velocity = (transform.forward * VSpeed * backwardsSpeed * Time.deltaTime);
            }
        }
    }
}
