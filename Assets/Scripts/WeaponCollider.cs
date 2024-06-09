using UnityEngine;
using Mirror;
using EnumTypes;
using EventLibrary;

public class WeaponCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
        }
    }
}
