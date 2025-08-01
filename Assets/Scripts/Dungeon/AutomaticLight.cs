using System;
using System.Collections;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Dungeon
{
    public class AutomaticLight: MonoBehaviour
    {
        [SerializeField] private Light2D _light;
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() != null)
            {
                StartCoroutine(FadeIn());
            }
        }

        public void EnableLight()
        {
            _light.color = new Color(_light.color.r, _light.color.g, _light.color.b, 1f);
        }

        private IEnumerator FadeIn()
        {
            while (_light.color.a < 1)
            {
                _light.color = new Color(_light.color.r, _light.color.g, _light.color.b, _light.color.a + 0.2f);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}