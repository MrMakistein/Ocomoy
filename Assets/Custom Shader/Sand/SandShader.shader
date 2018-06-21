Shader "Custom/SandShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)

		[Header(Waves)]

		_NoiseTex("Noise Texture", 2D) = "white" {}
		_WaveDetail("WaveDetail", Vector) = (3.19, 16, 6.05, 1)
		_DetailStrength("WaveDetailStrength", Float) = 0.00600
	}

	SubShader
	{
		Pass
		{

			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			// required to use ComputeScreenPos()
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			// Unity built-in - NOT required in Properties

			sampler2D _NoiseTex;
			float4 _Color;
			float4 _WaveDetail;
			float _DetailStrength;


			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texCoord : TEXCOORD1;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				// convert obj-space position to camera clip space
				output.pos = UnityObjectToClipPos(input.vertex);

				// apply wave animation
				float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
				output.pos.y += sin(_Time*noiseSample)*_DetailStrength;
				output.pos.x += cos(_Time*noiseSample)*_DetailStrength;

				// compute depth (screenPos is a float4)
				output.screenPos = ComputeScreenPos(output.pos);

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return _Color;
			}
			ENDCG
		}

	}
}
