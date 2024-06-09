using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class KillLogManager : MonoBehaviour
{
    public GameObject killLogContainer; // KillLogContainer의 참조
    public GameObject killLogItemPrefab; // KillLogItem 프리팹의 참조
    public int maxKillLogs = 5; // 화면에 표시할 최대 킬로그 개수
    public Sprite defaultIcon; // 기본 아이콘 스프라이트

    private Queue<GameObject> killLogItems = new Queue<GameObject>(); // 킬로그 아이템들을 관리하기 위한 큐

    void Start()
    {
        // 5초마다 임의의 킬로그를 추가하는 코루틴 시작
        StartCoroutine(AddRandomKillLogEveryFiveSeconds());
    }

    // 새 킬로그를 추가하는 함수
    public void AddKillLog(string killer, string killed, Sprite icon)
    {
        // 최대 킬로그 개수를 초과하면 첫 번째 킬로그를 제거
        if (killLogItems.Count >= maxKillLogs)
        {
            Destroy(killLogItems.Dequeue());
        }

        // 새로운 킬로그 아이템 생성
        GameObject newKillLogItem = Instantiate(killLogItemPrefab, killLogContainer.transform);

        // KillLogItem 프리팹에서 텍스트와 이미지 컴포넌트 가져오기
        TextMeshProUGUI[] texts = newKillLogItem.GetComponentsInChildren<TextMeshProUGUI>();
        Image[] images = newKillLogItem.GetComponentsInChildren<Image>();

        // 텍스트 컴포넌트가 두 개, 이미지 컴포넌트가 하나 있는지 확인
        if (texts.Length == 2 && images.Length == 1)
        {
            texts[0].text = killer; // 첫 번째 TextMeshProUGUI에 killer 텍스트 할당
            texts[1].text = killed; // 두 번째 TextMeshProUGUI에 killed 텍스트 할당
            images[0].sprite = icon; // 이미지 컴포넌트에 아이콘 스프라이트 할당
        }

        // 새로운 킬로그 아이템을 큐에 추가
        killLogItems.Enqueue(newKillLogItem);
    }

    // 임의의 킬로그를 5초마다 추가하는 코루틴
    private IEnumerator AddRandomKillLogEveryFiveSeconds()
    {
        string[] playerNames = { "PlayerOne", "PlayerTwo", "PlayerThree", "PlayerFour", "PlayerFive" };

        while (true)
        {
            yield return new WaitForSeconds(5);

            // 임의의 플레이어 이름 선택
            string killer = playerNames[Random.Range(0, playerNames.Length)];
            string killed = playerNames[Random.Range(0, playerNames.Length)];

            // 킬로그 추가
            AddKillLog(killer, killed, defaultIcon);
        }
    }
}
