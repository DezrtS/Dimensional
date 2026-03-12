using Managers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Systems.Shaders
{
    public class Clouds : MonoBehaviour
    {
        private static readonly int MidYValueProperty = Shader.PropertyToID("_midYValue");
        private static readonly int CloudHeightProperty = Shader.PropertyToID("_cloudHeight");
        private static readonly int CameraPositionProperty = Shader.PropertyToID("_cameraPosition");

        [SerializeField] private int horizontalStackSize = 20;
        [SerializeField] private float cloudHeight;
        [SerializeField] private Mesh quadMesh;
        [SerializeField] private Material cloudMaterial;

        [SerializeField] private bool disableRuntimeMaterial;

        private Transform _targetTransform;
        private Camera _camera;

        private Matrix4x4[] _matrices;
        private RenderParams _renderParams;

        public Material RuntimeClouds { get; private set; }

        private void Awake()
        {
            // Create runtime material instance
            RuntimeClouds = disableRuntimeMaterial ? cloudMaterial : Instantiate(cloudMaterial);

            // Preallocate matrices
            _matrices = new Matrix4x4[horizontalStackSize];

            // Setup RenderParams
            _renderParams = new RenderParams(RuntimeClouds)
            {
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = false,
                worldBounds = new Bounds(Vector3.zero, Vector3.one * 2500f)
            };

            var instance = CameraManager.Instance;
            if (instance != null)
            {
                _camera = instance.Camera;
            }
            else
            {
                CameraManager.Initialized += cam => _camera = cam.Camera;
            }
        }

        private void Start()
        {
            _targetTransform = CameraManager.Instance.Camera.transform;
        }

        private void FixedUpdate()
        {
            // Keep clouds centered horizontally on the camera
            var pos = _targetTransform.position;
            pos.y = transform.position.y;
            transform.position = pos;
        }

        private void Update()
        {
            // Rebuild matrices if stack size changed or transform moved
            if (transform.hasChanged || _matrices.Length != horizontalStackSize)
            {
                RecalculateMatrices();
                transform.hasChanged = false;
            }

            // Update shader properties directly on the runtime material
            RuntimeClouds.SetFloat(MidYValueProperty, transform.position.y);
            RuntimeClouds.SetFloat(CloudHeightProperty, cloudHeight);
            RuntimeClouds.SetVector(CameraPositionProperty, _camera.transform.position);

            // Draw all cloud layers
            Graphics.RenderMeshInstanced(
                _renderParams,
                quadMesh,
                0,
                _matrices,
                horizontalStackSize
            );
        }

        private void RecalculateMatrices()
        {
            if (_matrices.Length != horizontalStackSize)
                _matrices = new Matrix4x4[horizontalStackSize];

            Vector3 start = transform.position + Vector3.up * (cloudHeight / 2f);

            for (int i = 0; i < horizontalStackSize; i++)
            {
                Vector3 pos = start - Vector3.up * (cloudHeight * i / horizontalStackSize);
                _matrices[i] = Matrix4x4.TRS(pos, transform.rotation, transform.localScale);
            }
        }
    }
}
