using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Agent
{
    /// <summary>
    /// Bakes the collider for the agent
    /// </summary>
    public class ColliderBaker : MonoBehaviour
    {
        void Start()
        {
            SkinnedMeshRenderer meshRenderer = GetComponent<SkinnedMeshRenderer>();
            MeshCollider coll = GetComponent<MeshCollider>();

            Mesh colliderMesh = new Mesh();
            meshRenderer.BakeMesh(colliderMesh);
            var sharedMesh = coll.sharedMesh;
            sharedMesh = colliderMesh;
            coll.sharedMesh = sharedMesh;
        }
    }
}
