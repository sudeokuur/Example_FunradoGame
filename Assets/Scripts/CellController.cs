using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    private Material[] cellColors;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        cellColors = GameObject.Find("GameController").GetComponent<GameController>().sendMeshes();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetCellColors(int a)
    {
        Material[] materials = meshRenderer.materials;
        
        switch (a)
        {
            case 0:
            materials[0] = cellColors[0];
            break;
            case 1:
            materials[0] = cellColors[1];
            break;
            case 2:
            materials[0] = cellColors[2];
            break;
            case 3:
            materials[0] = cellColors[3];
            break;
            case 4:
            materials[0] = cellColors[4];
            break;
            case 5:
            materials[0] = cellColors[5];
            break;
        }

        meshRenderer.materials = materials;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
