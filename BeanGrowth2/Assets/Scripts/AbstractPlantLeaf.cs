using UnityEngine;
using System.Collections;

public abstract class AbstractPlantLeaf : MonoBehaviour {

    protected MasterConfig mc;


    public AbstractPlantLeaf( MasterConfig mc)
    {
        this.mc = mc;
    }


    public abstract void harvest( );


}
