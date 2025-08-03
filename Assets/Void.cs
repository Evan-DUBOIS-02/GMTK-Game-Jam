using Player;
using UnityEngine;

namespace Puzzle
{

    public class Void : MonoBehaviour
    {
        [SerializeField] private GameObject _verticalVoid;
        [SerializeField] private GameObject _horizontalVoid;


        private bool _isHorizontal = false;

        public void SetSideRenderer()
        {
            _isHorizontal = true;
            _verticalVoid.SetActive(false);
            _horizontalVoid.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                if(!collision.GetComponent<PlayerManager>()._isBeingBombed)
                {
                    //Activate death + reset boucle? Le faire dans PlayerManager mais comment acceder à void?
                }
            }
        }
    }
}