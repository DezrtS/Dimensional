using System;
using System.Collections.Generic;
using Systems.Shaders;
using UnityEngine;

namespace Systems.Grass
{
    public struct TriangleData
    {
        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector3 normal;
        public float area;
    }

    [Serializable]
    public struct GrassSettings
    {
        public float MaxGrassDistance;
        [Range(-1, 1)] public float MinChunkDot;
        [Range(-1, 1)] public float MaxChunkDot;
        
        public int BladesPerSquareUnit;
        public int BladeSegments;
        public float BladeHeight;
        public float BladeWidth;
        public float WidthTaper;
        public float CurveFactor;

        public LayerMask MaskLayer;
    }
    
    public class GrassSystem : MonoBehaviour
    {
        [Header("Chunking")]
        public Vector3 chunkSize = new Vector3(20, 20, 20);

        [Header("Grass Settings")]
        public ComputeShader grassCompute;
        public Material grassMaterial;
        public GrassSettings grassSettings;

        private Dictionary<Vector3Int, GrassChunk> _chunkMap;
        
        private readonly List<GrassMesh> _grassMeshes = new List<GrassMesh>();

        public void AddGrassMesh(GrassMesh grassMesh)
        {
            _grassMeshes.Add(grassMesh);
        }

        public void GenerateGrass()
        {
            BuildChunks();
        }

        private void BuildChunks()
        {
            _chunkMap = new Dictionary<Vector3Int, GrassChunk>();
            foreach (var grassMesh in _grassMeshes)
            {
                var mesh = grassMesh.MeshFilter.sharedMesh;
                if (!mesh.isReadable)
                {
                    Debug.LogError($"Mesh {mesh.name} is not readable.");
                    continue;
                }
                
                var meshChunkMap = new Dictionary<Vector3Int, List<TriangleData>>();

                var vertices = mesh.vertices;
                var triangles = mesh.triangles;
                var uvs = mesh.uv;

                var meshTransform = grassMesh.transform;

                for (var i = 0; i < triangles.Length; i += 3)
                {
                    var i0 = triangles[i + 0];
                    var i1 = triangles[i + 1];
                    var i2 = triangles[i + 2];
                    
                    var v0 = meshTransform.TransformPoint(vertices[i0]);
                    var v1 = meshTransform.TransformPoint(vertices[i1]);
                    var v2 = meshTransform.TransformPoint(vertices[i2]);
                    
                    var uv0 = uvs[i0];
                    var uv1 = uvs[i1];
                    var uv2 = uvs[i2];

                    var center = (v0 + v1 + v2) / 3f;
                    var chunkID = WorldToChunk(center);
                    
                    if (!meshChunkMap.TryGetValue(chunkID, out var list))
                    {
                        list = new List<TriangleData>();
                        meshChunkMap.Add(chunkID, list);
                    }

                    var cross = Vector3.Cross(v1 - v0, v2 - v0);
                    list.Add(new TriangleData
                    {
                        v0 = v0,
                        v1 = v1,
                        v2 = v2,
                        uv0 = uv0,
                        uv1 = uv1,
                        uv2 = uv2,
                        normal = cross.normalized,
                        area = cross.magnitude * 0.5f
                    });
                }

                foreach (var kvp in meshChunkMap)
                {
                    if (!_chunkMap.TryGetValue(kvp.Key, out var grassChunk))
                    {
                        var center = ChunkToWorld(kvp.Key);
                        var bounds = new Bounds(center, chunkSize);
                        var chunkGameObject = new GameObject($"GrassChunk_{kvp.Key}")
                        {
                            transform =
                            {
                                parent = transform
                            }
                        };
                        grassChunk = chunkGameObject.AddComponent<GrassChunk>();
                        grassChunk.Initialize(bounds, grassSettings);
                        _chunkMap.Add(kvp.Key, grassChunk);
                    }
                    
                    grassChunk.AddGrassInstance(grassMesh.CreateGrassInstance(kvp.Value, _chunkMap[kvp.Key], grassCompute, grassMaterial, grassSettings));
                }
            }
        }

        private Vector3Int WorldToChunk(Vector3 p)
        {
            return new Vector3Int(
                Mathf.FloorToInt(p.x / chunkSize.x),
                Mathf.FloorToInt(p.y / chunkSize.y),
                Mathf.FloorToInt(p.z / chunkSize.z)
            );
        }

        private Vector3 ChunkToWorld(Vector3Int id)
        {
            return new Vector3(
                (id.x + 0.5f) * chunkSize.x,
                (id.y + 0.5f) * chunkSize.y,
                (id.z + 0.5f) * chunkSize.z
            );
        }
    }
}
