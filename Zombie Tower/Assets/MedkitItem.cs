using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedkitItem : MonoBehaviour
{

    public Vector2 position = new Vector2(0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 playerPos = PlayerGlobals.position;
        Vector2 playerDir = new Vector2(Mathf.Sin(PlayerGlobals.direction * Mathf.Deg2Rad), Mathf.Cos(PlayerGlobals.direction * Mathf.Deg2Rad));

        Vector2 toMedkit = (position - playerPos);
        Vector2 toPlayer = (playerPos - position);
        float distance = toPlayer.magnitude;

        RayResult ray = MainScript.FireRay(position, toPlayer.normalized);
        if (ray.dist > distance)
        {
            float inFOV = Vector2.Dot(playerDir, toMedkit.normalized);
            if (inFOV > Mathf.Cos((float)PlayerGlobals.FOV * 0.5f * Mathf.Deg2Rad))
            {
                float width = 290.0f;
                if (Vector2.Dot(new Vector2(playerDir.y, -playerDir.x), toMedkit.normalized) < 0.0f)
                    width = -290.0f;

                float ratio = Mathf.Acos(inFOV) * Mathf.Rad2Deg / ((float)PlayerGlobals.FOV * 0.5f);

                gameObject.transform.localPosition = new Vector2(width * ratio, -180.0f / distance);

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

        if(distance < 0.5f)
        {
            Player.addHealth(50);
            Destroy(gameObject);
        }

    }
}
