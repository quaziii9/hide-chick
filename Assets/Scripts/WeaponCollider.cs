using UnityEngine;
using Mirror;
using EnumTypes;
using EventLibrary;

public class WeaponCollider : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            var attackedPlayer = other.GetComponentInParent<GamePlayer>();
            var attackingPlayer = GetComponentInParent<GamePlayer>();

            if (attackedPlayer != null && attackingPlayer != null)
            {
                CmdSendKillLog(attackingPlayer.PlayerName, attackedPlayer.PlayerName);
                EventManager<PlayerEvents>.TriggerEvent(PlayerEvents.WeaponColliderFalse);
                var attackingPlayerController = GetComponentInParent<PlayerController>();
                var attackedPlayerController = other.GetComponentInParent<PlayerController>();
                attackedPlayerController.Die();
                //RoomManager.Instance.PlayerKill(attackedPlayerController);
            }

            var attackedAI = other.GetComponentInParent<AIController>();
            if (attackedAI != null)
            {
                CmdSendKillLog(attackingPlayer.PlayerName, "AI");
                EventManager<PlayerEvents>.TriggerEvent(PlayerEvents.WeaponColliderFalse);
                attackedAI.Die();
            }
        }
    }

    [Command]
    private void CmdSendKillLog(string attacker, string victim)
    {
        RpcSendKillLog(attacker, victim);
    }

    [ClientRpc]
    private void RpcSendKillLog(string attacker, string victim)
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.addKillLog, attacker, victim);
    }
}
