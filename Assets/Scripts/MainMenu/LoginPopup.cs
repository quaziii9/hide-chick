using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Unity.VisualScripting;

public class LoginPopup : Singleton<LoginPopup>
{
    //
    // internal
    // 동일 어셈블리(프로젝트) 내에서만 접근가능

    [Header("UI")]


    [SerializeField] internal TextMeshProUGUI NickName;

    [SerializeField] internal Button Btn_StartAsHostServer;
    [SerializeField] internal Button Btn_StartAsClient;

    [SerializeField] internal TextMeshProUGUI Text_Error;


    public GameObject popup;


    protected override void Awake()
    {
        base.Awake();
        
        // instance를 현재 객체로 설정하여 싱글톤 인스턴스 초기화,
        // Loginpopup 클래스의 인스턴스가 하나만 존재하도록 보장
        //instance = this;
        Text_Error.gameObject.SetActive(false);
    }

    public void setnickName(string PlayerNickName)
    {
        NickName.text = PlayerNickName;
    }

    public void SetUIOnClientDisconnected()
    {
        popup.SetActive(true);
    }

    // 인증값이 변경될때 ui 업데이트
    public void SetUIOnAuthValueChanged()
    {
        Text_Error.text = string.Empty;
        Text_Error.gameObject.SetActive(false);
    }

    public void SetUIOnAuthError(string msg)
    {
        Text_Error.text = msg;
        Text_Error.gameObject.SetActive(true);
    }

    public void OnClick_StartAsHost()
    {

        var manager = NetworkManager.singleton as RoomManager;
        manager.StartHost();
        popup.SetActive(false);
    }

    public void OnClick_StartAsClient()
    {
        var manager = RoomManager.singleton;
        manager.StartClient();
        popup.SetActive(false);
    }
}
