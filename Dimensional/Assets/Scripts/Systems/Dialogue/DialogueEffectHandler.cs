using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Systems.Dialogue
{
    public class DialogueEffectHandler
    {
        private enum TagType
        {
            None,
            Typewriter,
            ScreenShake,
            Audio,
            Size,
            Wavy,
            Shakey,
            Distortion,
        }
        
        private List<Tag> _tags = new List<Tag>();
        private readonly Dictionary<TagType, List<Tag>> _inProgressTags = new Dictionary<TagType, List<Tag>>();
        
        public string ProcessTags(string text)
        {
            _tags.Clear();
            _inProgressTags.Clear();

            var result = new StringBuilder();
            var tag = new StringBuilder();
            var skipIndexes = 0;
            var inTag = false;
            
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (inTag)
                {
                    skipIndexes++;
                    if (c == '>')
                    {
                        inTag = false;
                        ProcessTag(tag.ToString(), i - skipIndexes);
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
                        skipIndexes++;
                    }
                }
                
                if (!inTag) result.Append(c);
            }
            
            if (inTag) Debug.LogWarning("Not all tags have been closed");
            return result.ToString();
        }

        private void ProcessTag(string tagText, int index)
        {
            try
            {
                var tagPlusData = tagText.Split('=');
                var endTag = tagPlusData[0].StartsWith('/');
                var tagType = tagPlusData[0] switch
                {
                    "typeSpeed" => TagType.Typewriter,
                    "screenShake" => TagType.ScreenShake,
                    "audio" => TagType.Audio,
                    "size" => TagType.Size,
                    "wavy" => TagType.Wavy,
                    "shake" => TagType.Shakey,
                    "distort" => TagType.Distortion,
                    _ => TagType.None,
                };

                if (tagType == TagType.None) return;
                var data = tagPlusData[1].Split(',');
                
                if (endTag) CloseTag(tagType, index);
                else
                {
                    var parameters = tagType switch
                    {
                        TagType.Typewriter => new Dictionary<string, object>()
                        {
                            {"SkipOpenTag", false},
                            {"Speed", float.Parse(data[0])},
                        },
                        TagType.ScreenShake => new Dictionary<string, object>()
                        {
                            {"SkipOpenTag", true},
                            {"Strength", float.Parse(data[0])},
                        },
                        TagType.Audio => new Dictionary<string, object>()
                        {
                            {"SkipOpenTag", true},
                            {"AudioId", data[0]},
                        },
                        TagType.Size => new Dictionary<string, object>()
                        {
                            {"SkipOpenTag", false},
                            {"Size", float.Parse(data[0])},
                        },
                        TagType.Wavy => new Dictionary<string, object>()
                        {
                            {"SkipOpenTag", false},
                            {"Amplitude", float.Parse(data[0])},
                            {"Strength", float.Parse(data[1])},
                        },
                        TagType.Shakey => new Dictionary<string, object>()
                        {
                            {"SkipOpenTag", false},
                            {"Strength", float.Parse(data[1])},
                        },
                        TagType.Distortion => new Dictionary<string, object>()
                        {
                            {"SkipOpenTag", false},
                            {"Strength", float.Parse(data[1])},
                        },
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    var tag = Tag.Construct(tagType, index, index, parameters);
                    if ((bool)parameters["SkipOpenTag"])
                    {
                        _tags.Add(tag);
                        return;
                    }
                    
                    OpenTag(tag);
                } 
            }
            catch
            {
                Debug.LogError($"Error processing tag: {tagText}");
            }
        }

        private void OpenTag(Tag tag)
        {
            if (_inProgressTags.TryAdd(tag.Type, new List<Tag>() { tag })) return;
            _inProgressTags[tag.Type].Add(tag);
        }

        private void CloseTag(TagType tagType, int index)
        {
            var tags = _inProgressTags[tagType];
            for (var i = tags.Count - 1; i >= 0; i--)
            {
                var tag = tags[i];
                if (tag.Type != tagType) continue;
                _inProgressTags[tag.Type].Remove(tag);
                tag.EndIndex = index;
                return;
            }
            
            Debug.LogWarning($"Failed to find opening tag of type: {tagType}");
        }

        private class Tag
        {
            public TagType Type;
            public int Index;
            public int EndIndex;
            
            public Dictionary<string, object> Parameters;

            public static Tag Construct(TagType type, int index, int endIndex, Dictionary<string, object> parameters)
            {
                return new Tag()
                {
                    Type = type,
                    Index = index,
                    EndIndex = endIndex,

                    Parameters = parameters,
                };
            }

            public bool IsActive(int index)
            {
                return index >= Index && index <= EndIndex;
            }
        }
    }
}
