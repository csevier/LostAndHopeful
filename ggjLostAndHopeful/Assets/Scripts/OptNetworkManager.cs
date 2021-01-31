using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OptNetworkManager : NetworkManager
{
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        //Debug.Log($"New friend joined.");
    }
}
