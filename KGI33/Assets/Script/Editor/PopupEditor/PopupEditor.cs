
using UnityEditor;

[CustomEditor(typeof(Popup),true)]
public class PopupEditor : Editor
{
    private void OnEnable() {
        (target as Popup).gameObject.tag = "Popup";
    }
}
