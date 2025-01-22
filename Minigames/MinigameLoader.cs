using System;
using System.Collections;
using UnityEngine;
using Party.Player;
using Party.Movement;
using TMPro;

namespace Party.Minigames
{
    public class MinigameLoader : MonoBehaviour
    {
        public static event Action MinigameStartedEvent; 
        
        [SerializeField] private PlayerMovement[] _players;
        [SerializeField] private GameObject _startScreenParent;
        [SerializeField] private TMP_Text _startScreenText;

        private void Awake()
        {
            Reposition();
            StartCoroutine(InitializeMinigame());
        }

        private void Reposition()
        {
            var spawns = new[] {_players[0].transform.position, _players[1].transform.position };
            
            _players[0].transform.position = spawns[(PlayersData.PlayerPosition[0] + 1) / 2];
            _players[1].transform.position = spawns[(PlayersData.PlayerPosition[1] + 1) / 2];
        }

        private IEnumerator InitializeMinigame()
        {
            _players[0].CanMove = false;
            _players[1].CanMove = false;
            
            _startScreenParent.SetActive(true);
            _startScreenText.text = "";
            
            yield return new WaitForSeconds(MinigamesData.MinigameNameDelay);

            var second = new WaitForSeconds(MinigamesData.InitialDelay * 0.25f);

            for (var i = 3; i > 0; i--)
            {
                _startScreenText.text = i.ToString();
                yield return second;
            }
            
            _players[0].CanMove = true;
            _players[1].CanMove = true;

            _startScreenText.text = "GO!";
            yield return second;
            
            _startScreenParent.SetActive(false);
            
            MinigameStartedEvent?.Invoke();
        }
    }
}