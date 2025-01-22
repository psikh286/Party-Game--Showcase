using Party.Interactions.Interfaces;

namespace Party.Minigames.Bombs
{
    /// <summary>
    /// Bomb that kills entity.
    /// Explodes after certain amount of time.
    /// </summary>
    public class KillerTimeBasedBomb : TimeBasedBomb<IKillable>
    {
        protected override void OnExplosion(IKillable target) => target.OnDeath();
    }
}