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
                var attackedPlayerController = other.GetComponentInParent<PlayerController>();
                attackedPlayerController.Die();
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
        Debug.Log("COMMAND");
        RpcSendKillLog(attacker, victim);
    }

    [ClientRpc]
    private void RpcSendKillLog(string attacker, string victim)
    {
        Debug.Log("CLIENTRPC");

        EventManager<UIEvents>.TriggerEvent(UIEvents.addKillLog, attacker, victim);
    }
}
