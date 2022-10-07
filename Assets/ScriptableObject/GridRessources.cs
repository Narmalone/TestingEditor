using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Créer grille data",menuName = "Data grille",order =0 )]
public class GridRessources : ScriptableObject
{
    public bool isDataCreated = false;
    public bool drawLines = true;
    public int lenght = 10;
    public int width = 5;
    public int lineLength = 300;
    public int LineStep = 1;

    public void DrawLines()
    {

    }
}
