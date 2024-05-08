
using UnityEngine;

public class ParticleExtensions :MonoBehaviour
{
    public SpriteMaskInteraction interaction;

    private ParticleSystemRenderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<ParticleSystemRenderer>();

        for (int i = 0; i < renderers.Length; i++) {
            Material mat = new Material(renderers[i].sharedMaterial);
            renderers[i].sharedMaterial = mat;
        }
        SetMaskInteraction(interaction);
    }

    public void SetMaskInteraction(SpriteMaskInteraction interaction) {
        int compValue = 0;

        switch (interaction)
        {
            case SpriteMaskInteraction.None:
                compValue = 0;
                break;
            case SpriteMaskInteraction.VisibleInsideMask:
                compValue = 2;
                break;
            case SpriteMaskInteraction.VisibleOutsideMask:
                compValue = 3;
                break;
        }
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial.SetFloat("_CompValue", compValue);
        }
    }
}
