using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Systems.Dialogue
{
    public class DialogueEffectHandler
    {
        public struct ProcessData
        {
            public string FormattedText;
            public string BareText;
        }

        private struct DialogueTagEvaluationData
        {
            public bool IsEndTag;
            public DialogueTag.Type Type;
            public string Data;
        }

        private struct OpenDialogueTagData
        {
            public DialogueTagEvaluationData DialogueTagEvaluationData;
            public int Index;
            public int AdjustedIndex;
        }
        
        private List<DialogueTag> _closedDialogueTags = new List<DialogueTag>();
        private List<OpenDialogueTagData> _openDialogueTags = new List<OpenDialogueTagData>();
        
        public ProcessData ProcessTags(string text)
        {
            _closedDialogueTags.Clear();
            _openDialogueTags.Clear();

            var formattedResult = new StringBuilder();
            var result = new StringBuilder();
            var tag = new StringBuilder();
            
            var skipIndexCount = 0;
            var formattedSkipIndexCount = 0;
            
            var inTag = false;
            
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (inTag)
                {
                    skipIndexCount++;
                    if (c == '>')
                    {
                        inTag = false;
                        var tagEvaluation = EvaluateDialogueTag(tag.ToString());
                        if (tagEvaluation.Type == DialogueTag.Type.None)
                        {
                            formattedSkipIndexCount += tag.Length + 2;
                            formattedResult.Append($"<{tag}>");
                        }
                        ProcessTag(tagEvaluation, i - skipIndexCount, i - formattedSkipIndexCount);
                        tag.Clear();
                        continue;
                    }
                    
                    tag.Append(c);
                }
                else
                {
                    if (c == '<')
                    {
                        inTag = true;
                        skipIndexCount++;
                    }
                }

                if (inTag) continue;
                formattedResult.Append(c);
                result.Append(c);
            }
            
            if (inTag) Debug.LogWarning("Not all tags have been closed");
            CloseAllTags();

            foreach (var dialogueTag in _closedDialogueTags)
            {
                Debug.Log($"{dialogueTag.TagType} --> {dialogueTag.Index} - {dialogueTag.EndIndex}");
            }
            return new ProcessData() { FormattedText = formattedResult.ToString(), BareText  = result.ToString() };
        }

        private static DialogueTagEvaluationData EvaluateDialogueTag(string dialogueTag)
        {
            var isEndTag = dialogueTag.StartsWith("/");
            if (isEndTag) dialogueTag = dialogueTag[1..];
            
            var tagPlusData = dialogueTag.Split('=');
            var data = tagPlusData.Length == 2 ? tagPlusData[1] : string.Empty;
            
            var type = tagPlusData[0] switch
            {
                "typeSpeed" => DialogueTag.Type.Typewriter,
                "screenShake" => DialogueTag.Type.ScreenShake,
                "audio" => DialogueTag.Type.Audio,
                "size" => DialogueTag.Type.Size,
                "wavy" => DialogueTag.Type.Wavy,
                "shake" => DialogueTag.Type.Shakey,
                "distort" => DialogueTag.Type.Distort,
                _ => DialogueTag.Type.None,
            };
            
            return new DialogueTagEvaluationData() { IsEndTag = isEndTag, Type = type, Data = data };
        }

        
        private void ProcessTag(DialogueTagEvaluationData dialogueTagEvaluationData, int index, int adjustedIndex)
        {
            try
            {
                if (dialogueTagEvaluationData.Type == DialogueTag.Type.None) return;
                
                if (dialogueTagEvaluationData.IsEndTag)  CloseTag(dialogueTagEvaluationData.Type, index, adjustedIndex);
                else OpenTag(dialogueTagEvaluationData, index, adjustedIndex);
            }
            catch
            {
                Debug.LogError($"Error processing tag: {dialogueTagEvaluationData.Type}");
            }
        }
        
        private void OpenTag(DialogueTagEvaluationData dialogueTagEvaluationData, int index, int adjustedIndex)
        {
            _openDialogueTags.Add(new OpenDialogueTagData() { DialogueTagEvaluationData = dialogueTagEvaluationData, Index = index, AdjustedIndex = adjustedIndex });
        }

        private void CloseAllTags()
        {
            for (var i = _openDialogueTags.Count - 1; i >= 0; i--)
            {
                var index = _openDialogueTags[i].Index;
                var adjustedIndex = _openDialogueTags[i].Index;
                CloseTag(_openDialogueTags[i].DialogueTagEvaluationData.Type, index, adjustedIndex);
            }
        }

        private void CloseTag(DialogueTag.Type tagType, int endIndex, int adjustedEndIndex)
        {
            for (var i = _openDialogueTags.Count - 1; i >= 0; i--)
            {
                var openDialogueTagData = _openDialogueTags[i];
                if (openDialogueTagData.DialogueTagEvaluationData.Type != tagType) continue;
                _openDialogueTags.RemoveAt(i);

                var index = openDialogueTagData.Index;
                var adjustedIndex = openDialogueTagData.AdjustedIndex;
                var data = openDialogueTagData.DialogueTagEvaluationData.Data.Split(',');
                DialogueTag dialogueTag = tagType switch
                {
                    DialogueTag.Type.None => null,
                    DialogueTag.Type.Typewriter => new TypewriterTag(index, endIndex, float.Parse(data[0])),
                    DialogueTag.Type.ScreenShake => new ScreenShakeTag(index, endIndex, float.Parse(data[0]), float.Parse(data[1])),
                    DialogueTag.Type.Audio => new AudioTag(index, endIndex, data[0]),
                    DialogueTag.Type.Size => new SizeTag(index, endIndex, adjustedIndex, adjustedEndIndex,float.Parse(data[0])),
                    DialogueTag.Type.Wavy => new WavyTag(index, endIndex, adjustedIndex, adjustedEndIndex, float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2])),
                    DialogueTag.Type.Shakey => new ShakeyTag(index, endIndex, adjustedIndex, adjustedEndIndex, float.Parse(data[0]), float.Parse(data[1])),
                    DialogueTag.Type.Distort => new DistortTag(index, endIndex, adjustedIndex, adjustedEndIndex, float.Parse(data[0]), float.Parse(data[1])),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                _closedDialogueTags.Add(dialogueTag);
                return;
            }
            
            Debug.LogWarning($"Failed to find opening tag of type: {tagType}");
        }

        public void OnCharacterRevealed(char character, int index)
        {
            foreach (var closedDialogueTag in _closedDialogueTags)
            {
                closedDialogueTag.OnCharacterRevealed(character, index);
            }
        }
    }
}
