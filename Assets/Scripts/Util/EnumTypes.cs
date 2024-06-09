using UnityEngine;

namespace EnumTypes
{


    public enum GlobalEvents
    {
        PlayerDead,
        PlayerSpawned,
        PlayerInactive,
        PlayerDamaged,
    }


    public enum SoundEvents
    {
        FadeIn,
        FadeOut,
        MineBgmPlay,
    }

    public enum UIEvents
    {
        atkTime,
        addKillLog
    }


    public enum PlayerEvents
    {
        isAtkTrue,
        isAtkFalse
    }

    public enum SceneType
    {
        Intro,
        BattleLobby,
        WorldMap,
        BattleMap,
        StageMap,
        // Canvas분기 처리를 위한 추가
        NetworkBattleMap,
    }

    public enum MapType
    {
        None,
        Tuto,
        stage1_4,
        stage2_5,
        stage3_3,
        stageWizard,
        stageMine,
    }

    public class EnumTypes : MonoBehaviour { }
}