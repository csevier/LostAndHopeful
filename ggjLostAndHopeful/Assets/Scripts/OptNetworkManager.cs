using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OptNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Character player = conn.identity.GetComponent<Character>();
        if (numPlayers == 1)
        {
            player.SetCharacterType("Hopeful"); 
        }
        else
        {
            player.SetCharacterType("Lost");
        }
        
        //Debug.Log($"New friend joined.");
    }
}
