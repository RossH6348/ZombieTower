using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGlobals
{

    //Going to need some variables, to determine position, direction, and FOV.
    public static Vector2 position = new Vector2(15.0f, 2.0f);
    public static float direction = 0.0f;
    public static int FOV = 70;

    public static int MaxHealth = 100;
    public static int Health = MaxHealth;

    public static int Ammo = 80; //How much ammo does the player have?
    public static int WeaponType = 0; //What weapon does the player have?
    public static float cooldown = 0.0f; //How much time left until they can fire again?

}


public class WeaponStats
{

    //Pistol stats.
    public static float[,] Weapons = new float[1, 2]
    {
        //The first float is the weapon's damage, and the second is the firerate.
        {15.0f, 0.225f }
    };


    //This list will hold the animation of the pistol firing.
    public static List<Sprite>[] WeaponsSprites = new List<Sprite>[1]{
        new List<Sprite>(){ }
    };

    //This list will hold the gunshot sounds of every weapon.
    public static List<AudioClip> WeaponsSound = new List<AudioClip>();

}


public class Player : MonoBehaviour
{

    float speed = 5.0f; //Gonna need movement speed too.

    float sensitivity = 240.0f; //Gonna make them be able to turn 60 degrees per second.

    GameObject Protag;
    GameObject Gun;

    public Text ammoDisplay;

    public static int currExpression = 0;
    public static float timeLeft = 0.0f; //How much time left before resettting to Idle expression.

    //This 2D list will contain the expressions and stages of the protagonist portrait.
    List<List<Sprite>> ProtagonistSprites = new List<List<Sprite>>();

    private AudioSource audio; //I will need this to play any sort of audio on the player.

    // Start is called before the first frame update
    void Start()
    {

        Protag = GameObject.Find("Protagonist");
        Gun = GameObject.Find("Gun");

        audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;

        //Load in all the protagonist sprites.
        for (int i = 0; i < 4; i++)
        {
            List<Sprite> ProtagonistExpress = new List<Sprite>();

            ProtagonistExpress.Add(Resources.Load<Sprite>("Textures/Protagonist/Idle" + i.ToString()));
            ProtagonistExpress.Add(Resources.Load<Sprite>("Textures/Protagonist/What" + i.ToString()));
            ProtagonistExpress.Add(Resources.Load<Sprite>("Textures/Protagonist/Right" + i.ToString()));
            ProtagonistExpress.Add(Resources.Load<Sprite>("Textures/Protagonist/Left" + i.ToString()));
            ProtagonistExpress.Add(Resources.Load<Sprite>("Textures/Protagonist/Hurt" + i.ToString()));
            ProtagonistExpress.Add(Resources.Load<Sprite>("Textures/Protagonist/Kill" + i.ToString()));

            ProtagonistSprites.Add(ProtagonistExpress);
        }

        //Load in the pistol sprites.
        WeaponStats.WeaponsSprites[0].Add(Resources.Load<Sprite>("Textures/Pistol/Idle"));

        //I am putting it in reverse, so I can start the animation at the last index, and smoothly go back to the idle.
        WeaponStats.WeaponsSprites[0].Add(Resources.Load<Sprite>("Textures/Pistol/Fire2"));
        WeaponStats.WeaponsSprites[0].Add(Resources.Load<Sprite>("Textures/Pistol/Fire1"));
        WeaponStats.WeaponsSprites[0].Add(Resources.Load<Sprite>("Textures/Pistol/Fire0"));

        //I am having to duplicate the frame again, cuz of how the animation system works...
        WeaponStats.WeaponsSprites[0].Add(Resources.Load<Sprite>("Textures/Pistol/Fire0"));

        //Adding the pistol gunshot sound.
        WeaponStats.WeaponsSound.Add(Resources.Load<AudioClip>("Sounds/PistolFire"));

    }

    //This function will be responsible for changing expressions.
    public static void changeExpression(int express, float duration)
    {
        currExpression = express;
        timeLeft = duration;
    }

    //While the player is taking damage, it will return true or false if the player reached 0.
    public static bool takeDamage(int damage)
    {
        PlayerGlobals.Health -= damage;
        if (PlayerGlobals.Health < 0)
        {
            PlayerGlobals.Health = 0;
            return true;
        }

        changeExpression(4, 0.5f);

        return false;
    }

    public static void addHealth(int amount)
    {
        PlayerGlobals.Health += amount;
        if (PlayerGlobals.Health > 100)
            PlayerGlobals.Health = 100;
    }


    // Update is called once per frame
    void Update()
    {

        //We need to calculate what our forward vector is, based on our current direction.
        //Luckily, this can be obtained by just using sin and cosine functions.
        Vector2 forward = new Vector2(Mathf.Sin(PlayerGlobals.direction * Mathf.Deg2Rad), Mathf.Cos(PlayerGlobals.direction * Mathf.Deg2Rad));

        forward *= speed * Time.deltaTime; //Scale the vector by the speed and deltaTime.

        //Just obtaining the left vector by swapping x and y of forward.
        Vector2 left = new Vector2(forward.y, -forward.x);

        //I will be using this vector to do "attempt" movements for collision reasons.
        Vector2 attempt = new Vector2(0.0f, 0.0f);

        if (Input.GetKey("w"))
        {

            //I am doing multiple attempts, to ensure smooth collision and movement.
            attempt = PlayerGlobals.position + forward;
            if (!MainScript.hasCollided(attempt))
            {
                PlayerGlobals.position = attempt;
            }
            else
            {
                attempt = PlayerGlobals.position + new Vector2(forward.x, 0.0f);
                if (!MainScript.hasCollided(attempt))
                {
                    PlayerGlobals.position = attempt;
                }
                else
                {
                    attempt = PlayerGlobals.position + new Vector2(0.0f, forward.y);
                    if (!MainScript.hasCollided(attempt))
                        PlayerGlobals.position = attempt;
                }
            }
            
        }

        if (Input.GetKey("s"))
        {
            attempt = PlayerGlobals.position - forward;
            if (!MainScript.hasCollided(attempt))
            {
                PlayerGlobals.position = attempt;
            }
            else
            {
                attempt = PlayerGlobals.position - new Vector2(forward.x, 0.0f);
                if (!MainScript.hasCollided(attempt))
                {
                    PlayerGlobals.position = attempt;
                }
                else
                {
                    attempt = PlayerGlobals.position - new Vector2(0.0f, forward.y);
                    if (!MainScript.hasCollided(attempt))
                        PlayerGlobals.position = attempt;
                }
            }
        }

        if (Input.GetKey("a"))
        {
            attempt = PlayerGlobals.position - left;
            if (!MainScript.hasCollided(attempt))
            {
                PlayerGlobals.position = attempt;
            }
            else
            {
                attempt = PlayerGlobals.position - new Vector2(left.x, 0.0f);
                if (!MainScript.hasCollided(attempt))
                {
                    PlayerGlobals.position = attempt;
                }
                else
                {
                    attempt = PlayerGlobals.position - new Vector2(0.0f, left.y);
                    if (!MainScript.hasCollided(attempt))
                        PlayerGlobals.position = attempt;
                }
            }
        }

        if (Input.GetKey("d"))
        {
            attempt = PlayerGlobals.position + left;
            if (!MainScript.hasCollided(attempt))
            {
                PlayerGlobals.position = attempt;
            }
            else
            {
                attempt = PlayerGlobals.position + new Vector2(left.x, 0.0f);
                if (!MainScript.hasCollided(attempt))
                {
                    PlayerGlobals.position = attempt;
                }
                else
                {
                    attempt = PlayerGlobals.position + new Vector2(0.0f, left.y);
                    if (!MainScript.hasCollided(attempt))
                        PlayerGlobals.position = attempt;
                }
            }
        }


        //Change the player direction, based on the mouse x axis movement.
        PlayerGlobals.direction += sensitivity * Time.deltaTime * Input.GetAxis("Mouse X");

        if (PlayerGlobals.direction < -180.0f)
            PlayerGlobals.direction += 360.0f;

        if (PlayerGlobals.direction > 180.0f)
            PlayerGlobals.direction -= 360.0f;

        //Put the mouse cursor back to the center of the screen, so that way the player can't drag their mouse off screen.
        Cursor.lockState = CursorLockMode.Locked;

        //All this bit will involve the gun.
        ammoDisplay.text = "Ammo: " + PlayerGlobals.Ammo.ToString();

        int frame = (int)((float)(WeaponStats.WeaponsSprites[PlayerGlobals.WeaponType].Count - 1) * (PlayerGlobals.cooldown / WeaponStats.Weapons[PlayerGlobals.WeaponType, 1]));

        Gun.GetComponent<SpriteRenderer>().sprite = WeaponStats.WeaponsSprites[PlayerGlobals.WeaponType][frame];

        if(PlayerGlobals.cooldown > 0.0f)
        {
            PlayerGlobals.cooldown -= Time.deltaTime;
            if (PlayerGlobals.cooldown <= 0.0f)
                PlayerGlobals.cooldown = 0.0f;
        }

        //Control for firing the gun.
        if(Input.GetMouseButtonDown(0) && PlayerGlobals.cooldown <= 0.0f && PlayerGlobals.Ammo > 0)
        {

            //Play the sound.
            audio.PlayOneShot(WeaponStats.WeaponsSound[PlayerGlobals.WeaponType]);

            //Begin firing animation.
            PlayerGlobals.cooldown = WeaponStats.Weapons[PlayerGlobals.WeaponType, 1];

            //Reduce ammo.
            PlayerGlobals.Ammo--;

            //Get our tile we have shot at, we will be using this to compare distance for later.
            RayResult ray = MainScript.FireRay(PlayerGlobals.position, forward.normalized);

            GameObject result = MainScript.FireBullet(PlayerGlobals.position, forward.normalized);

            if (result != null && (result.GetComponent<ZombieAI>().position - PlayerGlobals.position).magnitude < ray.dist) 
            {
                //Okay, we did get a zombie, and the intersection point is closer than the wall intersection point so we can damage this zombie.
                result.GetComponent<ZombieAI>().Damage((int)WeaponStats.Weapons[PlayerGlobals.WeaponType, 0]);
            }

        }

        //This entire bit just handles the cosmetic gag of the protagonist having expressions :)
        int stage = (int)((1.0f - ((float)PlayerGlobals.Health / (float)PlayerGlobals.MaxHealth)) * 4.0f);
        if (stage == 4)
            stage = 3;

        Protag.GetComponent<SpriteRenderer>().sprite = ProtagonistSprites[stage][currExpression];

        if(timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
        }
        else
        {
            timeLeft = 0.0f;
            currExpression = 0;
        }

    }
}
