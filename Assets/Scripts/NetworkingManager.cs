using UnityEngine;
using Mirror;

public class NetworkingManager : NetworkManager
{


    public void OnInputValueChanged_SetHostName(string hostName)
    {
        this.networkAddress = hostName;
    }

}
