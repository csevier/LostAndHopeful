using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RemoveMenu : NetworkBehaviour
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        GameObject camera = GameObject.Find("Main Camera");
        if (camera != null)
        {
            Destroy(camera);
        }
        
        GameObject man = GameObject.Find("LonelyMan");
        if (man != null)
        {
            Destroy(man);
        }
        
        GameObject title = GameObject.Find("Title");
        if (title != null)
        {
            Destroy(title);
        }
    }
}
