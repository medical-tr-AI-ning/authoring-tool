using UnityEngine;
using UnityEngine.Events;

public class MelanomaCollider : MonoBehaviour
{
    Melanoma _melanoma;

    public void SetMelanoma(Melanoma melanoma)
    {
        _melanoma = melanoma;
        if(gameObject.GetComponent<SphereCollider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }

        gameObject.GetComponent<SphereCollider>().radius = _melanoma.SizeUI / 50;
        transform.position = _melanoma.Position;
    }

    public Melanoma GetMelanoma()
    {
        return _melanoma;
    }

    public static void SetMelanomaCollider(Melanoma melanoma)
    {
        if(melanoma.collider == null)
        {
            melanoma.collider = new GameObject("MelanomaCollider");
            melanoma.collider.layer = LayerMask.NameToLayer("Melanoma");
            melanoma.collider.AddComponent<MelanomaCollider>();
        }

        melanoma.collider.GetComponent<MelanomaCollider>().SetMelanoma(melanoma);
    }
}
