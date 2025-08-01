using UnityEditor;
using UnityEngine;

namespace Puzzle.PuzzleEditor
{
    [CustomEditor(typeof(PuzzleGenerator))]
    public class PuzzleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PuzzleGenerator pzgenerator = (PuzzleGenerator)target;

            if (GUILayout.Button("Generate"))
            {
                pzgenerator.GeneratePuzzle();//Manque les parametres mais on devrait pas en passer?
            }
        }
    }
}
