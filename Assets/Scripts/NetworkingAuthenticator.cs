using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// conn(connection) : 클라이언트와 서버 간의 네트워크 연결을 나타내는 객체

public partial class NetworkingAuthenticator : NetworkAuthenticator
{
    // _connectionPendingDisconnect : 연결을 끊을 예정인 클라이언트 목록
    // _playerNames : 인증된 사용자 이름 목록
    readonly HashSet<NetworkConnection> _connectionPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> _playerNames = new HashSet<string>();


    // AuthReqMsg : 클라이언트가 보낸 인증 요청 메시지, 사용자 이름을 포함
    public struct AuthReqMsg : NetworkMessage
    {
        // 인증을 위해 사용
        // OAuth 같은걸 사용시 이 부분에 엑세스 토큰 같은 변수를 추가하면 됨
        public string authUserName;
    }

    // AuthResMsg : 서버가 보낸 인증 응답 메시지, 응답 코드와 메시지를 포함
    public struct AuthResMsg : NetworkMessage
    {
        public byte code;
        public string message;
    }

    #region ServerSide
    [UnityEngine.RuntimeInitializeOnLoadMethod]

    static void ResetStatics()
    {

    }


    // 서버가 시작될때 호출, AuthReqMsg 메시지를 처리할 핸들러를 등록
    // 핸들러 : 특정 이벤트나 작업이 발생했을때 이를 처리하는 코드
    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }

    // 서버가 멈출때 호출 AuthResMsg 메시지 핸들러를 등록 해제
    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }

    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
    }


    // 클라이언트가 인증 요청 메시지를 보낼때 호출
    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        // 클라 인증 요청 메세지 도착 시 처리
        Debug.Log($"인증 요청 : {msg.authUserName}");

        // 이미연결 대기 중인 클라이언트는 무시
        if (_connectionPendingDisconnect.Contains(conn)) return;

        // 웹서버 , DB, Playerfab API 등을 호출해 인증 확인
        // 새로운 사용자 이름인 경우 목록에 추가, 성공 메시지를 보낸 후 연결을 수락
        if(!_playerNames.Contains(msg.authUserName))
        {
            _playerNames.Add(msg.authUserName);

            // 대입한 인증 값은 Player.OnStartServer 시점에서 읽음
            conn.authenticationData = msg.authUserName;

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 100,
                message = "Auth Success"
            };

            conn.Send(authResMsg);
            ServerAccept(conn);
        }

        // 이미 사용중인 이름인 경우 실패 메시지를 보내고, 연결을 대기 목록에 추가
        else
        {
            _connectionPendingDisconnect.Add(conn);

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
        // 지정시간이 지난후 클라이언트 연결을 거부
        yield return new WaitForSeconds(waitTime);
        ServerReject(conn);

        // 대기 목록에서 해당 클라이언트 제거
        yield return null;
        _connectionPendingDisconnect.Remove(conn);
    }
    #endregion
}
