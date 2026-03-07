using System.Collections.Generic;
using UnityEngine;

namespace Systems.Grass
{
    public class GrassInstance
    {
        private static readonly int TrianglesProperty = Shader.PropertyToID("_Triangles");
        private static readonly int GrassBladesProperty  = Shader.PropertyToID("_GrassBlades");
        private static readonly int TriangleCountProperty = Shader.PropertyToID("_TriangleCount");
        private static readonly int BladesPerSquareUnitProperty = Shader.PropertyToID("_BladesPerSquareUnit");

        private const int MaxBlades = 45000;
        
        private readonly Material _grassMaterial;
        
        private readonly ComputeShader _grassCompute;
        
        private readonly ComputeBuffer _triangleBuffer;
        private readonly ComputeBuffer _grassBladeBuffer;
        private readonly ComputeBuffer _argsBuffer;
        
        private readonly GrassChunk _grassChunk;
        private readonly GrassSettings _grassSettings;

        public bool IsVisible { get; set; }
        public MaterialPropertyBlock MaterialPropertyBlock { get; }

        public GrassInstance(List<TriangleData> triangleData, GrassChunk grassChunk, ComputeShader grassCompute, Material grassMaterial, GrassSettings grassSettings)
        {
            _triangleBuffer = new ComputeBuffer(triangleData.Count, sizeof(float) * 19);
            _triangleBuffer.SetData(triangleData);
            _grassChunk = grassChunk;
            _grassCompute = grassCompute;
            _grassMaterial = grassMaterial;
            _grassSettings = grassSettings;
            
            _grassBladeBuffer = new ComputeBuffer(MaxBlades, sizeof(float) * 19, ComputeBufferType.Append);
            _grassBladeBuffer.SetCounterValue(0);
            
            _argsBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
            
            Dispatch();
            
            MaterialPropertyBlock ??= new MaterialPropertyBlock();
            MaterialPropertyBlock.SetBuffer(GrassBladesProperty, _grassBladeBuffer);
        }
        
        private void Dispatch()
        {
            var kernel = _grassCompute.FindKernel("CSMain");

            _grassCompute.SetBuffer(kernel, TrianglesProperty, _triangleBuffer);
            _grassCompute.SetBuffer(kernel, GrassBladesProperty, _grassBladeBuffer);
            
            _grassCompute.SetInt(TriangleCountProperty, _triangleBuffer.count);
            _grassCompute.SetInt(BladesPerSquareUnitProperty, _grassSettings.BladesPerSquareUnit);
            
            _grassChunk.AssignMaskBuffer(kernel, _grassCompute);
            
            var groups = Mathf.CeilToInt(_triangleBuffer.count / 64f);
            _grassCompute.Dispatch(kernel, groups, 1, 1);

            var bladeSegments = _grassMaterial.GetInt("_BladeSegments");

            var trisPerBlade = (uint)(2 * bladeSegments - 1);
            var vertsPerBlade = trisPerBlade * 3;

            uint[] args = { MaxBlades * trisPerBlade, 1, 0, 0};
            _argsBuffer.SetData(args);
        }

        public void Render()
        {
            if (!IsVisible) return;

            Graphics.DrawProceduralIndirect(
                _grassMaterial,
                _grassChunk.ChunkBounds,
                MeshTopology.Triangles,
                _argsBuffer,
                0,
                null,
                MaterialPropertyBlock
            );
        }

        public void ReleaseBuffers()
        {
            _triangleBuffer?.Release();
            _grassBladeBuffer?.Release();
            _argsBuffer?.Release();
        }
    }
}
