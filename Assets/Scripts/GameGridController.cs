using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameGridController : MonoBehaviour
{
    private int height;
    private int width;
    private float gridSpaceSize=1;
    private GameController gameController;
    [SerializeField]private GameObject gridCellPrefab;
    [SerializeField]private GameObject[] frogPrefab;
    [SerializeField]private GameObject[] grapePrefabs;
    [SerializeField]private GameObject[] arrowCellPrefabs;
    private GameObject[,] gameGrid;
    private Stack<GameObject>[,] inGameObjects;
    private GameObject[] frogs;
    [SerializeField]private int maxFrogCount;
    private System.Random random;

    private string[] borders;

    private Stack<int> destroyCordinates;

    private List<Vector3> startPoints;
    private List<Vector3> endPoints;

    void Start()
    {
        
    }

    //waits for the gameController before starting
    public void StartGrid(int a, int b, int seed=1234)
    {
        //using seeds the levels can be recreated if width,height and maxFrogCount is the same
        random = new System.Random(seed);

        width=a;
        height=b;
        
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        destroyCordinates = new Stack<int>();
        startPoints = new List<Vector3>();
        endPoints = new List<Vector3>();

        //creates the initial grid
        CreateGrid();

        //finds borders for the frogs to spawn
        FindBorders();

        //pushes the initial gameobjects up
        PushObjectsUp();
    }

    private void PushObjectsUp()
    {
        for (int y = 0; y<height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(inGameObjects[x,y].Count!= 0)
                {
                    inGameObjects[x,y].Peek().transform.localPosition += new Vector3(0,10,0);
                    int colorIndex = FindColor(inGameObjects[x,y].Peek().tag);
                    gameGrid[x,y].GetComponent<CellController>().SetCellColors(colorIndex);
                } 
            
            }
        }
    }

    //all the colors have the same integer indexes throughout the project
    private int FindColor(string str)
    {
        switch (str)
        {
            case "Blue":
            return 0;
            case "Green":
            return 1;
            case "Purple":
            return 2;
            case "Red":
            return 3;
            case "Yellow":
            return 4;
        }
        return -1;
    }

    //creates the grid
    private void CreateGrid()
    {
        //initializes the 2D stack array for the cells
        gameGrid = new GameObject[width, height];

        if(gridCellPrefab == null)
        {
            Debug.LogError("prefab empty");
            return;
        }

        for(int z = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
            {
                //instantiates a cell model for each grid space
                CreateCell(x,z);
                gameGrid[x,z].transform.parent = transform;
                gameGrid[x,z].name="X: " + x.ToString() + " Z: " + z.ToString(); 
            }
        }
        //gameGrid is used to get the cordinates
    }

    private void CreateCell(int a,int b)
    {
        //x and z are calculated using indexes to find correct positions
        gameGrid[a,b] = Instantiate(gridCellPrefab , new Vector3(a*gridSpaceSize , 0 , b*gridSpaceSize),Quaternion.identity);
        
    }

    //the loops travel along the border of the grid and save the cordinates
    private void FindBorders()
    {
        string[] leftSide = new string[height];
        
        for(int i=0; i<height; i++)
        {
            leftSide[i]= 0.ToString() + i.ToString();
        }

        string[] bottomSide = new string[width-1];

        for(int i=1; i<width; i++)
        {
            bottomSide[i-1]= i.ToString() + 0.ToString();
        }

        string[] topSide = new string[width-1];

        for(int i=1; i<width; i++)
        {
            topSide[i-1]= i.ToString() + (height-1).ToString();
        }

        string[] rightSide = new string[height-2];

        for(int i=1; i<height-1; i++)
        {
            rightSide[i-1]= (width-1).ToString() + i.ToString();
        }
        
        int sum = leftSide.Length+bottomSide.Length+topSide.Length+rightSide.Length;
        borders = new string[sum];

        leftSide.CopyTo(borders, 0);
        bottomSide.CopyTo(borders, leftSide.Length);
        topSide.CopyTo(borders, bottomSide.Length+leftSide.Length);
        rightSide.CopyTo(borders, topSide.Length+bottomSide.Length+leftSide.Length);

        
        CreateFrogs();
    }

    //initializes variables needed to use inGameObjects
    private void CreateFrogs()
    {
        inGameObjects = new Stack<GameObject>[width,height];
        for (int z= 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {   
                //Initializing each stack inside the cells
                inGameObjects[x, z] = new Stack<GameObject>();
            }
        }
        /*for (int y = 0; y<height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                inGameObjects[x,y].Push(new GameObject("Empty"));
            }
        }*/
        for(int i=0; i<maxFrogCount; i++)
        {
            InsRandomFrog();
        }
        
    }
    
    //instantiates a random colored frog, with a random cord inside borders and corrects the rotation
    private void InsRandomFrog()
    {
        
        string temp = borders[random.Next(0,borders.Length)];
        int randomx = temp[0]-48;
        int randomy = temp[1]-48;

        int no = random.Next(0,5);
        float correctRotation = CorrectRotation(randomx,randomy);
        inGameObjects[randomx,randomy].Push(Instantiate(frogPrefab[no], new Vector3(randomx,-5,randomy), Quaternion.Euler(0,correctRotation,0)));
        AddGrapes(correctRotation,randomx,randomy,no);
    }

    private float CorrectRotation(int a, int b)
    {
        float correctRotation=0;

        if(a == 0)
        {
            correctRotation=270;
        }

        if(b == 0)
        {
            correctRotation = 180; 
        }

        if(a == width-1)
        {
            correctRotation = 90;
        }

        if(b == height-1)
        {
            correctRotation = 0;
        }

        return correctRotation;
    }

    //instantiates grapes and arrows. Puts grapes in the direction the frog is facing
    //if a arrow gets added calls the overloaded AddGrapes function 
    private void AddGrapes(float rot,int a,int b,int no)
    {
        switch (rot)
        {
            //creates the grape line according to the frogs rotation
            case 0:
            for(int i = height-2; i >= 0; i--)
            {
                int intFlag = random.Next(0,10);
                //randomly checks if an arrow will be added
                if(intFlag == 0)
                {
                    float randomRotation = (int)random.Next(0,4)*90;
                    while(randomRotation == 0 || randomRotation == 180)
                    {
                        randomRotation = (int)random.Next(0,4)*90;
                        if(a==0)randomRotation=270;
                        if(a==width-1)randomRotation=90;
                    }
                    inGameObjects[a,i].Push(Instantiate(arrowCellPrefabs[no], gameGrid[a,i].transform.position + new Vector3(0,-5,0), Quaternion.Euler(0,randomRotation,0)));
                    //gives the rotation and cord of the arrow
                    AddGrapes(randomRotation,a,i,no,a,i);
                    break;
                }
                else
                {
                    inGameObjects[a,i].Push(Instantiate(grapePrefabs[no], gameGrid[a,i].transform.position + new Vector3(0,-5,0), Quaternion.identity));
                }
            }
            break;
            case 90:
            for(int i = width-2; i >= 0; i--)
            {
                int intFlag = random.Next(0,10);
                if(intFlag == 0)
                {
                    float randomRotation = (int)random.Next(0,4)*90;
                    while(randomRotation == 90 || randomRotation == 270)
                    {
                        randomRotation = (int)random.Next(0,4)*90;
                        if(b==0)randomRotation=180;
                        if(b==height-1)randomRotation=0;
                    }
                    inGameObjects[i,b].Push(Instantiate(arrowCellPrefabs[no], gameGrid[i,b].transform.position + new Vector3(0,-5,0), Quaternion.Euler(0,randomRotation,0)));
                    AddGrapes(randomRotation,i,b,no,i,b);
                    break;
                }
                else
                {
                    inGameObjects[i,b].Push(Instantiate(grapePrefabs[no], gameGrid[i,b].transform.position + new Vector3(0,-5,0), Quaternion.identity));
                }
            }
            break;
            case 180:
            for(int i = 1; i < height; i++)
            {
                int intFlag = random.Next(0,10);
                if(intFlag == 0)
                {
                    float randomRotation = (int)random.Next(0,4)*90;
                    while(randomRotation == 180 || randomRotation == 0)
                    {
                        randomRotation = (int)random.Next(0,4)*90;
                        if(a==0)randomRotation=270;
                        if(a==width-1)randomRotation=90;
                    }
                    inGameObjects[a,i].Push(Instantiate(arrowCellPrefabs[no], gameGrid[a,i].transform.position + new Vector3(0,-5,0), Quaternion.Euler(0,randomRotation,0)));
                    AddGrapes(randomRotation,a,i,no,a,i);
                    break;
                }
                else
                {
                    inGameObjects[a,i].Push(Instantiate(grapePrefabs[no], gameGrid[a,i].transform.position + new Vector3(0,-5,0), Quaternion.identity));
                }
            }
            break;
            case 270:
            for(int i = 1; i < width; i++)
            {
                int intFlag = random.Next(0,10);
                if(intFlag == 0)
                {
                    float randomRotation = (int)random.Next(0,4)*90;
                    while(randomRotation == 270 || randomRotation == 90)
                    {
                        randomRotation = (int)random.Next(0,4)*90;
                        if(b==0)randomRotation=180;
                        if(b==height-1)randomRotation=0;
                    }
                    inGameObjects[i,b].Push(Instantiate(arrowCellPrefabs[no], gameGrid[i,b].transform.position + new Vector3(0,-5,0), Quaternion.Euler(0,randomRotation,0)));
                    AddGrapes(randomRotation,i,b,no,i,b);
                    break;
                }
                else
                {
                    inGameObjects[i,b].Push(Instantiate(grapePrefabs[no], gameGrid[i,b].transform.position + new Vector3(0,-5,0), Quaternion.identity));
                } 
            }
            break;
        }
    }

    //instead of a frog, grape line now gets created according to the arrow 
    private void AddGrapes(float rot,int a,int b,int no,int widthMax, int heightMax)
    {
        switch (rot)
        {   
            //now the switch bases the rotation on the arrow object
            case 0:
            for(int i = heightMax-1; i >= 0; i--)
            {
                inGameObjects[a,i].Push(Instantiate(grapePrefabs[no], gameGrid[a,i].transform.position + new Vector3(0,-5,0), Quaternion.identity));
            }
            break;
            case 90:
            for(int i = widthMax-1; i >= 0; i--)
            {
                inGameObjects[i,b].Push(Instantiate(grapePrefabs[no], gameGrid[i,b].transform.position + new Vector3(0,-5,0), Quaternion.identity));
            }
            break;
            case 180:
            for(int i = heightMax+1; i < height; i++)
            {
                inGameObjects[a,i].Push(Instantiate(grapePrefabs[no], gameGrid[a,i].transform.position + new Vector3(0,-5,0), Quaternion.identity));
            }
            break;
            case 270:
            for(int i = widthMax+1; i < width; i++)
            {
                inGameObjects[i,b].Push(Instantiate(grapePrefabs[no], gameGrid[i,b].transform.position + new Vector3(0,-5,0), Quaternion.identity));
            }
            break;
        }
    }

    private bool flag;
    private bool arrowFlag;
    
    //after a frog is pressed checks if the line continues uninterrupted
    public void CheckFrog(int a, int b,float rot)
    {
        //one of the start points is added
        startPoints.Add(inGameObjects[a,b].Peek().transform.position+new Vector3(0,1,0));
        destroyCordinates.Push((a*10)+b);

        flag=true;
        arrowFlag=false;
        switch (rot)
        {
            case 0:
            for(int i = height-2; i >= 0; i--)
            {   
                //chekcs if the next object is arrow
                if(inGameObjects[a,i].Peek().layer == 8)
                {
                    arrowFlag = true;
                    endPoints.Add(inGameObjects[a,i].Peek().transform.position+new Vector3(0,1,0));

                    //if it is a arrow another overloaded function is called for the arrow
                    CheckFrog(a,i,inGameObjects[a,i].Peek().transform.rotation.eulerAngles.y,a,i);
                    break;
                }
                else if(inGameObjects[a,i].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    //correct grapes get added to the destroy stack
                    destroyCordinates.Push((a*10)+i);
                }
                else if(inGameObjects[a,i].Count== 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            if(!arrowFlag)endPoints.Add(inGameObjects[a,0].Peek().transform.position+new Vector3(0,1,0));
            break;
            case 90:
            for(int i = width-2; i >= 0; i--)
            {
                if(inGameObjects[i,b].Peek().layer == 8)
                {
                    arrowFlag=true;
                    endPoints.Add(inGameObjects[i,b].Peek().transform.position+new Vector3(0,1,0));
                    CheckFrog(i,b,inGameObjects[i,b].Peek().transform.rotation.eulerAngles.y,i,b);
                    break;
                }
                else if(inGameObjects[i,b].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    print("good");
                    destroyCordinates.Push((i*10)+b);
                }
                else if(inGameObjects[i,b].Count== 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            if(!arrowFlag)endPoints.Add(inGameObjects[0,b].Peek().transform.position+new Vector3(0,1,0));
            break;
            case 180:
            for(int i = 1; i < height; i++)
            {
                if(inGameObjects[a,i].Peek().layer == 8)
                {
                    arrowFlag=true;
                    endPoints.Add(inGameObjects[a,i].Peek().transform.position+new Vector3(0,1,0));
                    CheckFrog(a,i,inGameObjects[a,i].Peek().transform.rotation.eulerAngles.y,a,i);
                    break;
                }
                else if(inGameObjects[a,i].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    print("good");
                    destroyCordinates.Push((a*10)+i);
                }
                else if(inGameObjects[a,i].Count == 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            if(!arrowFlag)endPoints.Add(inGameObjects[a,height-1].Peek().transform.position+new Vector3(0,1,0));
            break;
            case 270:
            for(int i = 1; i < width; i++)
            {
                if(inGameObjects[i,b].Peek().layer == 8)
                {
                    arrowFlag=true;
                    endPoints.Add(inGameObjects[i,b].Peek().transform.position+new Vector3(0,1,0));
                    CheckFrog(i,b,inGameObjects[i,b].Peek().transform.rotation.eulerAngles.y,i,b);
                    break;
                }
                else if(inGameObjects[i,b].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    print("good");
                    destroyCordinates.Push((i*10)+b);
                }
                else if(inGameObjects[i,b].Count== 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            if(!arrowFlag)endPoints.Add(inGameObjects[width-1,b].Peek().transform.position+new Vector3(0,1,0));
            break;
        }
        DestroyObjects(flag);

    }

    //takes the arrow as base, everythng else works nearly the same  
    public bool CheckFrog(int a, int b,float rot,int widthMax, int heightMax)
    {
        startPoints.Add(inGameObjects[a,b].Peek().transform.position+new Vector3(0,0.2f,0));
        destroyCordinates.Push((a*10)+b);

        switch (rot)
        {
            case 0:
            print("case0");
            for(int i = heightMax-1; i >= 0; i--)
            {
                if(inGameObjects[a,i].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    print("good");
                    destroyCordinates.Push((a*10)+i);
                }
                else if(inGameObjects[a,i].Count
                 == 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            //adds the end point after all the grapes turn out the correct color
            if(flag)endPoints.Add(inGameObjects[a,0].Peek().transform.position+new Vector3(0,1,0));
            break;

            case 90:
            print("case-90");
            for(int i = widthMax-1; i >= 0; i--)
            {
                if(inGameObjects[i,b].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    print("good");
                    destroyCordinates.Push((i*10)+b);
                }
                else if(inGameObjects[i,b].Count
                 == 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            if(flag)endPoints.Add(inGameObjects[0,b].Peek().transform.position+new Vector3(0,1,0));
            break;

            case 180:
            print("case-180");
            for(int i = heightMax+1; i < height; i++)
            {
                if(inGameObjects[a,i].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    print("good");
                    destroyCordinates.Push((a*10)+i);
                }
                else if(inGameObjects[a,i].Count
                 == 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            if(flag)endPoints.Add(inGameObjects[a,height-1].Peek().transform.position+new Vector3(0,1,0));
            break;

            case 270:
            print("case-270");
            for(int i = widthMax+1; i < width; i++)
            {

                if(inGameObjects[i,b].Peek().tag == inGameObjects[a,b].Peek().tag)
                {
                    print("good");
                    destroyCordinates.Push((i*10)+b);
                }
                else if(inGameObjects[i,b].Count
                 == 0){}
                else
                {
                    flag=false;
                    break;
                }
            }
            if(flag)endPoints.Add(inGameObjects[width-1,b].Peek().transform.position+new Vector3(0,1,0));
            break;
        }

        return flag;
    }

    private void DestroyObjects(bool flag)
    {
        if (flag)
        {
            gameController.drawLines(startPoints, endPoints);
            
            //using Coroutine to adjust the pace at which objects get destroyed
            StartCoroutine(DestroyObjectsCoroutine());
        }
        else
        {
            //resets everytihng
            startPoints.Clear();
            endPoints.Clear();
            destroyCordinates.Clear();
            print("wrongmove");
            gameController.setCanClick(true);
        }
    }

private IEnumerator DestroyObjectsCoroutine()
{
    // Wait time is calculated according to how long the line is
    float waitTime = destroyCordinates.Count / 5;

    if (waitTime < 0.3f) waitTime = 0.3f;
    if (waitTime > 0.6f) waitTime = 0.6f;

    yield return new WaitForSeconds(waitTime);

    while (destroyCordinates.Count > 0)
    {
        // Pops and destroy the top object and shows the new object
        int cord = destroyCordinates.Pop();
        int b = cord % 10;
        int a = (cord - b) / 10;

        GameObject temp = inGameObjects[a, b].Pop();

        // Call ScaleAndDestroy coroutine to scale up the object before destroying it
        yield return StartCoroutine(ScaleAndDestroy(temp));

        PlaySound();

        if (inGameObjects[a, b].Count > 0)
        {
            inGameObjects[a, b].Peek().transform.position += new Vector3(0, 10, 0);
            ChangeCellColor(a, b);
        }
        else ChangeCellColor(a, b);

        // Yield a small delay before processing the next object destruction
        if (destroyCordinates.Count > 0) yield return new WaitForSeconds(0.2f); 
    }

    // Keeps track of frogs left as a win condition
    maxFrogCount--;

    if (maxFrogCount == 0)
    {
        gameController.openWinScreen();
    }
    else if (gameController.retrunRemainingClicks() == 0)
    {
        gameController.openLoseScreen();
    }

    startPoints.Clear();
    endPoints.Clear();
    CallDestroyLines();
    gameController.setCanClick(true);
}

// Coroutine to scale and destroy the object
private IEnumerator ScaleAndDestroy(GameObject obj)
{
    Vector3 originalScale = obj.transform.localScale;
    Vector3 targetScale = originalScale * 1.5f; // Scale up to 1.5 times the original size
    Vector3 originalPosition = obj.transform.position;
    Vector3 targetPosition = originalPosition + new Vector3(0, 1, 0); // Move upward by 1 unit

    float duration = 0.1f; // Duration of the animation
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;

        // Interpolate scale and position
        obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
        obj.transform.position = Vector3.Lerp(originalPosition, targetPosition, t);

        yield return null;
    }

    // Ensure the final scale and position are set
    obj.transform.localScale = targetScale;
    obj.transform.position = targetPosition;

    // Destroy the object
    Destroy(obj);
}

    
    public int sentMaxFrogCount()
    {
        return maxFrogCount;
    }
    private void PlaySound()
    {
        gameController.PlaySound();
    }

    private void CallDestroyLines()
    {
        gameController.destroyLines();
    }

    private void ChangeCellColor(int x, int y)
    {   
        if(inGameObjects[x,y].Count == 0)
        {
            gameGrid[x,y].GetComponent<CellController>().SetCellColors(5);
        }
        else
        {
            int colorIndex = FindColor(inGameObjects[x,y].Peek().tag);
            gameGrid[x,y].GetComponent<CellController>().SetCellColors(colorIndex);
        }
    }

    public float sendHeight()
    {
        return height;
    }
    public float sendWidth()
    {
        return width;
    }

}
