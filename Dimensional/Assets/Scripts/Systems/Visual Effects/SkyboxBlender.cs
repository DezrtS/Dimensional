using System.Collections;
using Scriptables.Save;
using Systems.Shaders;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class SkyboxBlender : MonoBehaviour
    {
        [Header("Save Data")]
        [SerializeField] private BoolVariable isStormySaveData;
        
        [Header("Skybox Materials (Presets)")]
        public Material skyA;
        public Material skyB;

        [Header("Fog Settings")]
        public Color fogColorA;
        public Color fogColorB;
        public float fogDensityA = 0.01f;
        public float fogDensityB = 0.01f;
        
        [Header("Cloud Materials (Presets)")]
        public Material cloudA;
        public Material cloudB;

        public Clouds clouds;
        
        [Header("Light Settings")]
        public Light sunLight;
        public float lightIntensityA;
        public float lightIntensityB;

        [Header("Blend Settings")]
        public float duration = 5f;
        public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Material _runtimeSkybox;
        private Material _runtimeClouds;

        private void Start()
        {
            _runtimeSkybox = Instantiate(skyA);
            _runtimeClouds = clouds.RuntimeClouds;
            RenderSettings.skybox = _runtimeSkybox;

            if (!isStormySaveData.Value) return;
            BlendSkybox(1f);
            BlendFog(1f);
            BlendClouds(1f);
        }

        public void StartBlend()
        {
            StopAllCoroutines();
            StartCoroutine(Blend());
        }

        private IEnumerator Blend()
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float evaluatedT = blendCurve.Evaluate(Mathf.Clamp01(t));

                BlendSkybox(evaluatedT);
                BlendFog(evaluatedT);
                BlendClouds(evaluatedT);

                yield return null;
            }
            
            BlendSkybox(1f);
            BlendFog(1f);
            BlendClouds(1f);
        }

        private void BlendSkybox(float t)
        {
            // Gradient
            LerpColor(_runtimeSkybox, skyA, skyB,"_TopColor", t);
            LerpColor(_runtimeSkybox, skyA, skyB,"_BottomColor", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_GradientOffset", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_Exponent", t);

            // Sun
            LerpColor(_runtimeSkybox, skyA, skyB,"_SunColor", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_SunBrightness", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_SunSize", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_SunSoftness", t);

            // Mountains
            LerpColor(_runtimeSkybox, skyA, skyB,"_MountainTint", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_MountainHeight", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_MountainBlend", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_MountainMaskStrength", t);

            // Clouds
            LerpColor(_runtimeSkybox, skyA, skyB,"_CloudTint", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_CloudHeight", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_CloudBlend", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_CloudStrength", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_CloudSpeed", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_CloudSunIntensity", t);
            LerpFloat(_runtimeSkybox, skyA, skyB,"_CloudSunSharpness", t);
            
            // Light
            sunLight.intensity = Mathf.Lerp(lightIntensityA, lightIntensityB, t);
        }

        private void BlendFog(float t)
        {
            RenderSettings.fogColor = Color.Lerp(fogColorA, fogColorB, t);
            RenderSettings.fogDensity = Mathf.Lerp(fogDensityA, fogDensityB, t);
        }

        private void BlendClouds(float t)
        {
            LerpColor(_runtimeClouds, cloudA, cloudB,"_CloudColor", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_NoiseScale", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_CloudCutoff", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_CloudSoftness", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_CloudSpeed", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_TaperPower", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_ShadowOffset", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_SSSStrength", t);
            LerpFloat(_runtimeClouds, cloudA, cloudB,"_SSSPower", t);
        }

        private void LerpFloat(Material runtimeMaterial, Material materialA, Material materialB, string property, float t)
        {
            if (!materialA.HasProperty(property) || !materialB.HasProperty(property))
                return;

            float a = materialA.GetFloat(property);
            float b = materialB.GetFloat(property);
            runtimeMaterial.SetFloat(property, Mathf.Lerp(a, b, t));
        }

        private void LerpColor(Material runtimeMaterial, Material materialA, Material materialB, string property, float t)
        {
            if (!materialA.HasProperty(property) || !materialB.HasProperty(property))
                return;

            Color a = materialA.GetColor(property);
            Color b = materialB.GetColor(property);
            runtimeMaterial.SetColor(property, Color.Lerp(a, b, t));
        }
    }
}
