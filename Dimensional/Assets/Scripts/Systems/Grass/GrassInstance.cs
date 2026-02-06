using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Systems.Grass
{
    public class GrassInstance
    {
        private static readonly int TrianglesProperty = Shader.PropertyToID("_Triangles");
        private static readonly int VerticesProperty  = Shader.PropertyToID("_Vertices");
        private static readonly int IndicesProperty   = Shader.PropertyToID("_Indices");
        private static readonly int VertexCounterProperty = Shader.PropertyToID("_VertexCounter");
        private static readonly int IndexCounterProperty = Shader.PropertyToID("_IndexCounter");
        private static readonly int TriangleCountProperty = Shader.PropertyToID("_TriangleCount");
        private static readonly int BladesPerSquareUnitProperty = Shader.PropertyToID("_BladesPerSquareUnit");
        private static readonly int BladeSegmentsProperty = Shader.PropertyToID("_BladeSegments");
        private static readonly int BladeHeightProperty = Shader.PropertyToID("_BladeHeight");
        private static readonly int BladeWidthProperty = Shader.PropertyToID("_BladeWidth");
        private static readonly int WidthTaperProperty = Shader.PropertyToID("_WidthTaper");
        private static readonly int CurveFactorProperty = Shader.PropertyToID("_CurveFactor");

        private const int MaxBlades = 50000;
        
        private readonly Material _grassMaterial;
        
        private readonly ComputeShader _grassCompute;
        
        private readonly ComputeBuffer _triangleBuffer;
        private ComputeBuffer _vertexBuffer;
        private ComputeBuffer _indexBuffer;
        private ComputeBuffer _vertexCounter;
        private ComputeBuffer _indexCounter;
        private ComputeBuffer _argsBuffer;
        
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
            
            SetupBuffers();
            Dispatch();
            
            MaterialPropertyBlock ??= new MaterialPropertyBlock();
            MaterialPropertyBlock.SetBuffer(VerticesProperty, _vertexBuffer);
            MaterialPropertyBlock.SetBuffer(IndicesProperty, _indexBuffer);
        }
        
        private void SetupBuffers()
        {
            _vertexBuffer?.Release();
            _indexBuffer?.Release();
            _vertexCounter?.Release();
            _indexCounter?.Release();
            _argsBuffer?.Release();

            _vertexBuffer = new ComputeBuffer(
                MaxBlades * _grassSettings.BladeSegments * 2 + MaxBlades,
                sizeof(float) * 9,
                ComputeBufferType.Structured
            );

            _indexBuffer = new ComputeBuffer(
                MaxBlades * _grassSettings.BladeSegments * 6,
                sizeof(uint),
                ComputeBufferType.Structured
            );
            
            _vertexCounter = new ComputeBuffer(
                1,
                sizeof(uint),
                ComputeBufferType.Structured
            );

            _indexCounter = new ComputeBuffer(
                1,
                sizeof(uint),
                ComputeBufferType.Structured
            );
            
            uint[] zero = { 0 }; 
            _vertexCounter.SetData(zero);
            _indexCounter.SetData(zero);
            
            _argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            _argsBuffer.SetData(new uint[] { 0, 1, 0, 0, 0 });
        }
        
        private void Dispatch()
        {
            var kernel = _grassCompute.FindKernel("CSMain");

            _grassCompute.SetBuffer(kernel, TrianglesProperty, _triangleBuffer);
            _grassCompute.SetBuffer(kernel, VerticesProperty, _vertexBuffer);
            _grassCompute.SetBuffer(kernel, IndicesProperty, _indexBuffer);
            _grassCompute.SetBuffer(kernel, VertexCounterProperty, _vertexCounter);
            _grassCompute.SetBuffer(kernel, IndexCounterProperty, _indexCounter);
            
            _grassCompute.SetInt(TriangleCountProperty, _triangleBuffer.count);
            _grassCompute.SetInt(BladesPerSquareUnitProperty, _grassSettings.BladesPerSquareUnit);
            _grassCompute.SetInt(BladeSegmentsProperty, _grassSettings.BladeSegments);
            _grassCompute.SetFloat(BladeHeightProperty, _grassSettings.BladeHeight);
            _grassCompute.SetFloat(BladeWidthProperty, _grassSettings.BladeWidth);
            _grassCompute.SetFloat(WidthTaperProperty, _grassSettings.WidthTaper);
            _grassCompute.SetFloat(CurveFactorProperty, _grassSettings.CurveFactor);
            
            _grassChunk.AssignMaskBuffer(kernel, _grassCompute);
            
            var groups = Mathf.CeilToInt(_triangleBuffer.count / 64f);
            _grassCompute.Dispatch(kernel, groups, 1, 1);

            ComputeBuffer.CopyCount(_indexBuffer, _argsBuffer, 0);

            uint[] indexCountData = { 0 };
            _indexCounter.GetData(indexCountData);
            var indexCount = indexCountData[0];

            uint[] args = { indexCount, 1, 0, 0, 0 };
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
                MaterialPropertyBlock,
                ShadowCastingMode.Off
            );
        }

        public void ReleaseBuffers()
        {
            _triangleBuffer?.Release();
            _vertexBuffer?.Release();
            _indexBuffer?.Release();
            _vertexCounter?.Release();
            _indexCounter?.Release();
            _argsBuffer?.Release();
        }
    }
}
