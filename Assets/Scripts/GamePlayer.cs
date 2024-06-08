using Mirror;
using UnityEngine;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar]
    public string PlayerName;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // 서버에서 로컬 플레이어의 닉네임을 설정합니다.
        if (isLocalPlayer)
        {
            PlayerName = Database.Instance.PlayerNickName;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // 클라이언트가 시작할 때 플레이어의 닉네임을 서버에 요청합니다.
        if (isLocalPlayer)
        {
            CmdSetPlayerName(Database.Instance.PlayerNickName);
        }
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        PlayerName = playerName;
    }
}
