using UnityEngine;


[CreateAssetMenu(fileName = "Text", menuName = "Start Text")] 
public class StartText : ScriptableObject
{
    public string[] Texts;
    public float Delay = 0.4f;
}
