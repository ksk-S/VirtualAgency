#pragma strict

//@script ExecuteInEditMode
@script RequireComponent(Camera)
@script AddComponentMenu("Image Effects/Pixelation")

class Pixelation extends PostEffectsBase {
    @HideInInspector var shader : Shader;

    var scale = 20;
	var redness = 0.0f;
    private var material : Material;

    function CheckResources() {
        material = CheckShaderAndCreateMaterial(shader, material);
        return CheckSupport();
    }
    
    /*
	function OnEnable() { 
		var camera : Camera;
		camera = GetComponent(Camera);
		camera.depthTextureMode |= DepthTextureMode.DepthNormals; 
	}
	*/
	

    function OnRenderImage(source : RenderTexture, destination : RenderTexture) {
        if (!CheckResources()) {
            ReportAutoDisable();
            Graphics.Blit(source, destination);
            return;
        }

        var small = RenderTexture.GetTemporary(source.width / scale, source.height / scale, 0);

        Graphics.Blit(source, small);

        small.filterMode = FilterMode.Point;

		material.SetFloat("_Redness", redness);

        material.SetTexture("_SmallTex", small);
        Graphics.Blit(source, destination, material, 0);
        
        RenderTexture.ReleaseTemporary(small);           
    }
}
