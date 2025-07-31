using UnityEditor;
using UnityEngine;

namespace Dungeon.DungeonEditor
{
    [CustomEditor(typeof(Generator))]
    public class GeneratorEditor: Editor
    {
        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Generator generator = (Generator)target;

            if (GUILayout.Button("Generate"))
            {
                generator.Generate();
            }
        }
    }
}