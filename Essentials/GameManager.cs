using Party.Essentials.Singletons;
using UnityEngine;

namespace Party.Essentials
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [field: SerializeField] public ReferenceProvider ReferenceProvider { get; private set; }
    }
}
