using UnityEngine;

public static class MaterialExtensions
{
    private const string EmissionName = "_EmissionColor";
    private static readonly int Emission = Shader.PropertyToID(EmissionName);

    public static void SetEmissionColor(this Material material, Color emissionColor)
    {
        if (!material.IsKeywordEnabled(EmissionName))
        {
            material.EnableKeyword(EmissionName);
        }

        material.SetColor(Emission, emissionColor);
    }
}
