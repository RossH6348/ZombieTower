using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This will be used alot...
public class RayResult
{
    public int tileID;
    public int side;
    public float dist;
    public float uv;
    public int MapX;
    public int MapY;
}


public class MainScript : MonoBehaviour
{

    //We are going to need our canvas object.
    Canvas canvas;
    public GameObject FOVTemplate;
    public GameObject YouWon;
    public GameObject YouDied;
    public GameObject ZombieTemplate;
    public GameObject MedkitTemplate;
    List<GameObject> Images = new List<GameObject>();

    List<float> ImagesPosition = new List<float>();

    static List<TileData> TileDatas = new List<TileData>();

    private void Start()
    {
        canvas = this.GetComponent<Canvas>();

        //We are going to need to create a bunch of image components to use to draw our world.
        //This is to try to eliminate the constant need of destroying and instantiating new image components...

        int size = Mathf.RoundToInt((canvas.pixelRect.width / (float)PlayerGlobals.FOV)); //We going to need to round this though...

        int layer = 1;

        int startX = -80;

        //Here I am just cloning the FOVTemplate over and over, to make the field of view columns.
        //Then putting them in an array for easy updating.
        for (int i = 0; i <= PlayerGlobals.FOV; i++)
        {

            GameObject imgObj = Instantiate(FOVTemplate, canvas.transform.Find("Scene").transform, false);
            imgObj.name = "FOVImage " + i.ToString();

            imgObj.transform.localPosition = new Vector3(startX + size * i + 3.5f, 0.0f, 0.0f);

            imgObj.transform.localScale = new Vector3(3000.0f, 3000.0f, 1.0f);

            //Image img = imgObj.GetComponent<Image>();

            //img.rectTransform.sizeDelta = new Vector2(480.0f, canvas.pixelRect.height);

            //These four lines are extremely important, this is to stop their sprite masks from affecting one another...
            imgObj.GetComponent<SpriteRenderer>().sortingOrder = layer;
            imgObj.GetComponentInChildren<SpriteMask>().isCustomRangeActive = true;
            imgObj.GetComponentInChildren<SpriteMask>().frontSortingOrder = layer;
            imgObj.GetComponentInChildren<SpriteMask>().backSortingOrder = layer - 1;

            imgObj.SetActive(false); //Make them invisible for now, cuz we aren't drawing anything yet.

            Images.Add(imgObj);

            ImagesPosition.Add(imgObj.transform.localPosition.x);

            layer++;

        }

        //Cobble
        TileData Cobble = new TileData();
        Cobble.texture = Resources.Load<Sprite>("Textures/Tileset/Cobble");
        Cobble.solid = true;
        TileDatas.Add(Cobble);

        //Stone
        TileData Stone = new TileData();
        Stone.texture = Resources.Load<Sprite>("Textures/Tileset/Stone");
        Stone.solid = true;
        TileDatas.Add(Stone);

        //GrayBricks
        TileData GrayBricks = new TileData();
        GrayBricks.texture = Resources.Load<Sprite>("Textures/Tileset/GrayBricks");
        GrayBricks.solid = true;
        TileDatas.Add(GrayBricks);

        //BloodedCobble
        TileData BloodedCobble = new TileData();
        BloodedCobble.texture = Resources.Load<Sprite>("Textures/Tileset/BloodedCobble");
        BloodedCobble.solid = true;
        TileDatas.Add(BloodedCobble);

        //BloodedStone
        TileData BloodedStone = new TileData();
        BloodedStone.texture = Resources.Load<Sprite>("Textures/Tileset/BloodedStone");
        BloodedStone.solid = true;
        TileDatas.Add(BloodedStone);

        //BloodedGrayBricks
        TileData BloodedGrayBricks = new TileData();
        BloodedGrayBricks.texture = Resources.Load<Sprite>("Textures/Tileset/BloodedGrayBricks");
        BloodedGrayBricks.solid = true;
        TileDatas.Add(BloodedGrayBricks);

        //MossyCobble
        TileData MossyCobble = new TileData();
        MossyCobble.texture = Resources.Load<Sprite>("Textures/Tileset/MossyCobble");
        MossyCobble.solid = true;
        TileDatas.Add(MossyCobble);

        //Doorway
        TileData Doorway = new TileData();
        Doorway.texture = Resources.Load<Sprite>("Textures/Tileset/Doorway");
        Doorway.solid = false;
        TileDatas.Add(Doorway);

        //Stairs
        TileData Stairs = new TileData();
        Stairs.texture = Resources.Load<Sprite>("Textures/Tileset/Stairs");
        Stairs.solid = false;
        TileDatas.Add(Stairs);

        //CrackedStairs
        TileData CrackedStairs = new TileData();
        CrackedStairs.texture = Resources.Load<Sprite>("Textures/Tileset/CrackedStairs");
        CrackedStairs.solid = false;
        TileDatas.Add(CrackedStairs);

        //Planks
        TileData Planks = new TileData();
        Planks.texture = Resources.Load<Sprite>("Textures/Tileset/Planks");
        Planks.solid = true;
        TileDatas.Add(Planks);

        //PuncturedPlanks
        TileData PuncturedPlanks = new TileData();
        PuncturedPlanks.texture = Resources.Load<Sprite>("Textures/Tileset/PuncturedPlanks");
        PuncturedPlanks.solid = true;
        TileDatas.Add(PuncturedPlanks);

        CreateZombie(new Vector2(3.0f, 2.0f));
        CreateZombie(new Vector2(7.0f, 2.0f));


        CreateZombie(new Vector2(17.0f, 9.0f));

        CreateZombie(new Vector2(23.0f, 7.0f));


        CreateZombie(new Vector2(28.0f, 7.0f));

        CreateZombie(new Vector2(28.0f, 16.0f));
        CreateZombie(new Vector2(28.0f, 23.0f));
        CreateZombie(new Vector2(28.0f, 30.0f));


        CreateZombie(new Vector2(20.0f, 30.0f));
        CreateZombie(new Vector2(15.0f, 26.0f));
        CreateZombie(new Vector2(10.0f, 30.0f));
        CreateZombie(new Vector2(5.0f, 26.0f));

        CreateZombie(new Vector2(17.0f, 16.0f));


        CreateMedkit(new Vector2(23.5f, 10.5f));
        CreateMedkit(new Vector2(16.5f, 16.5f));

        YouWon.SetActive(false);
        YouDied.SetActive(false);

    }


    //Going to create two functions that will make spawning medkits and zombies much easier.
    public void CreateZombie(Vector2 position)
    {
        GameObject newZombie = Instantiate(ZombieTemplate, canvas.transform, false);
        newZombie.GetComponent<ZombieAI>().position = position;
        newZombie.SetActive(true);
    }

    public void CreateMedkit(Vector2 position)
    {
        GameObject newMedkit = Instantiate(MedkitTemplate, canvas.transform, false);
        newMedkit.GetComponent<MedkitItem>().position = position;
        newMedkit.SetActive(true);
    }


    //For this section here, it will be containing all the functions reponsible for raycasting and drawing.
    void drawColumn(int FOV, int tileID, float U, float distance, int side)
    {

        //-0.079 is 0.0, and 0.079 is 1.0
        float realU = -0.079f + (0.158f * U);

        float newPosition = ImagesPosition[FOV] - (472.0f * U);

        GameObject image = Images[FOV];

        image.transform.localPosition = new Vector3(newPosition - 1.75f, 0.0f, distance);

        image.GetComponentInChildren<SpriteMask>().transform.localPosition = new Vector3(realU, 0.0f, 0.0f);

        image.transform.localScale = new Vector3(3000.0f, 3000.0f * (1.0f / distance), 1.0f);

        image.SetActive(true);

        image.GetComponent<SpriteRenderer>().sprite = TileDatas[tileID - 1].texture;

        //Update the colour of this sprite, determined by side value, to get basic lighting.
        image.GetComponent<SpriteRenderer>().color = side != 0 ? new Color(1.0f, 1.0f, 1.0f, 1.0f) : new Color(0.5f, 0.5f, 0.5f, 1.0f);

    }


    public static RayResult FireRay(Vector2 rayPos, Vector2 rayDir)
    {
        RayResult ray = new RayResult();

        //Give the struct default values.
        ray.tileID = 0;
        ray.dist = 0.0f;
        ray.uv = 0.0f;

        //First find which cell the ray starts at.
        int MapX = (int)rayPos.x;
        int MapY = (int)rayPos.y;

        //We need to keep track of the overall distance in each axis.
        float DistX = 0.0f;
        float DistY = 0.0f;

        //Just splitting the ray direction vector, it will make it faster by reducing memory overhead I think?
        float RayDirX = rayDir.x;
        float RayDirY = rayDir.y;

        //Delta
        float deltaX = Mathf.Abs(1.0f / RayDirX);
        float deltaY = Mathf.Abs(1.0f / RayDirY);

        //This basically just to keep track which cell we are travesing in X and Y when intersecting.
        int stepX = 0;
        int stepY = 0;


        //The following is just figuring out the two step values and dist.

        //Do X first.
        if(RayDirX < 0)
        {
            stepX = -1;
            DistX = (rayPos.x - MapX) * deltaX;
        }
        else
        {
            stepX = 1;
            DistX = (MapX + 1.0f - rayPos.x) * deltaX;
        }

        //Now do Y.
        if (RayDirY < 0)
        {
            stepY = -1;
            DistY = (rayPos.y - MapY) * deltaY;
        }
        else
        {
            stepY = 1;
            DistY = (MapY + 1.0f - rayPos.y) * deltaY;
        }


        //We need to keep track of which side it has hit, 0 being X and Y being 1.
        int side = 0;

        //Now the main DDA ray travesal algorithm.
        while(ray.tileID == 0)
        {
            if(DistX < DistY)
            {
                //Travel in X if X is shorter than Y
                DistX += deltaX;
                MapX += stepX;
                side = 0;
            }
            else
            {
                //Else-wise, travel in Y.
                DistY += deltaY;
                MapY += stepY;
                side = 1;
            }

            if(MapX > -1 && MapX < 32 && MapY > -1 && MapY < 32)
            {
                //Update what our current tileID is, since we found a tile within the world.
                ray.tileID = Levels.LevelData[Levels.Level, MapY, MapX];
            }
            else
            {
                break; //Fail outside the map.
            }
        }

        //Compute the proper real distance.
        if (side == 0)
            ray.dist = (MapX - rayPos.x + (1 - stepX) / 2.0f) / RayDirX;
        else
            ray.dist = (MapY - rayPos.y + (1 - stepY) / 2.0f) / RayDirY;

        //Compute uv for texturing.
        if(side == 0)
        {
            ray.uv = (rayPos.y + rayDir.y * ray.dist);
            ray.uv -= Mathf.Floor(ray.uv);
        }
        else
        {
            ray.uv = (rayPos.x + rayDir.x * ray.dist);
            ray.uv -= Mathf.Floor(ray.uv);
        }

        ray.side = side; //Need to keep track of this side value, this so I can do some basic lighting on the tiles.

        ray.MapX = MapX;
        ray.MapY = MapY;

        return ray;
    }


    //This function is a much simplier raycast, this one is ray-circle intersection, since the zombies are going to have circle hitboxes.
    public static GameObject FireBullet(Vector2 rayPos, Vector2 rayDir)
    {

        GameObject enemy = null;

        float closest = 999999.0f; //To keep track which enemy is closest to us.

        //Loop through whatever the function returns a list of zombies as.
        foreach(GameObject zombie in GameObject.FindGameObjectsWithTag("ZombieTag"))
        {

            //Get the zombie position.
            Vector2 zombiePos = zombie.GetComponent<ZombieAI>().position;

            //Produce a vector going from position to the zombie.
            Vector2 toZombie = (zombiePos - rayPos);

            //Produce dot product to get the length of that vector.
            float dot = toZombie.x * rayDir.x + toZombie.y * rayDir.y;

            Vector2 point = new Vector2(rayPos.x + rayDir.x * dot, rayPos.y + rayDir.y * dot);

            //Now we got the point, let see if that point is close enough to the zombie with a radius hit box of 0.5f
            float distance = (zombiePos - point).magnitude;

            if(distance <= 0.5f)
            {
                //Okay it is, produce the two t values.
                float dt = Mathf.Sqrt(0.5f * 0.5f - distance * distance);

                float t0 = dot - dt;
                float t1 = dot + dt;

                if (t1 < t0)
                    t0 = t1; //Set t0 to t1 instead, cuz it is closer.

                //Okay now we got the closest t value, let compare it to our closest value.
                if(t0 < closest)
                {
                    //Okay this zombie hitbox intersected is closer.
                    enemy = zombie;
                    closest = t0;
                }
            }
        }

        return enemy;

    }


    //this function is to see if entities have collided, and if so return true.
    public static bool hasCollided(Vector2 position) {

        //First create four boundary points, creating a box.
        Vector2 posA = position + new Vector2(-0.25f, -0.25f);
        Vector2 posB = position + new Vector2(0.25f, 0.25f);
        Vector2 posC = position + new Vector2(0.25f, -0.25f);
        Vector2 posD = position + new Vector2(-0.25f, 0.25f);

        //Find out which cell are they currently in?
        int MapX = (int)position.x;
        int MapY = (int)position.y;

        //Now do a scanline approach, going from top left corner to bottom right corner neighbours.
        //And compare the boundaries!

        //This looks like a mess of code just to get collision, but it has a huge advantage of only checking 8 tiles at a time.
        //Meaning regardless of HOW LARGE the map is, it will never ever lose performance.
        for (int x = -1; x < 2; x++)
        {
            for(int y = -1; y < 2; y++)
            {

                if (x == 0 && y == 0)
                    continue; //Why would you check the cell you are in???

                int cellX = MapX + x;
                int cellY = MapY + y;

                //Before we do check, let make sure it is not going out of bound.
                if(cellX > -1 && cellX < 32 && cellY > -1 && cellY < 32)
                {
                    //One more check, is it a solid block? After checking it is not 0.
                    if (Levels.LevelData[Levels.Level, cellY, cellX] != 0 && TileDatas[Levels.LevelData[Levels.Level, cellY, cellX] - 1].solid)
                    {
                        //Okay we can check if either point lies inside a block.
                        if (posA.x >= (float)cellX && posA.x < (float)cellX + 1.0f && posA.y >= (float)cellY && posA.y < (float)cellY + 1.0f)
                            return true; //YES WE ARE COLLIDING!
                        else if (posB.x >= (float)cellX && posB.x < (float)cellX + 1.0f && posB.y >= (float)cellY && posB.y < (float)cellY + 1.0f)
                            return true; //SAME THING, EXCEPT FOR THE OTHER CORNER.
                        else if (posC.x >= (float)cellX && posC.x < (float)cellX + 1.0f && posC.y >= (float)cellY && posC.y < (float)cellY + 1.0f)
                            return true; //I think you get the idea by now.
                        else if (posD.x >= (float)cellX && posD.x < (float)cellX + 1.0f && posD.y >= (float)cellY && posD.y < (float)cellY + 1.0f)
                            return true;
                    }
                }
            }
        }

        return false; //If they pass all the test, then they can move.

    }

    float expressionTime = 10.0f;

    private void Update()
    {

        //Get the starting field of FOV, and the ending field of FOV angles.
        float startFOV = PlayerGlobals.direction - (float)(PlayerGlobals.FOV / 2);
        float endFOV = PlayerGlobals.direction + (float)(PlayerGlobals.FOV / 2);

        int realFOV = 0;

        //Starting from left to right, begin raycasting out toward the world.
        while(startFOV <= endFOV)
        {

            RayResult ray = FireRay(PlayerGlobals.position, new Vector2(Mathf.Sin(startFOV * Mathf.Deg2Rad), Mathf.Cos(startFOV * Mathf.Deg2Rad)));

            if (ray.tileID != 0)
                drawColumn(realFOV, ray.tileID, ray.uv, ray.dist * Mathf.Cos((PlayerGlobals.direction - startFOV) * Mathf.Deg2Rad), ray.side);
            else
                Images[realFOV].SetActive(false);

            realFOV++;
            startFOV++;
        }

        expressionTime -= Time.deltaTime;
        if(expressionTime <= 0.0f && Player.currExpression == 0)
        {
            expressionTime = (float)Random.Range(5, 10);
            Player.changeExpression(Random.Range(1, 3), 2.0f);
        }


        int MapX = (int)PlayerGlobals.position.x;
        int MapY = (int)PlayerGlobals.position.y;

        if(MapX == 6 && MapY == 1)
        {
            //THEY WON!
            YouWon.SetActive(true);
            Application.Quit();
        }else if(PlayerGlobals.Health <= 0)
        {
            GameOver();
        }

    }
    
    void GameOver()
    {
        YouDied.SetActive(true);
        Application.Quit();
    }


}
