Shader "Custom/WaterShaderv2" {
	Properties
	{
		// color of the water
		_Color("Color", Color) = (1, 1, 1, 0)
		// color of the edge effect
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 0)
		// width of the edge effect
		_DepthFactor("Depth Factor", float) = 1.0
	  
		_WaveSpeed("Wave Speed", float) = 1.0

		_WaveAmp("Wave Amp", float) = 0.2
	
		_NoiseTex("Noise Texture", 2D) = "white" {}
		
		_DepthRampTex("Depth Ramp", 2D) = "white" {}

		_DistortStrength("Distort Strength", float) = 1.0

		_ExtraHeight("Extra Height", float) = 0.0

		_EnableRamp("Enable Ramp", float) = 0.0
	}

	SubShader
	{
	
		Tags
		{ 
			"Queue" = "Transparent"
		}

			// Grab the screen behind the object into _BackgroundTexture
			GrabPass
		{
			"_BackgroundTexture"
		}

			// Background distortion
		/*
		Pass
		{

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		// Properties
		sampler2D _BackgroundTexture;
		sampler2D _NoiseTex;
		float     _DistortStrength;
		float  _WaveSpeed;
		float  _WaveAmp;

		struct vertexInput
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float3 texCoord : TEXCOORD0;
		};

		struct vertexOutput
		{
			float4 pos : SV_POSITION;
			float4 grabPos : TEXCOORD0;
		};

		vertexOutput vert(vertexInput input)
		{
			vertexOutput output;

			// convert input to world space
			output.pos = UnityObjectToClipPos(input.vertex);
			float4 normal4 = float4(input.normal, 0.0);
			float3 normal = normalize(mul(normal4, unity_WorldToObject).xyz);

			// use ComputeGrabScreenPos function from UnityCG.cginc
			// to get the correct texture coordinate
			output.grabPos = ComputeGrabScreenPos(output.pos);

			// distort based on bump map
			float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
			output.grabPos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp * _DistortStrength;
			output.grabPos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp * _DistortStrength;

			return output;
		}

		float4 frag(vertexOutput input) : COLOR
		{
			return tex2Dproj(_BackgroundTexture, input.grabPos);
		}
			ENDCG
		}
		*/
		
		Pass	
		{
		
            Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			// required to use ComputeScreenPos()
			#include "UnityCG.cginc"


			#pragma vertex vert
			#pragma fragment frag
 
			// Unity built-in - NOT required in Properties
			sampler2D _CameraDepthTexture;
			sampler2D _DepthRampTex;			
			sampler2D _NoiseTex;
			float _ExtraHeight;
			float _DepthFactor;
			float4 _EdgeColor;
			float4 _Color;
            float _DistortStrength;
			float _WaveSpeed;
			float _WaveAmp;

			float _EnableRamp;


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
				output.pos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp + _ExtraHeight;
				output.pos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp;

				// compute depth (screenPos is a float4)
				output.screenPos = ComputeScreenPos(output.pos);

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				// sample camera depth texture
				float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
				
				float depth = LinearEyeDepth(depthSample).r;

				// Because the camera depth texture returns a value between 0-1,
				// we can use that value to create a grayscale color
				// to test the value output.
				//float4 foamLine = float4(depth, depth, depth, 1);

				//return foamLine;

				 // apply the DepthFactor to be able to tune at what depth values
				// the foam line actually starts
				//***** Saturate seems to be clamping (http://developer.download.nvidia.com/cg/saturate.html)
				float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));//1 - saturate(_DepthFactor * (depth - input.screenPos.w));

				
				// sample the ramp texture
				//***** Tex2D: looks at a texture (sampler) at a specific position (sampler, position) http://developer.download.nvidia.com/cg/tex2D.html
				
				float4 foamRamp = float4(tex2D(_DepthRampTex, float2(foamLine, 0.5)).rgb, 1.0);
  
				// multiply the edge color by the foam factor to get the edge,
				// then add that to the color of the water
				float4 col; 
				if(_EnableRamp >= 1.0) {
					col = foamRamp * _Color;
				} else {
					col = foamLine * _EdgeColor + _Color;
				}
				//float4 col = foamRamp * _Color;
				return col;
			}

			ENDCG
		}
	}
}
