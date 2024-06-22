using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField]Material material;
    private float hue=50;
    private float s=50;
    private float v=50;

    void Update()
    {
        //changes the color of the background overtime
        hue +=Time.deltaTime*10;
        if(hue>=360)hue=0;

        Color newColor = Color.HSVToRGB(hue/360, s/100, v/100);

        material.color = newColor;
    }
}
