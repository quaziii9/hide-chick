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
    public void AddKillLog(string killer, string killed, Sprite icon, Color iconColor)
    {
        // 최대 킬로그 개수를 초과하면 첫 번째 킬로그를 제거
        if (killLogItems.Count >= maxKillLogs)
        {
            GameObject oldestKillLog = killLogItems.Dequeue();
            Destroy(oldestKillLog);
        }

        // 새로운 킬로그 아이템 생성
        GameObject newKillLogItem = Instantiate(killLogItemPrefab, killLogContainer.transform);

        // KillLogItem 프리팹에서 텍스트와 이미지 컴포넌트 가져오기
        TextMeshProUGUI[] texts = newKillLogItem.GetComponentsInChildren<TextMeshProUGUI>();
        Image[] images = newKillLogItem.GetComponentsInChildren<Image>();
        RectTransform rectTransform = newKillLogItem.GetComponent<RectTransform>();

        // 텍스트 컴포넌트가 두 개, 이미지 컴포넌트가 하나 있는지 확인
        if (texts.Length == 2 && images.Length == 1)
        {
            texts[0].text = killer; // 첫 번째 TextMeshProUGUI에 killer 텍스트 할당
            texts[1].text = killed; // 두 번째 TextMeshProUGUI에 killed 텍스트 할당
            images[0].sprite = icon; // 이미지 컴포넌트에 아이콘 스프라이트 할당
            images[0].color = Color.red; // 아이콘 색상 설정
        }

        // 새로운 킬로그 아이템에 Layout Element 설정
        foreach (var text in texts)
        {
            LayoutElement layoutElement = text.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = text.gameObject.AddComponent<LayoutElement>();
            }
            // 텍스트의 preferredWidth를 사용하여 LayoutElement의 preferredWidth를 동적으로 설정
            layoutElement.minWidth = 100;
            layoutElement.preferredWidth = text.preferredWidth;
            layoutElement.flexibleWidth = 0;
        }

        // KillLogItem의 RectTransform 설정
        if (rectTransform != null)
        {
            float totalPreferredWidth = images[0].rectTransform.sizeDelta.x + texts[0].preferredWidth + texts[1].preferredWidth + 20; // 간격 포함
            rectTransform.sizeDelta = new Vector2(totalPreferredWidth, rectTransform.sizeDelta.y);
        }

        // 새로운 킬로그 아이템을 큐에 추가
        killLogItems.Enqueue(newKillLogItem);

        // 부모 컨테이너의 크기를 재조정
        LayoutRebuilder.ForceRebuildLayoutImmediate(killLogContainer.GetComponent<RectTransform>());
    }

    // 임의의 킬로그를 5초마다 추가하는 코루틴
    private IEnumerator AddRandomKillLogEveryFiveSeconds()
    {
        string[] playerNames = { "PlayerOne", "PlayerTwo", "PlayerThree", "PlayerFour", "PlayerFive" };

        while (true)
        {
            yield return new WaitForSeconds(1);

            // 임의의 플레이어 이름 선택
            string killer = playerNames[Random.Range(0, playerNames.Length)];
            string killed = playerNames[Random.Range(0, playerNames.Length)];

            // 임의의 색상 선택 (예: 빨간색)
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // 킬로그 추가
            AddKillLog(killer, killed, defaultIcon, randomColor);
        }
    }
}
