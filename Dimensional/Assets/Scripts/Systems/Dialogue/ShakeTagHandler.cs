using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ShakeTagHandler : MonoBehaviour
{
    private TMP_Text _textComponent;
    private bool _isInitialized;
    private readonly List<int> _shakeStartIndices = new List<int>();
    private readonly List<int> _shakeEndIndices = new List<int>();
    
    public float intensity = 1.5f;
    public float speed = 10f;

    void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        _isInitialized = true;
    }

    void OnEnable()
    {
        if (!_isInitialized) return;
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    public void HandleTag(
        bool isStartTag, 
        string tag, 
        int[] props, 
        int startIndex, 
        int length,
        TMP_Text textComponent)
    {
        if (tag != "shake") return;
        
        if (isStartTag)
        {
            // Store start index of shake effect
            _shakeStartIndices.Add(startIndex);
        }
        else
        {
            // Store end index of shake effect
            _shakeEndIndices.Add(startIndex + length - 1);
        }
    }

    void OnTextChanged(Object obj)
    {
        if (obj != _textComponent) return;
        
        // Clear previous indices
        _shakeStartIndices.Clear();
        _shakeEndIndices.Clear();
        
        // Parse text to find shake tags
        //TMP_TextTagManager.ParseText(_textComponent, this);
        _textComponent.ForceMeshUpdate();
    }

    public void ProcessVertexData(TMP_TextInfo textInfo)
    {
        if (_shakeStartIndices.Count == 0) return;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            // Check if character is within shake range
            bool shouldShake = false;
            for (int j = 0; j < _shakeStartIndices.Count; j++)
            {
                if (i >= _shakeStartIndices[j] && i <= _shakeEndIndices[j])
                {
                    shouldShake = true;
                    break;
                }
            }

            if (!shouldShake) continue;

            // Apply shake effect
            Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vertexIndex = charInfo.vertexIndex;
            
            Vector3 shakeOffset = new Vector3(
                Mathf.Sin(Time.time * speed + i) * intensity,
                Mathf.Cos(Time.time * speed + i) * intensity,
                0
            );

            for (int k = 0; k < 4; k++)
            {
                verts[vertexIndex + k] += shakeOffset;
            }
        }
    }
}