Shader "Block/BlockShader Invert"
{
	Properties
	{
		_MainTex ("Atlas (red, red, green)", 2D) = "white" {}
		_ShineEffect ("Shine", 2D) = "white"{}
		_ShineSpeed ("ShineSpeed", float) = 1
	}
	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0.1
		ZWrite Off
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				fixed4 col : COLOR;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 col : COLOR;
			};

			sampler2D _MainTex, _ShineEffect;
			float4 _MainTex_ST, _ShineEffect_ST, _GameTime;
			float _ShineSpeed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _ShineEffect) + half2(_Time.y*_ShineSpeed, 0);
				o.col = v.col;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv.xy);
			    fixed4 shine = tex2D(_ShineEffect, i.uv.zw);
				float3 mainCol = col.ggg*i.col;
				float3 shineCol = (1-mainCol)*shine;

				
				shineCol = shineCol*(col.r);
				mainCol = (mainCol * (1- (shine.r*col.r)));


				return fixed4(mainCol+shineCol, col.a);
			}
			ENDCG
		}
	}
}
