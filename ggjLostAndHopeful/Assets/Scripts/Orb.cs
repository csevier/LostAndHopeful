using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



[RequireComponent(typeof(Collider))]
public class Orb : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Player"))
        {
            if (isServer)
            {
                Character c = other.GetComponent<Character>();
                if (c.type == "Hopeful") return;
                c.OnOrbCollected();
                NetworkServer.Destroy(this.gameObject);
            }
        }
    }
}
