using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{

    //How much health will it have?
    public int Health = 100;

    //How fast can it move/run?
    float maxSpeed = 2.0f;
    float speed = 0.0f;
    float sightRange = 12.0f; //How far can they see?

    //The position of the zombie.
    public Vector2 position = new Vector2(0.0f, 0.0f);

    //The direction vector of which way they are facing.
    Vector2 forward = new Vector2(1.0f, 0.0f);
    Vector2 target = new Vector2(0.0f, 0.0f);

    //A fixed array to contain all of it's movement array.
    Sprite[,] ZombieFrames = new Sprite[4, 2];

    //Two variables to handle when the zombie is free to change frame.
    int frame = 0;
    float nextFrame = 0.333f; //Going to make it take 1/3 of a second to change frame.

    float attackCooldown = 2.0f; //The zombie can attack every 2 seconds.

    // Start is called before the first frame update
    void Start()
    {
        //Load in all the frames.
        ZombieFrames[0, 0] = Resources.Load<Sprite>("Textures/Zombie/TowardA");
        ZombieFrames[0, 1] = Resources.Load<Sprite>("Textures/Zombie/TowardB");
        ZombieFrames[1, 0] = Resources.Load<Sprite>("Textures/Zombie/AwayA");
        ZombieFrames[1, 1] = Resources.Load<Sprite>("Textures/Zombie/AwayB");
        ZombieFrames[2, 0] = Resources.Load<Sprite>("Textures/Zombie/LeftA");
        ZombieFrames[2, 1] = Resources.Load<Sprite>("Textures/Zombie/LeftB");
        ZombieFrames[3, 0] = Resources.Load<Sprite>("Textures/Zombie/RightA");
        ZombieFrames[3, 1] = Resources.Load<Sprite>("Textures/Zombie/RightB");
    }

    public void Damage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Player.changeExpression(5, 1.0f);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        //Get the position and direction of the player!
        Vector2 playerPos = PlayerGlobals.position;
        Vector2 playerDir = new Vector2(Mathf.Sin(PlayerGlobals.direction * Mathf.Deg2Rad), Mathf.Cos(PlayerGlobals.direction * Mathf.Deg2Rad));

       if(speed > 0.0f)
        {
            //The zombie is free to change frame.
            nextFrame -= Time.deltaTime;
            if(nextFrame <= 0.0f)
            {
                frame++;
                if (frame > 1)
                    frame = 0;
                nextFrame = 0.333f;
            }
        }

        //Figure out which way they are facing.

       //I will be using the rightvector to determine left or right when it comes to side-view.
        Vector2 rightVector = new Vector2(forward.y, -forward.x);

        Vector2 toZombie = (position - playerPos);

        float dot = Vector2.Dot(playerDir, forward);

        if(dot <= -0.707106f)
        {
            //The zombie is facing us.
            gameObject.GetComponent<SpriteRenderer>().sprite = ZombieFrames[0, frame];
        }else if(dot < 0.707106f && dot > -0.707106f)
        {
            //The zombie is facing side-ways.
            //This is where the right vector, comes into play. It will be a simple -1 for left, or 1 for right from the dot product.
            dot = Vector2.Dot(playerDir, rightVector);
            if(dot < 0.0f)
                gameObject.GetComponent<SpriteRenderer>().sprite = ZombieFrames[3, frame];
            else
                gameObject.GetComponent<SpriteRenderer>().sprite = ZombieFrames[2, frame];
        }
        else
        {
            //The zombie is facing away from us.
            gameObject.GetComponent<SpriteRenderer>().sprite = ZombieFrames[1, frame];
        }


        Vector2 toPlayer = (playerPos - position);
        float distance = toPlayer.magnitude;

        bool isVisible = false;

        RayResult ray = MainScript.FireRay(position, toPlayer.normalized);
        if(ray.dist > distance && distance <= sightRange)
        {
            speed = maxSpeed;
            target = playerPos;
            isVisible = true;
        }



        if (isVisible)
        {
            float inFOV = Vector2.Dot(playerDir, toZombie.normalized);
            if (inFOV > Mathf.Cos((float)PlayerGlobals.FOV * 0.5f * Mathf.Deg2Rad))
            {
                float width = 290.0f;
                if (Vector2.Dot(new Vector2(playerDir.y, -playerDir.x), toZombie.normalized) < 0.0f)
                    width = -290.0f;

                float ratio = Mathf.Acos(inFOV) * Mathf.Rad2Deg / ((float)PlayerGlobals.FOV * 0.5f);

                gameObject.transform.localPosition = new Vector2(width * ratio, -60.0f / distance);

                gameObject.transform.localScale = new Vector2(320.0f / distance, 320.0f / distance);
            }
            else
            {
                gameObject.transform.localScale = new Vector2(0.0f, 0.0f);
            }
        }
        else
        {
            gameObject.transform.localScale = new Vector2(0.0f, 0.0f);
        }


        if((position - target).magnitude > 0.5f)
        {
            forward = (target - position).normalized;

            Vector2 velocity = forward * speed * Time.deltaTime;

            Vector2 attempt = position + velocity;
            if (!MainScript.hasCollided(attempt) && distance > 0.75f)
            {
                position = attempt;
            }
            else
            {
                attempt = position + new Vector2(velocity.x, 0.0f);
                if (!MainScript.hasCollided(attempt) && distance > 0.75f)
                {
                    position = attempt;
                }
                else
                {
                    attempt = position + new Vector2(0.0f, velocity.y);
                    if (!MainScript.hasCollided(attempt) && distance > 0.75f)
                        position = attempt;
                }
            }

        }
        else
        {
            speed = 0.0f;
        }

        //Handle attacking.
        if(attackCooldown <= 0.0f && distance <= 1.25f)
        {
            attackCooldown = 2.0f;
            Player.takeDamage(10);
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }

    }
}
