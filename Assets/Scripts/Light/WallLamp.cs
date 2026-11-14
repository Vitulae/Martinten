using UnityEngine;

public class WallLamp : MonoBehaviour
{
    [SerializeField] private Transform lightOrigin;
    [SerializeField, Min(0.1f)] private float range = 12f;
    [SerializeField] private MeshRenderer lightSurface;

    private Texture2D occlusionMap;

    private void Start()
    {
        if (lightOrigin == null || lightSurface == null)
            return;

        const int sampleCount = 720;
        occlusionMap = new Texture2D(sampleCount, 1, TextureFormat.RFloat, false, true);
        occlusionMap.filterMode = FilterMode.Point;
        occlusionMap.wrapMode = TextureWrapMode.Repeat;

        float[] distances = new float[sampleCount];
        Transform surface = lightSurface.transform;

        for (int i = 0; i < sampleCount; i++)
        {
            float angle = (i + 0.5f) / sampleCount * 2f * Mathf.PI;
            Vector3 direction = surface.right * Mathf.Cos(angle)
                                + surface.up * Mathf.Sin(angle);

            distances[i] = Mathf.Clamp01(MeasureLightDistance(direction) / range);
        }

        occlusionMap.SetPixelData(distances, 0);
        occlusionMap.Apply(false, true);

        var properties = new MaterialPropertyBlock();
        lightSurface.GetPropertyBlock(properties);
        properties.SetTexture("_OcclusionMap", occlusionMap);
        lightSurface.SetPropertyBlock(properties);
    }

    private void OnDestroy()
    {
        if (occlusionMap != null)
            Destroy(occlusionMap);
    }
    private float MeasureLightDistance(Vector3 direction)
    {
        if (Physics.Raycast(
                lightSurface.transform.position + Vector3.up * 0.15f,
                direction,
                out RaycastHit hit,
                range,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
        {
            return hit.distance;
        }

        return range;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (lightOrigin == null)
            return;

        Gizmos.color = Color.darkGoldenRod;
        Gizmos.DrawWireSphere(lightOrigin.position, range);
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

            bool blocked = Physics.Raycast(
                lightOrigin.position, direction, out RaycastHit hit,
                range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            Gizmos.color = blocked ? Color.red : Color.green;
            Gizmos.DrawRay(lightOrigin.position, direction * (blocked ? hit.distance : range));
        }
    }
}
