using UnityEngine;

namespace Party.Player.Visuals
{
    [CreateAssetMenu(fileName = "Skin List", menuName = "Party/Visuals/Skin List")]
    public class PlayerSkinListPreset : ScriptableObject
    {
        [field: SerializeField] public Animator[] PlayerSkins;
    }
}