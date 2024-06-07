using UnityEngine;
using Mirror;

public class NetworkingManager : NetworkManager
{
    [SerializeField] LoginPopup _loginPopup;

    public void OnInputValueChanged_SetHostName(string hostName)
    {
        this.networkAddress = hostName;
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if (_loginPopup != null)
        {
            _loginPopup.SetUIOnClientDisconnected();
        }
    }

}
