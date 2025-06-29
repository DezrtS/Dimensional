using Managers;
using TMPro;

namespace Systems.Dialogue
{
    public abstract class DialogueTag
    {
        public enum Type
        {
            None,
            Typewriter,
            ScreenShake,
            Audio,
            Size,
            Wavy,
            Shakey,
            Distort,
        }
    
        public abstract Type TagType { get; }
        public int Index { get; private set; }
        public int EndIndex { get; set; }

        protected DialogueTag(int index, int endIndex)
        {
            Index = index;
            EndIndex = endIndex;
        }

        protected bool IsActive(int index)
        {
            return (index >= Index && index <= EndIndex);
        }

        public virtual void OnCharacterRevealed(char character, int index)
        {
            
        }
    }

    public class TypewriterTag : DialogueTag
    {
        public override Type TagType => Type.Typewriter;
        public float Speed { get; private set; }
        
        public TypewriterTag(int index, int endIndex, float speed) : base( index, endIndex)
        {
            Speed = speed;
        }

        public override void OnCharacterRevealed(char character, int index)
        {
            if (IsActive(index))
            {
                DialogueManager.Instance.ChangeTypewriterSpeed(Speed);
            }
        }
    }
    
    public class ScreenShakeTag : DialogueTag
    {
        public override Type TagType => Type.ScreenShake;
        public float Duration { get; set; }
        public float Strength { get; private set; }
        
        public ScreenShakeTag(int index, int endIndex, float duration, float strength) : base(index, endIndex)
        {
            Duration = duration;
            Strength = strength;
        }
    }
    
    public class AudioTag : DialogueTag
    {
        public override Type TagType => Type.Audio;
        public string EventId { get; private set; }
        
        public AudioTag(int index, int endIndex, string eventId) : base(index, endIndex)
        {
            EventId = eventId;
        }
    }
    
    public class SizeTag : DialogueTag
    {
        public override Type TagType => Type.Size;
        public int AdjustedIndex { get; set; }
        public int AdjustedEndIndex { get; set; }
        public float Size { get; private set; }
        
        public SizeTag(int index, int endIndex, int adjustedIndex, int adjustedEndIndex, float size) : base( index, endIndex)
        {
            AdjustedIndex = adjustedIndex;
            AdjustedEndIndex = adjustedEndIndex;
            Size = size;
        }
    }
    
    public class WavyTag : DialogueTag
    {
        public override Type TagType => Type.Wavy;
        public int AdjustedIndex { get; set; }
        public int AdjustedEndIndex { get; set; }
        public float Amplitude { get; private set; }
        public float Frequency { get; private set; }
        public float Phase { get; private set; }
        
        public WavyTag(int index, int endIndex, int adjustedIndex, int adjustedEndIndex, float amplitude, float frequency, float phase) : base( index, endIndex)
        {
            AdjustedIndex = adjustedIndex;
            AdjustedEndIndex = adjustedEndIndex;
            Amplitude = amplitude;
            Frequency = frequency;
            Phase = phase;
        }
    }
    
    public class ShakeyTag : DialogueTag
    {
        public override Type TagType => Type.Shakey;
        public int AdjustedIndex { get; set; }
        public int AdjustedEndIndex { get;  set; }
        public float Strength { get; private set; }
        public float Frequency { get; private set; }
        
        public ShakeyTag(int index, int endIndex, int adjustedIndex, int adjustedEndIndex, float strength, float frequency) : base( index, endIndex)
        {
            AdjustedIndex = adjustedIndex;
            AdjustedEndIndex = adjustedEndIndex;
            Strength = strength;
            Frequency = frequency;
        }
    }
    
    public class DistortTag : DialogueTag
    {
        public override Type TagType => Type.Distort;
        public int AdjustedIndex { get; set; }
        public int AdjustedEndIndex { get; set; }
        public float Strength { get; private set; }
        public float Frequency { get; private set; }
        
        public DistortTag(int index, int endIndex, int adjustedIndex, int adjustedEndIndex, float strength, float frequency) : base( index, endIndex)
        {
            AdjustedIndex = adjustedIndex;
            AdjustedEndIndex = adjustedEndIndex;
            Strength = strength;
            Frequency = frequency;
        }
    }
}