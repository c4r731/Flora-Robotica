using UnityEngine;
using System.Collections;

public class BeanLeaf : AbstractPlantLeaf {

    protected float energy;
    protected float efficiency;
    protected float max_size;
    protected float size_step;
    protected float max_age;
    protected float age_step;
    protected float cur_age;

    public BeanLeaf( MasterConfig mc ) :base( mc )
    {
        energy = mc.LeafEnergy;
        max_size = mc.LeafMaxSize;
        size_step = mc.LeafSizeStep;
        efficiency = mc.LeafEfficiency;
        max_age = mc.LeafMaxAge;
        age_step = mc.LeafAgingStep;
        cur_age = 0.0f;

    }

    private void optimizeHarvest( )
    {
        if (this.transform.localScale.x == max_size)
            growup( );

        Transform li;
        Vector3 pos = Vector3.zero;
        float max_light_intensity = 0.0f;
        RaycastHit hit;
        Ray ray;

        for (int i = 0; i < mc.Lights.Length; i++)
        {

            if (mc.Lights[i].transform.GetComponent<Light>( ).enabled)
                max_light_intensity += mc.Lights[i].transform.GetComponent<Light>( ).intensity;
        }

        for (int i = 0; i < mc.Lights.Length; i++)
        {
            li = mc.Lights[i].transform;
            if (!li.GetComponent<Light>( ).enabled)
                continue;


            bool hitflag = false;
            ray = new Ray( transform.GetChild( 0 ).transform.position, li.position - transform.GetChild( 0 ).transform.position );
            hitflag = mc.safeRaycast( ray, out hit, li.GetComponent<Light>( ).range );

            if (
                hitflag &&
                hit.collider.gameObject.GetComponent<Light>( ) != null
                )
            {
                if (pos == Vector3.zero)
                    pos = li.position;
                else
                    pos += pos * (li.GetComponent<Light>( ).intensity / max_light_intensity) - li.position * (li.GetComponent<Light>( ).intensity / max_light_intensity);
            }

            if (pos == Vector3.zero)
            {
                cur_age += age_step;
                Color tmp = this.transform.GetChild( 0 ).transform.GetComponent<Renderer>( ).material.color;
                tmp.r += 255 * age_step / max_age;
                energy = 0.0f;
                if (age_step == max_age)
                    dropLeaf( );
            }
            else
            {
                mc.safeLookRotation( pos );
                energy = li.GetComponent<Light>( ).intensity / max_light_intensity * efficiency * mc.LeafEnergy;
            }
        }
    }

    public override void harvest( )
    {
        optimizeHarvest( );
        if (energy > 0.0f)
            mc.LeafEnergy += energy;

    }

    private void dropLeaf( )
    {
        this.transform.GetComponent<Rigidbody>( ).useGravity = true;
        this.transform.GetComponent<Rigidbody>( ).isKinematic = false;
        this.transform.parent = mc.PatientZero.transform.parent;
        Invoke( "destroyLeaf", mc.DayCycleInSeconds*4 );
       
    }

    private void destroyLeaf( )
    {
        Destroy( this.gameObject );
    }

    private void growup( )
    {
        Vector3 tmp = this.transform.localScale;
        tmp.x += size_step;
        tmp.y += size_step;
        this.transform.localScale = tmp;
    }
}
