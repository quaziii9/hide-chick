using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class NetworkingAuthenticator : NetworkAuthenticator
{
    readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> _playerNames = new HashSet<string>();

    public struct AuthReqMsg : NetworkMessage
    {
        public string authUserName;
    }

    public struct AuthResMsg : NetworkMessage
    {
        public byte code;
        public string message;
    }

    #region ServerSide
    [UnityEngine.RuntimeInitializeOnLoadMethod]
    static void ResetStatics()
    {
        _playerNames.Clear();
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }

    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }

    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
        // Do nothing...wait for AuthReqMsg from client
    }

    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        Debug.Log($"인증 요청 : {msg.authUserName}");

        if (_connectionsPendingDisconnect.Contains(conn)) return;

        if (!_playerNames.Contains(msg.authUserName))
        {
            Debug.Log("!!");
            _playerNames.Add(msg.authUserName);
            conn.authenticationData = msg.authUserName;

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 100,
                message = "Auth Success"
            };

            conn.Send(authResMsg);
            ServerAccept(conn);
        }
        else
        {
            _connectionsPendingDisconnect.Add(conn);

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 200,
                message = "User Name already in use! Try again!"
            };

            conn.Send(authResMsg);
            conn.isAuthenticated = false;

            StartCoroutine(DelayedDisconnect(conn, 1.0f));
        }
    }

    IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ServerReject(conn);

        yield return null;
        _connectionsPendingDisconnect.Remove(conn);
    }
    #endregion
}

