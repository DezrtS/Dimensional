using Systems.Dialogue;
using UnityEngine;

namespace Scriptables.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueLineDatum", menuName = "Scriptable Objects/Dialogue/DialogueLineDatum")]
    public class DialogueLineDatum : ScriptableObject
    {
        [SerializeField] private DialogueLine dialogueLine;
        
        public DialogueLine DialogueLine => dialogueLine;
    }
}
