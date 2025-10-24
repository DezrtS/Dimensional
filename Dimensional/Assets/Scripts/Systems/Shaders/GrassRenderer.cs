using UnityEngine;

namespace Systems.Shaders
{
    public class GrassRenderer : MonoBehaviour
    {
        public Mesh bladeMesh;
        public Material grassMaterial;
        public ComputeShader compute;
        public int instanceCount = 100000;

        ComputeBuffer basePosBuffer;
        ComputeBuffer windWeightBuffer;
        ComputeBuffer matrixBuffer;
        ComputeBuffer argsBuffer;

        const int THREADS = 256;

        void Start()
        {
            // 1) Create placement (example: random over a rectangle; in practice sample terrain)
            Vector3[] positions = new Vector3[instanceCount];
            float[] windWeights = new float[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                positions[i] = new Vector3(Random.Range(-50f, 50f), 0f, Random.Range(-50f, 50f));
                // sample terrain height here and set positions[i].y = height
                windWeights[i] = Random.Range(0.5f, 1.0f);
            }

            // 2) Create buffers
            basePosBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 3);
            basePosBuffer.SetData(positions);

            windWeightBuffer = new ComputeBuffer(instanceCount, sizeof(float));
            windWeightBuffer.SetData(windWeights);

            matrixBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 16, ComputeBufferType.Default);

            // 3) Indirect args buffer: indexCountPerInstance, instanceCount, startIndex, baseVertex, startInstance
            uint[] args = new uint[5]
            {
                (uint)bladeMesh.GetIndexCount(0),
                (uint)instanceCount,
                (uint)bladeMesh.GetIndexStart(0),
                (uint)bladeMesh.GetBaseVertex(0),
                0
            };
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            argsBuffer.SetData(args);

            // 4) Hook buffers to compute + material
            int kernel = compute.FindKernel("CSMain");
            compute.SetBuffer(kernel, "_BasePositions", basePosBuffer);
            compute.SetBuffer(kernel, "_WindWeights", windWeightBuffer);
            compute.SetBuffer(kernel, "_Matrices", matrixBuffer);
            compute.SetInt("_NumInstances", instanceCount);

            grassMaterial.SetBuffer("_Matrices", matrixBuffer);
        }

        void Update()
        {
            // dispatch compute
            int kernel = compute.FindKernel("CSMain");
            compute.SetFloat("_Time", Time.time);
            compute.SetFloat("_WindStrength", 0.5f);
            compute.SetFloat("_WindSpeed", 1.0f);
            compute.SetFloat("_RandomSeed", 0.1f);

            int groups = Mathf.CeilToInt((float)instanceCount / THREADS);
            compute.Dispatch(kernel, groups, 1, 1);

            // bounds must encompass all instances; Unity won't frustum cull per instance automatically with indirect draws
            Bounds bounds = new Bounds(transform.position, new Vector3(1000, 200, 1000));
            Graphics.DrawMeshInstancedIndirect(bladeMesh, 0, grassMaterial, bounds, argsBuffer);
        }

        void OnDestroy()
        {
            basePosBuffer?.Release();
            windWeightBuffer?.Release();
            matrixBuffer?.Release();
            argsBuffer?.Release();
        }
    }
}
