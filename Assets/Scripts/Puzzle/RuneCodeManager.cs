using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Rendering;

namespace Puzzle
{
    public class RuneCodeManager: MonoBehaviour
    {
        private int compteur;
        private int currentValue = -1;
        private Door _exitDoor;
        private List<RuneSlabs> _activatedSlabs;

        private void Start()
        {
            
            _activatedSlabs = new List<RuneSlabs>();
        }

        public void RegisterInput(int value, RuneSlabs slab)
        {
            _activatedSlabs.Add(slab);
            
            // Correct entry
            if (currentValue < value)
            {
                currentValue = value;
                compteur++;
                
                if (compteur == 4)
                    OpenExitDoor();
            }
            // Wrong entry
            else
            {
                compteur = 0;
                currentValue = -1;
                
                GameManager.Instance.StopLoop();

                foreach (var s in _activatedSlabs)
                {
                    s.EnableSlab();
                }
                _activatedSlabs.Clear();
            }
        }

        private void OpenExitDoor()
        {
            _exitDoor.ManageDoor(true);
        }

        public void SetExitDoor(Door door)
        {
            _exitDoor = door;
        }
    }
}