using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CutOffMaskUI : Image
{
    private Material _customMaterial;

    public override Material materialForRendering
    {
        get
        {
            if (_customMaterial == null)
            {
                _customMaterial = new Material(base.materialForRendering);

                // Ustawienia stencil maski
                _customMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);

                // Ustawienia przezroczystoœci
                _customMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                _customMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                _customMaterial.SetInt("_ZWrite", 0);
                _customMaterial.DisableKeyword("_ALPHATEST_ON");
                _customMaterial.EnableKeyword("_ALPHABLEND_ON");
                _customMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                _customMaterial.renderQueue = (int)RenderQueue.Transparent;
            }
            return _customMaterial;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_customMaterial != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(_customMaterial);
            }
            else
#endif
            {
                Destroy(_customMaterial);
            }
        }
    }
}
