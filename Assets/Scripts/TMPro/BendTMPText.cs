using UnityEngine;
using TMPro;



//CHATGPT CODE
[RequireComponent(typeof(TextMeshProUGUI))]
public class BendTMPText : MonoBehaviour
{
    [Range(-90f, 90f)]
    public float arc = 20f;

    private TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        tmp.ForceMeshUpdate();
        Bend();
    }

    void LateUpdate()
    {
        if (tmp.havePropertiesChanged)
        {
            tmp.ForceMeshUpdate();
            Bend();
        }
    }

    void Bend()
    {
        TMP_TextInfo textInfo = tmp.textInfo;

        if (textInfo.characterCount == 0)
            return;

        float width = tmp.rectTransform.rect.width;
        float center = width * 0.5f;

        float radians = arc * Mathf.Deg2Rad;

        // Avoid division by zero for a flat line.
        if (Mathf.Abs(radians) < 0.0001f)
            return;

        float radius = width / radians;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo character = textInfo.characterInfo[i];

            if (!character.isVisible)
                continue;

            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;

            Vector3[] vertices =
                textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = vertices[vertexIndex + j];

                float x = v.x - center;

                // Position along the arc.
                float angle = x / radius;

                float y = radius * (1f - Mathf.Cos(angle));

                v.y += y;
                vertices[vertexIndex + j] = v;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices =
                textInfo.meshInfo[i].vertices;

            tmp.UpdateGeometry(
                textInfo.meshInfo[i].mesh,
                i
            );
        }
    }
}