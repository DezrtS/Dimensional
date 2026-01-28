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
        public Vector3 normal;
        public float area;
    }

    [Serializable]
    public struct GrassSettings
    {
        public float MaxGrassDistance;
        [Range(-1, 1)] public float MinChunkDot;
        
        public int BladesPerSquareUnit;
        public uint BladeSegments;
        public float BladeHeight;
    }
    
    public class GrassSystem : MonoBehaviour
    {
        [Header("Source Meshes")]
        public List<MeshFilter> sourceMeshes;

        [Header("Chunking")]
        public Vector3 chunkSize = new Vector3(20, 20, 20);

        [Header("Grass Settings")]
        public ComputeShader grassCompute;
        public Material grassMaterial;
        public GrassSettings grassSettings;

        private Dictionary<Vector3Int, List<TriangleData>> _chunkMap;

        private void Start()
        {
            GenerateGrass();
        }

        private void GenerateGrass()
        {
            BuildChunks();
            SpawnChunks();
        }

        public void UpdateChunks()
        {
            var chunks = GetComponentsInChildren<GrassChunk>();
            foreach (var chunk in chunks)
            {
                chunk.UpdateGrassSettings(grassSettings);
            }
        }

        private void BuildChunks()
        {
            _chunkMap = new Dictionary<Vector3Int, List<TriangleData>>();
            foreach (var mf in sourceMeshes)
            {
                if (!mf) continue;

                Mesh mesh = mf.sharedMesh;
                if (!mesh.isReadable)
                {
                    Debug.LogError($"Mesh {mesh.name} is not readable.");
                    continue;
                }

                var verts = mesh.vertices;
                var tris = mesh.triangles;

                Transform t = mf.transform;

                for (int i = 0; i < tris.Length; i += 3)
                {
                    Vector3 v0 = t.TransformPoint(verts[tris[i]]);
                    Vector3 v1 = t.TransformPoint(verts[tris[i + 1]]);
                    Vector3 v2 = t.TransformPoint(verts[tris[i + 2]]);

                    Vector3 center = (v0 + v1 + v2) / 3f;
                    Vector3Int chunkID = WorldToChunk(center);
                    
                    if (!_chunkMap.TryGetValue(chunkID, out var list))
                    {
                        list = new List<TriangleData>();
                        _chunkMap.Add(chunkID, list);
                    }

                    var cross = Vector3.Cross(v1 - v0, v2 - v0);
                    list.Add(new TriangleData
                    {
                        v0 = v0,
                        v1 = v1,
                        v2 = v2,
                        normal = cross.normalized,
                        area = cross.magnitude * 0.5f
                    });
                }
            }
        }

        private void SpawnChunks()
        {
            foreach (var kvp in _chunkMap)
            {
                Vector3 center = ChunkToWorld(kvp.Key);
                //center.y = 0;
                Bounds bounds = new Bounds(center, chunkSize);

                GameObject go = new GameObject($"GrassChunk_{kvp.Key}");
                go.transform.parent = transform;

                var chunk = go.AddComponent<GrassChunk>();
                chunk.Initialize(
                    kvp.Value,
                    bounds,
                    grassCompute,
                    grassMaterial,
                    grassSettings
                );
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
