using System;

namespace Party.Movement.Interfaces
{
    public interface IPlayer
    {
        public int Index { get; }
        public bool CanMove { get; set; }
        public event Action DeathEvent;
    }
}