using UnityEngine;

public class FlashlightOcclusion : MonoBehaviour
{
    [SerializeField] private Transform lightOrigin;
    [SerializeField] private MeshRenderer lightSurface;
    [SerializeField, Min(0.1f)] private float range = 18f;

    private const int SampleCount = 360;
    private readonly float[] distances = new float[SampleCount];
    private readonly RaycastHit[] hits = new RaycastHit[16];
    private Texture2D occlusionMap;

    private void Start()
    {
        if (lightOrigin == null || lightSurface == null)
        {
            Debug.LogError("Assign Light Origin and Light Surface.", this);
            enabled = false;
            return;
        }

        occlusionMap = new Texture2D(SampleCount, 1, TextureFormat.RFloat, false, true);
        occlusionMap.filterMode = FilterMode.Point;
        occlusionMap.wrapMode = TextureWrapMode.Repeat;

        UpdateOcclusion();

        var properties = new MaterialPropertyBlock();
        lightSurface.GetPropertyBlock(properties);
        properties.SetTexture("_OcclusionMap", occlusionMap);
        lightSurface.SetPropertyBlock(properties);
    }

    private void LateUpdate()
    {
        UpdateOcclusion();
    }

    private void UpdateOcclusion()
    {
        Transform surface = lightSurface.transform;
        Vector3 origin = surface.position;
        origin.y = lightOrigin.position.y;

        for (int i = 0; i < SampleCount; i++)
        {
            float angle = (i + 0.5f) * 2f * Mathf.PI / SampleCount;
            Vector3 direction = surface.right * Mathf.Cos(angle)
                              + surface.up * Mathf.Sin(angle);

            int count = Physics.RaycastNonAlloc(
                origin, direction, hits, range,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            RaycastHit[] results = hits;
            if (count == hits.Length)
            {
                results = Physics.RaycastAll(
                    origin, direction, range,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
                count = results.Length;
            }

            float nearest = range;
            for (int j = 0; j < count; j++)
            {
                if (results[j].collider.transform.IsChildOf(transform))
                    continue;

                nearest = Mathf.Min(nearest, results[j].distance);
            }

            distances[i] = nearest / range;
        }

        occlusionMap.SetPixelData(distances, 0);
        occlusionMap.Apply(false, false);
    }

    private void OnDestroy()
    {
        if (occlusionMap != null)
            Destroy(occlusionMap);
    }
}