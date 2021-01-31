using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OrbPlacer : NetworkBehaviour
{
    public Terrain terrain;
    public GameObject orb;
    public float radius;
    public int amount;

    public override void OnStartServer()
    {
        base.OnStartServer();
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
            // Spawn the server instance on all clients
            NetworkServer.Spawn(oneOrb);
        }
    }
}