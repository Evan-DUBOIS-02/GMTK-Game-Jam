using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class GhostReplayer: MonoBehaviour
    {
        [SerializeField] private float _playbackInterval = 0.02f;
        private List<PlayerState> _states;

        public void Init(List<PlayerState> states)
        {
            _states = states;
        }

        public void StopReplay()
        {
            StopAllCoroutines();
            transform.position = _states[0].Position;
        }

        public void StartReplay()
        {
            StartCoroutine(ReplayStates());
        }

        private IEnumerator ReplayStates()
        {
            foreach (PlayerState state in _states)
            {
                transform.position = state.Position;
                yield return new WaitForSeconds(_playbackInterval);
            }
        }
    }
}