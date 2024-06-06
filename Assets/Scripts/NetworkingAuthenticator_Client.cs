using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class NetworkingAuthenticator
{
    [SerializeField] LoginPopup _loginPopup;

    [Header("Client Username")]
    public string _playerName;

    public void OnInputValueChanged_SetPlayerName(string username)
    {
        _playerName = username;
        // UI를 업데이트
        _loginPopup.SetUIOnAuthValueChanged();
    }

    // 클라이언트가 시작될 때 인증 응답 메시지 핸들러를 등록
    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AuthResMsg>(OnAuthResponseMessage, false);
    }

    // 클라이언트가 종료될 때 인증 응답 메시지 핸들러를 등록 해제
    public override void OnStopClient()
    {
        NetworkClient.UnregisterHandler<AuthResMsg>();
    }

    // 클라에서 인증 요청 시 불려짐
    public override void OnClientAuthenticate()
    {
        // AuthReqMsg 메시지를 생성하여 authUserName 필드에 _playerName 값을 설정,
        // 이를 서버로 전송합니다.
        NetworkClient.Send(new AuthReqMsg { authUserName = _playerName });
    }

    public void OnAuthResponseMessage(AuthResMsg msg)
    {
        if(msg.code ==100) // 성공
        {
            Debug.Log($"Auto Response:{msg.code} {msg.message}");
            ClientAccept(); // 클라 인증 완료
        }
        else
        {
            Debug.LogError($"Auth Response : {msg.code} {msg.message}");
            NetworkManager.singleton.StopHost();

            _loginPopup.SetUIOnAuthError(msg.message);
        }
    }
}
