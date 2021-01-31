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
                //tell client to tell server they picked up?
                // need a method on the player.
                // and call it here, is it a cmd? 
                Character c = other.GetComponent<Character>();
                c.OnOrbCollected();


                // Destory the object
                NetworkServer.Destroy(this.gameObject);
            }
        }
    }
}
