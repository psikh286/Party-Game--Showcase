using System.Collections;
using TMPro;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private const string _go = "GO!";
    
    public IEnumerator Init(StartText texts)
    {
        var delay = new WaitForSeconds(texts.Delay);
        foreach (var text in texts.Texts)
        {
            _text.SetText(text);
            yield return delay;
        }
        
        _text.SetText(_go);
        yield return delay;
    }
}
