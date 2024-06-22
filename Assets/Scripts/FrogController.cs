using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FrogController : MonoBehaviour, IPointerClickHandler
{
    private GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        //checks if the player can click yet or not
        if(gameController.getCanClick())
        {
            gameController.setCanClick(false);
            gameController.frogClicked((int)transform.position.x,(int)transform.position.z,transform.eulerAngles.y);
        }
    }
}
