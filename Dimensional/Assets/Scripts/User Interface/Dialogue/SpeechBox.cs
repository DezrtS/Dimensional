using Febucci.UI;
using Scriptables.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface.Dialogue
{
    public class SpeechBox : MonoBehaviour
    {
        [SerializeField] private DialogueLineDatum dialogueLineDatum;
        [Space]
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterByCharacter typewriterByCharacter;
        [Space] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;

        public void SetDialogueLine(DialogueLineDatum newDialogueLineDatum)
        {
            dialogueLineDatum = newDialogueLineDatum;
        }

        [ContextMenu("Show")]
        public void ShowText()
        {
            nameText.text = dialogueLineDatum.DialogueSpeakerDatum.SpeakerName;
            iconImage.sprite = dialogueLineDatum.DialogueSpeakerDatum.SpeakerIcon;
            textAnimator.SetText(dialogueLineDatum.Text, true);
            typewriterByCharacter.StartShowingText();
        }

        [ContextMenu("Hide")]
        public void HideText()
        {
            typewriterByCharacter.StartDisappearingText();
        }
    }
}
