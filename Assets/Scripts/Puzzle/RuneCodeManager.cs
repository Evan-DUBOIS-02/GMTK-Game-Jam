using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Puzzle
{
    public class RuneCodeManager: MonoBehaviour
    {
        private int compteur;
        private int currentValue = -1;
        private Door _exitDoor;

        public bool RegisterInput(int value)
        {
            Debug.Log(value);
            // Correct entry
            if (currentValue < value)
            {
                currentValue = value;
                compteur++;
                
                if (compteur == 4)
                    OpenExitDoor();
                
                return true;
            }
            // Wrong entry
            else
            {
                compteur = 0;
                currentValue = -1;
                
                foreach (var slab in GetComponentsInChildren<RuneSlabs>())
                    slab.EnableSlab();
                
                Debug.Log("Failed !");
                return false;
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