using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {

    public float speed = 5f;
    private float activeSpeed = 0;
    public float multiplier = 2;
    public bool isblackSheep = false;
    private Rigidbody2D rbd;

    public float jumpSpeed;
    public bool isGrounded = false;

    public float groundDistance = 5f;
    public LayerMask groundMask;

    public Animator animator;

    public GameObject[] limbs;

    public Color dark;
    public Color lightColor;

    public float maxX = 8.5f;
    public Vector2 startPosition;
    public Quaternion startRotation;

    public bool startSheep;
    public bool startedSheep;

    public float blackChance = 30;

    public GameObject whiteSheep;
    public GameObject blackSheep;

    // Use this for initialization
    void Start ()
    {
        rbd = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        Reset();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.ToLower() == "fence")
        {
            if(animator != null)
            {
                animator.SetTrigger("Idle");
            }
            if(GameController.Instance.isGameOver == false)
            {
                GameController.Instance.GameOver();
            }
            
        }
    }

    public void Reset()
    {
        if(isblackSheep == false && GameController.Instance.score > 0)
        {
            if(Random.Range(0, 100) < blackChance)
            {
                isblackSheep = true;
            }
        } else
        {
            isblackSheep = false;
        }
        if(isblackSheep)
        {
            activeSpeed = speed * multiplier;
            whiteSheep.SetActive(false);
            blackSheep.SetActive(true);
        } else
        {
            activeSpeed = speed;
            whiteSheep.SetActive(true);
            blackSheep.SetActive(false);
        }
        rbd.velocity = Vector2.zero;

        transform.position = startPosition;
        transform.rotation = startRotation;
        

        /*
        for(var i = 0; i < limbs.Length; i++)
        {
            if(isblackSheep)
            {
                if(limbs[i].name.ToLower() == "body")
                {
                    limbs[i].GetComponent<SpriteRenderer>().color = dark;
                }
                else
                {
                    limbs[i].GetComponent<SpriteRenderer>().color = lightColor;
                }
            } else
            {
                if (limbs[i].name.ToLower() == "body")
                {
                    limbs[i].GetComponent<SpriteRenderer>().color = lightColor;
                }
                else
                {
                    limbs[i].GetComponent<SpriteRenderer>().color = dark;
                }
            }
        }
        */
    }

    // Update is called once per frame
    void FixedUpdate ()
    {

        if(GameController.Instance.isGameOver == false)
        {
            float speed = activeSpeed;
            if(isblackSheep == false)
            {
                speed += Random.Range(.35f, 1f);
            }
            rbd.velocity = new Vector2(speed , rbd.velocity.y);
           
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundMask);

            if (hit && hit.collider.gameObject.tag.ToLower() == "ground")
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        if(GameController.Instance.isGameOver == true && rbd.velocity != Vector2.zero)
        {
            rbd.velocity = new Vector2(0, rbd.velocity.y);
        }
        if(transform.position.x > maxX)
        {
            
            this.gameObject.SetActive(false);
            if (startSheep == false)
            {
                GameController.Instance.SleepSheep(this.gameObject);
            } else
            {
                GameController.Instance.ResetStartSheep();
            }
            
        }
    }    

    public void Jump()
    {
        if (isGrounded)
        {
            rbd.AddForce(new Vector2(rbd.velocity.x, jumpSpeed), ForceMode2D.Impulse);
            if(animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    private void Charge()
    {
        rbd.AddForce(new Vector2(-speed, jumpSpeed), ForceMode2D.Impulse);
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }

    public void ChargeAndStart()
    {
        print("CHARGEANDSTART:");
        if(startedSheep == false)
        {
            Charge();
            startedSheep = true;
        }
        
    }
}
