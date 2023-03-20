using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace RGN.UI
{

#if UNITY_EDITOR
    [CustomEditor(typeof(Touchable))]
    public class Touchable_Editor : Editor
    { public override void OnInspectorGUI(){} }
#endif
    [RequireComponent(typeof(CanvasRenderer))]
    public class Touchable : Graphic
    {
        protected override void Awake() { base.Awake(); }
        protected override void UpdateGeometry() { }
    }

}
