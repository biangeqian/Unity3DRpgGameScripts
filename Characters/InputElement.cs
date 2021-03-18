using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputElement 
{
    public bool risingEdge = false;
    public bool longPress = false;
    public bool fallingEdge = false;

    public void releaseEdges()
    {
        longPress = false;
        fallingEdge = true;
    }

    public void resetEdges()
    {
        risingEdge = false;
        fallingEdge = false;
    }
}
