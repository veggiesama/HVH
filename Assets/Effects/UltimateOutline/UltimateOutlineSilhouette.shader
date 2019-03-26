//This version of the shader does not support shadows, but it does support transparent outlines

Shader "Custom/UltimateOutlineSilhouette"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_SilhouetteColor("Silhouette Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Texture", 2D) = "white" {}

		_OutlineColor("Outline color", Color) = (1,0,0,0.5)
		_OutlineWidth("Outlines width", Range(0.0, 2.0)) = 0.15

		_Angle("Switch shader on angle", Range(0.0, 180.0)) = 89
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float4 normal : NORMAL;
	};

	uniform float4 _OutlineColor;
	uniform float _OutlineWidth;

	uniform sampler2D _MainTex;
	uniform float4 _SilhouetteColor;
	uniform float4 _Color;
	uniform float _Angle;
	uniform float _Transparency;

	ENDCG

	SubShader{

		// Silhouette -- Adapted from https://www.youtube.com/watch?v=EthjeNeNTsM AlwaysVisible shader
		Pass
		{
			Tags { "Queue"="Transparent" }
			LOD 100

			Cull Back
			ZWrite Off
			ZTest Always 
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _SilhouetteColor;
            }


            ENDCG
		}

		//Outline
		Pass{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			CGPROGRAM

			struct v2f {
				float4 pos : SV_POSITION;
			};

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v) {
				appdata original = v;

				float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));
				//This shader consists of 2 ways of generating outline that are dynamically switched based on demiliter angle
				//If vertex normal is pointed away from object origin then custom outline generation is used (based on scaling along the origin-vertex vector)
				//Otherwise the old-school normal vector scaling is used
				//This way prevents weird artifacts from being created when using either of the methods
				if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _Angle) {
					v.vertex.xyz += normalize(v.normal.xyz) * _OutlineWidth;
				}else {
					v.vertex.xyz += scaleDir * _OutlineWidth;
				}

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR{
				return _OutlineColor;
			}

			ENDCG
		}
		
		//Surface shader
		Tags{ "Queue" = "Transparent" }

		CGPROGRAM
		#pragma surface surf Lambert alpha

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
