// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Shows the grayscale of the depth from the camera.
 
Shader "Custom/DepthShader"
{
   /* SubShader
    {
        Tags { "RenderType"="Opaque" }
 
        Pass
        {
 
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            uniform sampler2D _CameraDepthTexture; //the depth texture
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD1; //Screen position of pos
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
 
                return o;
            }
 
            half4 frag(v2f i) : COLOR
            {
                //Grab the depth value from the depth texture
                //Linear01Depth restricts this value to [0, 1]
                float depth = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
 
                half4 c;
                c.r = depth;
                c.g = depth;
                c.b = depth;
                c.a = 1;
 
                return c;
            }
 
            ENDCG
        }
    }
    FallBack "VertexLit"
	*/


	SubShader {
		Tags { "RenderType"="Opaque" }

		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		sampler2D _CameraDepthTexture;

		struct v2f {
		   float4 pos : SV_POSITION;
		   float4 scrPos:TEXCOORD1;
		};

		//Vertex Shader
		v2f vert (appdata_base v){
		   v2f o;
		   o.pos = UnityObjectToClipPos (v.vertex);
		   o.scrPos=ComputeScreenPos(o.pos);
		   //for some reason, the y position of the depth texture comes out inverted
		   o.scrPos.y = 1 - o.scrPos.y;
		   return o;
		}

		//Fragment Shader
		half4 frag (v2f i) : COLOR{
		   float depthValue = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
		   half4 depth;

		   depth.r = depthValue;
		   depth.g = depthValue;
		   depth.b = depthValue;

		   depth.a = 1;
		   return depth;
		}
		ENDCG
		}
	}
	FallBack "Diffuse"

}

