using SlotGame.Core;
using UnityEditor;

[CustomEditor(typeof(StateMachine),true)]
public class StateMachineEditor : Editor
{
    private void OnEnable() {
        (target as StateMachine).tag = "StateMachine";
    }
}
