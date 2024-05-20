using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScripts : MonoBehaviour
{
      
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 10f;
    [SerializeField] Vector2 deathKick;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] float reloadGunTime = 0.2f;
    // COYOTE TIME
    [SerializeField] float coyoteTime = 0.3f;
    float coyoteTimer = 0f;
 
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    private Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;

    bool isAlive = true;
   

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        if (myAnimator == null)
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    // Handle the event movement using Input system
    void OnMove(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }

        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }

        bool isTouchingGround = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (isTouchingGround)
        {
            coyoteTimer = 0;
        }
        else
        {
            coyoteTimer += Time.deltaTime;
        }

        if (value.isPressed)
        {
            myRigidBody.velocity += new Vector2(0f, jumpSpeed);
        }      
    }

    void OnFire(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }

        StartCoroutine(ReloadGun());

        IEnumerator ReloadGun()
        {
            Instantiate(bullet, gun.position, transform.rotation);
            yield return new WaitForSecondsRealtime(reloadGunTime);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerHasHozSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("IsRunning", playerHasHozSpeed);
    }

    void FlipSprite()
    {
        // Avoid event when player at 0 speed
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1);
        }
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidBody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("IsClimbing", false);
            return;
        }
        

        myRigidBody.gravityScale = 0f;
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
        myRigidBody.velocity = climbVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("IsClimbing", playerHasVerticalSpeed);
    }

    void Die()
    {
        int randomX = Random.Range(-30, 30);
        int randomY = Random.Range(0, 30);
        deathKick = new Vector2(Random.Range(-30, 30), Random.Range(0, 30));

        if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidBody.velocity = deathKick;

            FindObjectOfType<GameManagement>().ProcessPlayerDeath();
        }


    }
}
