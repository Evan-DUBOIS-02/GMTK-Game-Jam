using UnityEditor;
using UnityEngine;

namespace Puzzle.PuzzleEditor
{
    [CustomEditor(typeof(Lever))]
    public class LeverEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Lever generator = (Lever)target;

            if (GUILayout.Button("Open door"))
            {
                generator.OpenDoor();
            }
        }
    }
}