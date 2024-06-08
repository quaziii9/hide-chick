using UnityEngine;
using Mirror;

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
                Debug.Log($"{attackingPlayer.PlayerName} attacked {attackedPlayer.PlayerName}");

                // attackedPlayer 죽이기 로직 추가
                //attackedPlayer.Die();
            }
        }
    }
}
