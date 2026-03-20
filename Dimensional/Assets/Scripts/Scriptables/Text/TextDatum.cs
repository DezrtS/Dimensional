using UnityEngine;

namespace Scriptables.Text
{
    [CreateAssetMenu(fileName = "TextDatum", menuName = "Scriptable Objects/TextDatum")]
    public class TextDatum : ScriptableObject
    {
        [SerializeField] private string text;
        public string Text => text;
    }
}