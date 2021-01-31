using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPlacer : MonoBehaviour
{
    public Terrain terrain;
    public GameObject orb;
    public float radius;
    public int amount;

    void Start()
    {
        CreateAreaOfOrbs();
    }

    void CreateAreaOfOrbs()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = transform.position;
            pos = new Vector3(pos.x + Random.Range(-radius, radius), 0.0f, pos.z + Random.Range(-radius, radius));
            pos.y = terrain.SampleHeight(pos) + terrain.transform.position.y;
            GameObject oneOrb = Instantiate(orb, pos, Quaternion.identity);
            // TODO? take collider y into account
            //oneOrb.transform.position += new Vector3(0.0f, oneOrb.transform.GetComponent<Collider>().bounds.size.y / 2, 0.0f);
        }
    }
}