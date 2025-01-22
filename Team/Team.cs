using System.Collections.Generic;
using UnityEngine;

namespace Party.Team
{
    public class Team
    {
        public struct Member
        {
            public Member(string name, GameObject skin)
            {
                Name = name;
                Skin = skin;
            }
            public string Name;
            public GameObject Skin;
        }
        
        public readonly List<Member> Members = new();
    }
}
