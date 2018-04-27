Shader "Custom/CelShader" {
	Properties { //show up in the material inspector
		_MainTex("Texture", 2D) = "white" {}
		_RampTex("Ramp", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1)
		_OutlineExtrusion("Outline Extrusion", float) = 0
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineDot("Outline Dot", float) = 0.25
		_Aufheller("Aufheller", float) = 0.2
		_RampTint("RampTint", Color) = (1, 1, 1, 1)
		_EdgeTolerance("Edge Tolerance", float) = 0.7
	}
	SubShader { //min. 1 required. Unity goes through list of SubShaders and picks the first one that is compatible with the end user's device. if none: fallback shader
	//subshaders have #passes# -> the pass block causes the geometry of a GameObject to be rendered once!
	//A Pass can have a name and an arbitrary number of tags. (-> these name/value strings communicate the Pass's intent to the rendering engine)

		//Regular Color and Lighting pass
		Pass {
			Tags {
			 	"LightMode" = "ForwardBase"  //allows shadow rec/cast
			}

			// Write to Stencil buffer (so that outline pass can read)
			Stencil
			{
				Ref 4
				Comp always
				Pass replace
				ZFail keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase // shadows
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"

			// Properties
			sampler2D _MainTex;
			sampler2D _RampTex;
			float4 _Color;
			float4 _LightColor0; // provided by Unity
			
			float _Aufheller;
			float3 _RampTint;
			float _EdgeTolerance;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
				LIGHTING_COORDS(1,2) // shadows
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				// convert input to world space
				output.pos = UnityObjectToClipPos(input.vertex);
				float4 normal4 = float4(input.normal, 0.0); // need float4 to mult with 4x4 matrix
				output.normal = normalize(mul(normal4, unity_WorldToObject).xyz);

				output.texCoord = input.texCoord;

                TRANSFER_VERTEX_TO_FRAGMENT(output); // shadows
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				// lighting mode
				
				// convert light direction to world space & normalize
				// _WorldSpaceLightPos0 provided by Unity
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				// finds location on ramp texture that we should sample
				// based on angle between surface normal and light direction
				float ramp = clamp(dot(input.normal, lightDir), 0.05, 1.0); //vom lichteinfallswinkel abhängiger wert (wie hell wird der vertex(?)/Punkt(?) beleuchtet)
				//--> wenn dot product null: wird gar nicht beleuchtet (links auf ramp), sonst rechts = 1 oder dazwischen
				float3 lighting = tex2D(_RampTex, float2(ramp, 0.5)).rgb; //wert der Textur[ramp]
				float3 tintedLighting = float3(0, 0, 0);
				if(lighting.r > _EdgeTolerance && lighting.g > _EdgeTolerance && lighting.b > _EdgeTolerance) {
					tintedLighting = lighting;
				} else {
					tintedLighting = (lighting * _RampTint) / 0.5;
					/*tintedLighting.r = clamp(lighting.r * _RampTint, 0, 1);
					tintedLighting.g = clamp(lighting.g * _RampTint, 0, 1);
					tintedLighting.b = clamp(lighting.b * _RampTint, 0, 1);
					*/
				}
				
				// sample texture for color
				float4 albedo = tex2D(_MainTex, input.texCoord.xy); //wert der main tex (falls vorhanden)

				float attenuation = LIGHT_ATTENUATION(input) + _Aufheller; // shadow value
				float3 rgb = albedo.rgb * _LightColor0.rgb * tintedLighting * _Color.rgb * attenuation;

				//float3 crossProduct = float3(rgb.g * _RampTint.b - rgb.b * _RampTint.g, 
				//						rgb.b * _RampTint.r - rgb.r * _RampTint.b, 
				//						rgb.r * _RampTint.g - rgb.g * _RampTint.r);

				return float4(rgb, 1.0);
			}

			ENDCG
		} //end lighting pass

			

		

		//Shadow pass
		Pass {
			Tags {
				"LightMode" = "ShadowCaster"
				//"NoShadow" = "True"
			}

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
    	} //end shadow pass

		// Outline pass
		Pass
		{
			//Blend DstColor SrcColor // Multiplicative

			// Won't draw where it sees ref value 4
			Cull OFF
			ZWrite OFF
			ZTest ON
			Stencil
			{
				Ref 4
				Comp notequal
				Fail keep
				Pass replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Properties
			uniform float4 _OutlineColor;
			uniform float _OutlineSize;
			uniform float _OutlineExtrusion;
			uniform float _OutlineDot;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4 newPos = input.vertex;

				// normal extrusion technique
				float3 normal = normalize(input.normal);
				newPos += float4(normal, 0.0) * _OutlineExtrusion;

				// convert to world space
				output.pos = UnityObjectToClipPos(newPos);

				output.color = _OutlineColor;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				// checker value will be negative for 4x4 blocks of pixels
				// in a checkerboard pattern
				//input.pos.xy = floor(input.pos.xy * _OutlineDot) * 0.5;
				//float checker = -frac(input.pos.r + input.pos.g);

				// clip HLSL instruction stops rendering a pixel if value is negative
				//clip(checker);

				return input.color;
			}

			ENDCG
		
		}//end outline pass



//default
//		Tags { "RenderType"="Opaque" }
//		LOD 200
//
//		CGPROGRAM
//		// Physically based Standard lighting model, and enable shadows on all light types
//		#pragma surface surf Standard fullforwardshadows
//
//		// Use shader model 3.0 target, to get nicer looking lighting
//		#pragma target 3.0
//
//		sampler2D _MainTex;
//
//		struct Input {
//			float2 uv_MainTex;
//		};
//
//		half _Glossiness;
//		half _Metallic;
//		fixed4 _Color;
//
//		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
//		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
//		// #pragma instancing_options assumeuniformscaling
//		UNITY_INSTANCING_BUFFER_START(Props)
//			// put more per-instance properties here
//		UNITY_INSTANCING_BUFFER_END(Props)
//
//		void surf (Input IN, inout SurfaceOutputStandard o) {
//			// Albedo comes from a texture tinted by color
//			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb;
//			// Metallic and smoothness come from slider variables
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
//		}
//		ENDCG

	} //end subshader
} //end shader


//TO RESEARCH: Shader LOD for custom shaders, Shader Replacements