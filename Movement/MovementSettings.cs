using UnityEngine;

namespace Party.Movement
{
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "Party/Settings/MovementSettings")]
    public class MovementSettings : ScriptableObject
    {
        [field: Header("Suspension")] 
        [field: SerializeField] public float RestPosition { get; private set; }
        [field: SerializeField] public float SpringStrength { get; private set; }
        [field: SerializeField] public float SpringDamper { get; private set; }
        
        [field: Header("Movement")]
        [field: SerializeField] public float MaxSpeed { get; private set; }
        [field: SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public AnimationCurve AccelerationCurve { get; private set; }
        [field: SerializeField] public float GroundDeceleration { get; private set; }
        [field: SerializeField] public float AirDeceleration  { get; private set; }
        
        [field: Header("Jump")]
        [field: SerializeField] public float JumpPower { get; private set; }
        [field: SerializeField] public float JumpBuffer { get; private set; }
        [field: SerializeField] public float CoyoteTime { get; private set; }
        
        [field: Header("Gravity")]
        [field: SerializeField] public float GroundingForce { get; private set; }
        [field: SerializeField] public float RiseAcceleration { get; private set; }
        [field: SerializeField] public float FallAcceleration { get; private set; }
        [field: SerializeField] public float JumpEndEarlyGravityModifier { get; private set; }
        [field: SerializeField] public float MaxFallSpeed { get; private set; }

        [field: Header("Punch")] 
        [field: SerializeField] public float ObjectPunchForce { get; private set; }
        [field: SerializeField] public float PlayerPunchForce { get; private set; }
    }
}