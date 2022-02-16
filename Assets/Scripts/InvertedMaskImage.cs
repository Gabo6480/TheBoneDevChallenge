using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class InvertedMaskImage : Image
{
    public override Material materialForRendering { 
        get
        {
            Material temp = new Material(base.materialForRendering);
            temp.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return temp;
        }
    }
}
