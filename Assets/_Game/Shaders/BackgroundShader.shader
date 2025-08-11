// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BackgroundShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Grid("GridTexture", 2D) = "white"{}
		_TileX("Tile X", float) = 1
		_TileY("Tile X", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 col : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 uvGrid : TEXCOORD1;
				float3 worldPos: TEXCOORD2;
				fixed4 col : COLOR;
			};

			sampler2D _MainTex, _Grid;
			float4 _MainTex_ST, _Grid_ST;
			float _TileX, _TileY;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvGrid = TRANSFORM_TEX(float2(o.worldPos.x*_TileX, o.worldPos.y), _Grid);
				o.col = v.col;


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 grid = tex2D(_Grid,i.uvGrid);
				return fixed4 (col.rgb+((grid.rgb*grid.a)*col.a), 1*i.col.a);
			}
			ENDCG
		}
	}
}
