using UnityEngine;
using Mirror;
using EnumTypes;
using EventLibrary;

public class WeaponCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            var attackedPlayer = other.GetComponentInParent<GamePlayer>();
            var attackingPlayer = GetComponentInParent<GamePlayer>();

            if (attackedPlayer != null && attackingPlayer != null)
            {
                EventManager<UIEvents>.TriggerEvent(UIEvents.addKillLog, attackingPlayer.PlayerName, attackedPlayer.PlayerName);
                Debug.Log($"{attackingPlayer.PlayerName} attacked {attackedPlayer.PlayerName}");
                EventManager<PlayerEvents>.TriggerEvent(PlayerEvents.WeaponColliderFalse);
                var attackedPlayerController = other.GetComponent<PlayerController>();
                attackedPlayerController.Die();
            }

            var attackedAI = other.GetComponent<AIController>();
            if (attackedAI != null)
            {
                EventManager<UIEvents>.TriggerEvent(UIEvents.addKillLog, attackingPlayer.PlayerName, "AI");
                Debug.Log("AI attacked and dying.");
                attackedAI.Die();
            }
        }
    }
}
