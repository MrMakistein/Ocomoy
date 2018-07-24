Shader "Custom/CelShader" {
	Properties{ //show up in the material inspector
		_MainTex("Texture", 2D) = "white" {}

		_BumpMap("Normal Map", 2D) = "bump" { }
		_BumpHeight("Bump Height", float) = 1
	_Cutoff("Alpha Cut off", Range(0, 1)) = 0.1
		_RampTex("Ramp", 2D) = "white" {}
	_Color("Color", Color) = (1, 1, 1)
		_OutlineExtrusion("Outline Extrusion", float) = 0
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineDot("Outline Dot", float) = 0.25
		_Aufheller("Aufheller", float) = 0.2
		_RampTint("RampTint", Color) = (1, 1, 1, 1)
		_EdgeTolerance("Edge Tolerance", float) = 0.7

		[Header(Ramp)]
	[MaterialToggle] _UseRampTexture("Use Ramp from Slider", float) = 0
		_RampColor1("Ramp Color 1", color) = (0, 0, 0, 1)
		_RampPos1("Ramp Position 1", Range(0, 1)) = 0
		_RampColor2("Ramp Color 2", color) = (0.25, 0.25, 0.25, 1)
		_RampPos2("Ramp Position 2", Range(0, 1)) = 0.33
		_RampColor3("Ramp Color 3", color) = (0.50, 0.50, 0.50, 1)
		_RampPos3("Ramp Position 3", Range(0, 1)) = 0.66
		_RampColor4("Ramp Color 4", color) = (0.75, 0.75, 0.75, 1)
		_RampColorBlend("Ramp Blend", Range(0, 1)) = 0.0

		[Header(Gradient)]
	[MaterialToggle] _UseGradientColors("Use Gradients:", float) = 0
		_GradientStartHeight("Gradient Start Height", float) = 0
		_GradientScale("Gradient Size", float) = 1
		_RampColor1Grad("Ramp Color 1 Gradient", color) = (0, 0, 0, 1)
		_RampColor2Grad("Ramp Color 2 Gradient", color) = (0, 0, 0, 1)
		_RampColor3Grad("Ramp Color 3 Gradient", color) = (0, 0, 0, 1)
		_RampColor4Grad("Ramp Color 4 Gradient", color) = (0, 0, 0, 1)


		_LightmapIntensity("Lightmap intesity",  Range(0, 1)) = 0.66


	}
	SubShader
	{ //min. 1 required. Unity goes through list of SubShaders and picks the first one that is compatible with the end user's device. if none: fallback shader
				   //subshaders have #passes# -> the pass block causes the geometry of a GameObject to be rendered once!
				   //A Pass can have a name and an arbitrary number of tags. (-> these name/value strings communicate the Pass's intent to the rendering engine)
		Tags{
			"Queue" = "AlphaTest"
			"RenderType" = "TransparentCutout"
		}

		// Outline pass

		Pass
	{
		//Blend DstColor SrcColor // Multiplicative

		// Won't draw where it sees ref value 4
		Cull Front
		ZWrite OFF
		ZTest ON

		Stencil
		{
			Ref 4
			Comp always
			Pass replace
			ZFail keep
		}

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		// Properties
		uniform float4 _OutlineColor;
	uniform float _OutlineSize;
	uniform float _OutlineExtrusion;
	uniform float _OutlineDot;
	uniform float4 _MousePos;

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
		if (_OutlineExtrusion == 0) {
			discard;
		}


	//calculate screen position
	float4 screenPos = ComputeScreenPos(input.pos);

	//_CutObjPos
	float dx = _MousePos.x / 2 - screenPos.x + 5;
	float dy = -_MousePos.y / 2 - screenPos.y;


	//include aspect ratio, that cutout appears round |not needed apparently
	//dy *= _ScreenParams.y / _ScreenParams.x;
	float dist = (sqrt(dx * dx + dy * dy) / (_ScreenParams.y / 20));
	dist = 0;
	input.color.a = input.color.a - saturate(-0.2 + dist);
	return input.color;
	}

		ENDCG

	}//end outline pass

		//Regular Color and Lighting pass
		Pass{
			Tags{
				"LightMode" = "ForwardBase"  //allows shadow rec/cast
				//"LightMode" = "Vertex"
			}
			//Lighting Off
			
		// Write to Stencil buffer (so that outline pass can read)
		Stencil
		{
			Ref 4
			Comp always
			Pass replace
			ZFail keep
		}

		Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag



				//from Double Sided shader
		#define UNITY_PASS_FORWARDBASE
		#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
		#include "AutoLight.cginc"
		#include "UnityCG.cginc"
		#pragma multi_compile_fwdbase_fullshadows
		// Properties
		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _BumpMap_ST;
		float _BumpHeight;
		float _Cutoff;

		sampler2D _RampTex;
		float4 _Color;
		float4 _LightColor0; // provided by Unity


		float4 _MainTex_ST; //for uv tiling


							// These are prepopulated by Unity
							// sampler2D unity_Lightmap;
							// float4 unity_LightmapST;


		float _Aufheller;
		float3 _RampTint;
		float _EdgeTolerance;

		float _UseRampTexture;
		float _UseGradientColors;

		float4 _RampColor1;
		float4 _RampColor2;
		float4 _RampColor3;
		float4 _RampColor4;

		float4 _RampColor1Grad;
		float4 _RampColor2Grad;
		float4 _RampColor3Grad;
		float4 _RampColor4Grad;

		float _GradientStartHeight;
		float _GradientScale;

		float _RampPos1;
		float _RampPos2;
		float _RampPos3;
		float _RampColorBlend;

		//helper variables
		float lowSliderPos;
		float midSliderPos;
		float highSliderPos;
		float temp;


		float _LightmapIntensity;

		//returns the color from the "ramp", given a certain position
		float4 getColorFromSlider(float pos, float gradientPos) {

			//sort sliders to low, mid & high
			lowSliderPos = _RampPos1;
			midSliderPos = _RampPos2;
			highSliderPos = _RampPos3;

			while (lowSliderPos > midSliderPos || midSliderPos > highSliderPos) {

				if (lowSliderPos > midSliderPos) {
					float temp = lowSliderPos;
					lowSliderPos = midSliderPos;
					midSliderPos = temp;
				}

				if (midSliderPos > highSliderPos) {
					float temp = highSliderPos;
					highSliderPos = midSliderPos;
					midSliderPos = temp;
				}
			}

			//end sort

			//if Gradient is not used, set the calculation to 0, not performant, but more readable :O
			if (_UseGradientColors == 0) {
				gradientPos = 0;
			}

			//getGradientInformation
			//calculate color based on Gradient
			//get color according to Position and Ramp blending
			if (pos < lowSliderPos - _RampColorBlend) {
				return lerp(_RampColor1, _RampColor1Grad, gradientPos);
			}

			else if (pos < lowSliderPos + _RampColorBlend) { //blend
				//lerps the two colors
				float4 col1 = lerp(_RampColor1, _RampColor1Grad, gradientPos);
				float4 col2 = lerp(_RampColor2, _RampColor2Grad, gradientPos);

				//lerps the two colors together, uses the position of the blend sector as input-> needs to be mapped to the Range of the blend
				return lerp(col2, col1, (lowSliderPos + _RampColorBlend - pos) / (2 * _RampColorBlend));
			}

			else if (pos < midSliderPos - _RampColorBlend) {
				return lerp(_RampColor2, _RampColor2Grad, gradientPos);
			}

			else if (pos < midSliderPos + _RampColorBlend) { //blend
																 
				float4 col1 = lerp(_RampColor2, _RampColor2Grad, gradientPos);
				float4 col2 = lerp(_RampColor3, _RampColor3Grad, gradientPos);

				//lerps the two colors together, uses the position of the blend sector as input-> needs to be mapped to the Range of the blend
				return lerp(col2, col1, (midSliderPos + _RampColorBlend - pos) / (2 * _RampColorBlend));
			}

			else if (pos < highSliderPos - _RampColorBlend) {
				return lerp(_RampColor3, _RampColor3Grad, gradientPos);
			}

			else if (pos < highSliderPos + _RampColorBlend) { //blend
																  
				float4 col1 = lerp(_RampColor3, _RampColor3Grad, gradientPos);
				float4 col2 = lerp(_RampColor4, _RampColor4Grad, gradientPos);

				//lerps the two colors together, uses the position of the blend sector as input-> needs to be mapped to the Range of the blend
				return lerp(col2, col1, (highSliderPos + _RampColorBlend - pos) / (2 * _RampColorBlend));
			}

			else
			{
				return lerp(_RampColor4, _RampColor4Grad, gradientPos);
			}
		}

		


		struct vertexInput
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float3 texCoord : TEXCOORD0;
			float2 texCoord1 : TEXCOORD1;
			float4 tangent : TANGENT;
		};

		struct vertexOutput
		{
			float4 pos : SV_POSITION;
			float3 normal : NORMAL;
			float3 texCoord : TEXCOORD0;
			//float3 normalDir : TEXCOORD1;
			float3 screenPos : TEXCOORD2;
			float2 lightMap : TEXCOORD3;
			float3 objectPos : TEXCOORD4;
			LIGHTING_COORDS(5,6) // shadows

			half3 tspace0 : TEXCOORD7; // tangent.x, bitangent.x, normal.x
			half3 tspace1 : TEXCOORD8; // tangent.y, bitangent.y, normal.y
			half3 tspace2 : TEXCOORD9; // tangent.z, bitangent.z, normal.z
		};


		vertexOutput vert(vertexInput input)
		{
			vertexOutput output;

			// convert input to world space
			output.pos = UnityObjectToClipPos(input.vertex);
			float4 normal4 = float4(input.normal, 0.0); // need float4 to mult with 4x4 matrix

			output.normal = normalize(mul(normal4, unity_WorldToObject).xyz);
			output.texCoord = input.texCoord;

			output.screenPos = ComputeScreenPos(output.pos);
			output.objectPos = mul(unity_ObjectToWorld, input.vertex);
			//for normal map
			half3 wNormal = UnityObjectToWorldNormal(input.normal);
			half3 wTangent = UnityObjectToWorldDir(input.tangent.xyz);
			// compute bitangent from cross product of normal and tangent
			half tangentSign = input.tangent.w * unity_WorldTransformParams.w;
			half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
			// output the tangent space matrix
			output.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
			output.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
			output.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);


	#if LIGHTMAP_ON
			output.lightMap = input.texCoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif


			TRANSFER_VERTEX_TO_FRAGMENT(output); // shadows
			return output;
		}

		float4 frag(vertexOutput input, float facing : VFACE) : COLOR
		{

			//normal mapping
			half2 normalUV = TRANSFORM_TEX(input.texCoord.xy, _BumpMap);
			
			//+= tex2D(_BumpMap, input.texCoord.xy);//TRANSFORM_TEX(input.texCoord, _BumpMap));

			half3 tnormal = UnpackNormal(tex2D(_BumpMap, normalUV));
			//invert red for correct calculation
			tnormal.r *= -1;
			tnormal.z = _BumpHeight;
			// transform normal from tangent to world space
			half3 worldNormal;
			worldNormal.x = dot(input.tspace0, tnormal);
			worldNormal.y = dot(input.tspace1, tnormal);
			worldNormal.z = dot(input.tspace2, tnormal);

			// convert light direction to world space & normalize
			// _WorldSpaceLightPos0 provided by Unity
			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

			// finds location on ramp texture that we should sample
			// based on angle between surface normal and light direction

			float ramp = clamp(dot(input.normal, lightDir), 0.05, 1.0);		//vom lichteinfallswinkel abhängiger wert (wie hell wird der vertex(?)/Punkt(?) beleuchtet)
																		//--> wenn dot product null: wird gar nicht beleuchtet (links auf ramp), sonst rechts = 1 oder dazwischen

			ramp = clamp(dot(worldNormal, lightDir), 0.05, 1.0);
			
			float3 lighting = float3(0, 0, 0);
			float3 tintedLighting = float3(0, 0, 0);

			if (_UseRampTexture == 0) {
				lighting = tex2D(_RampTex, float2(ramp, 0.5)).rgb; //wert der Textur[ramp]
			}
			else
			{
				//multiplied with ModelVeiwProjectionMatrix, to get the real world position
				lighting = getColorFromSlider(ramp, clamp(input.objectPos.y * 0.01 * _GradientScale - _GradientStartHeight,0,1));
			}
			if (lighting.r > _EdgeTolerance && lighting.g > _EdgeTolerance && lighting.b > _EdgeTolerance) {
				tintedLighting = lighting;
			}
			else {
				tintedLighting = (lighting * _RampTint) / 0.5;

			}

			//texture tiling
			float2 newUV = TRANSFORM_TEX(input.texCoord, _MainTex);
			// sample texture for color
			float4 albedo = tex2D(_MainTex, newUV.xy); //wert der main tex (falls vorhanden)

			float attenuation = LIGHT_ATTENUATION(input) + _Aufheller; // shadow value
			float3 rgb = albedo.rgb * _LightColor0.rgb * tintedLighting * _Color.rgb * attenuation;


			//handles alpha cutoff
			if (albedo.a < _Cutoff)
			{
				discard;
			}

			#if LIGHTMAP_ON
			
			float3 lightMapContribution = 2 * DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.lightMap));

			float3 Base = rgb;
			float3 Blend = lightMapContribution;

			float3 Out;
			float Opacity = _LightmapIntensity;



			Out = Base * Blend;
			Out = lerp(Base, Out, Opacity);


			rgb = Out;
			#endif

			return float4(saturate(rgb), 1);
		}

			ENDCG
		} //end lighting pass


		//-----------------------------------------------------------------------------------------------------------------------------------------------


		  //Shadow pass
			Pass{
			Tags{
			"LightMode" = "ShadowCaster"
			//"NoShadow" = "True"
		}
			/*
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster

			#define UNITY_PASS_SHADOWCASTER
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;
			float _Cutoff;

			struct v2f {
			V2F_SHADOW_CASTER;
			float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
			v2f o;
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			return o;
			}

			float4 frag(v2f i) : SV_Target
			{

			fixed4 texcol = tex2D(_MainTex, i.uv);
			if (texcol.a < _Cutoff)
			{
			discard;
			}
			SHADOW_CASTER_FRAGMENT(i)

			}
			*/
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_SHADOWCASTER
			#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
			#define _GLOSSYENV 1
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fog
			#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
			#pragma target 3.0
			uniform sampler2D _MainTex; 
			uniform float4 _MainTex_ST;
			uniform float _Cutoff;
			struct VertexInput {
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
			};
			struct VertexOutput {
				V2F_SHADOW_CASTER;
				float2 uv0 : TEXCOORD1;
				float2 uv1 : TEXCOORD2;
				float2 uv2 : TEXCOORD3;
				float4 posWorld : TEXCOORD4;
			};

			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.uv1 = v.texcoord1;
				o.uv2 = v.texcoord2;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				TRANSFER_SHADOW_CASTER(o)
					return o;
			}
			float4 frag(VertexOutput i, float facing : VFACE) : COLOR{
			
			/*float isFrontFace = (facing >= 0 ? 1 : 0);
			float faceSign = (facing >= 0 ? 1 : -1);
			float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
			float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
			clip(_MainTex_var.a - 0.5);*/
			//handles alpha cutoff

			float2 newUV = TRANSFORM_TEX(i.uv0, _MainTex);
				float4 albedo = tex2D(_MainTex, newUV.xy); //wert der main tex (falls vorhanden)
				if (albedo.a < _Cutoff)
				{
					discard;
				}
			SHADOW_CASTER_FRAGMENT(i)
			}
				ENDCG

			} //end shadow pass


			//---------------------------------------------------------------------------------------------------------------------



					

	//________________________________________________________________________________________________________________________________________
	 /*
	 //Lightmap pass
	 Pass
	 {
	 #pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	 #ifdef LIGHTMAP_ON
	 half4 unity_LightmapST;
	 sampler2D_half unity_Lightmap;
	 #endif

	 c.rgb *= (DecodeLightmap(tex2D(unity_Lightmap, i.uv[1]))) * _Power;
	 }
	 */
	} //end subshader
		Fallback "Transparent/Cutout/Diffuse"
} //end shader


  //TO RESEARCH: Shader LOD for custom shaders, Shader Replacements
