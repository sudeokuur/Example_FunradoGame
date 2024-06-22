using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]private Text remainingClicksText;
    [SerializeField]private int seed;
    [SerializeField]private float width;
    [SerializeField]private float height;
    [SerializeField]private Material[] coloredCells;
    [SerializeField]private int extraClicks=2;
    [SerializeField]private GameObject loseScreen;
    [SerializeField]private GameObject winScreen;
    private GameObject gameGrid;
    private GameGridController gameGridController;
    private AudioManager audioManager;
    private Camera mainCamera;

    private LineDrawer lineDrawer;

    private bool canClick=true;

    private int remainingClicks;

    // Start is called before the first frame update
    void Start()
    {
        
        lineDrawer = GameObject.Find("LineRenderer").GetComponent<LineDrawer>();

        //Initializing gameGrid object and gameGridController script
        gameGrid=GameObject.Find("GameGrid");
        gameGridController=gameGrid.GetComponent<GameGridController>();

        audioManager = GameObject.Find("AudioSource").GetComponent<AudioManager>();

        remainingClicks = gameGridController.sentMaxFrogCount()+extraClicks;
        remainingClicksText.text = remainingClicks.ToString();
        

        mainCamera=Camera.main;
        centerMainCamera();

        gameGridController.StartGrid((int)width,(int)height,seed);
       
    }

    void Update()
    {

    }

    //to ensure another frog cannot be pressed while another is collecting grapes
    public void setCanClick(bool flag)
    {
        canClick = flag;
    }
    public bool getCanClick()
    {
        return canClick;
    }

    public void PlaySound()
    {
        audioManager.PlaySoundEffect();
    }
    private void centerMainCamera()
    {
        float x = (width-1)/2;
        float z = (height-1)/2;
        float y = (x+z)/2;

        mainCamera.transform.position = new Vector3(x,15,z);
        mainCamera.orthographicSize = 8+y;
    }

    public Material[] sendMeshes()
    {
        return coloredCells;
    }

    //keeps track of how many clicks remain
    public void frogClicked(int a, int b,float rot)
    {
        remainingClicks--;
        remainingClicksText.text = remainingClicks.ToString();
        gameGrid.GetComponent<GameGridController>().CheckFrog(a,b,rot);
    }
    
    public void openWinScreen()
    {
        winScreen.SetActive(true);
    }

    public void openLoseScreen()
    {
        loseScreen.SetActive(true);
    }

    public void drawLines(List<Vector3> startPoints, List<Vector3> endPoints)
    {
        float duration = 0.3f; // Duration in seconds for each line
        lineDrawer.DrawMultipleLines(startPoints, endPoints, duration);
    }

    public void destroyLines()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");

        foreach (GameObject line in lines)
        {
            line.SetActive(false);
        }
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void mainMenu()
    {
        SceneManager.LoadScene("MainScreen");
    }

    public int retrunRemainingClicks()
    {
        return remainingClicks;
    }

}
