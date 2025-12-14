using Scriptables.Dialogue;
using UnityEngine;
using User_Interface;
using User_Interface.Dialogue;

namespace Scriptables.User_Interface
{
    [CreateAssetMenu(fileName = "SpeechBoxDatum", menuName = "Scriptable Objects/SpeechBoxDatum")]
    public class SpeechBoxDatum : WorldUIAnchorDatum
    {
        [SerializeField] private DialogueLineDatum dialogueLineDatum;

        public override WorldUIAnchor SpawnWorldUIAnchor(Transform parent, GameObject holderGameObject, Transform worldTransform)
        {
            var speechBoxObject = Instantiate(Prefab, parent.transform);
            var speechBox = speechBoxObject.GetComponent<SpeechBox>();
            speechBox.Initialize(this, holderGameObject, worldTransform);
            speechBox.SetDialogueLine(dialogueLineDatum);
            return speechBox;
        }
    }
}
