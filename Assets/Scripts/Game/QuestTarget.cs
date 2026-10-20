using UnityEngine;

namespace BinaryLift
{
    public enum QuestAction { Begin, Jump, Cone, Saddle, Scan, Restart }

    /// <summary>Interaction contract retained when placeholder art is replaced.</summary>
    public sealed class QuestTarget : MonoBehaviour
    {
        public QuestAction Action;
        public int Value;
        public string Caption;
        public Renderer Surface;
        public Color BaseColor;
        public bool Available = true;
        MaterialPropertyBlock properties;

        public void Highlight(bool selected)
        {
            if (Surface == null) return;
            if (properties == null) properties = new MaterialPropertyBlock();
            properties.SetColor("_Color", selected && Available ? new Color(1f, 0.72f, 0.36f) : BaseColor);
            Surface.SetPropertyBlock(properties);
        }
    }
}
