Shader "Unlit/HealthEmoter"
{
    Properties
    {
		_HealthyColor ("Healthy Color", Color) = (0, 0, 1, 1)
		_HurtColor ("Hurt Color", Color) = (1, 0, 0, 1)
		_HealthFill ("Health Fill", Range(0.0, 1.0)) = 0.5
		_RemapMin ("Remap Min", Range(-4.0, 4.0)) = 1.7
		_RemapMax ("Remap Max", Range(-4.0, 4.0)) = -0.65
    }
    SubShader
    {
        Tags {
			"RenderType"="Opaque"
		}
		Cull Off
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float fillEdge : TEXCOORD1;
            };

            float4 _HealthyColor;
            float4 _HurtColor;
            float _HealthFill;
            float _RemapMin;
            float _RemapMax;
			
			float remap(float fromMin, float fromMax, float toMin, float toMax, float value) {
				float rel = (value - fromMin) / (fromMax - fromMin);
				return lerp(toMin, toMax, rel);
			}
			
			float4 rotateAroundYDegrees(float4 vertex, float degrees) {
				float alpha = degrees * UNITY_PI / 180;
				float sinA, cosA;
				sincos(alpha, sinA, cosA);
				float2x2 m = float2x2(cosA, sinA, -sinA, cosA);
				return float4(vertex.yz, mul(m, vertex.xz)).xzyw;
			}

            v2f vert (appdata v)
            {
                v2f o;
				// o.objectSpaceVertex = v.vertex;
				
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);   
				// rotate it around XY
				float3 worldPosX = rotateAroundYDegrees(float4(worldPos, 0), 360);
				// rotate around XZ
				float3 worldPosZ = float3 (worldPosX.y, worldPosX.z, worldPosX.x);        
				// combine rotations with worldPos, based on sine wave from script
				float3 worldPosAdjusted = worldPos;
				
				float vertHeight = remap(0, 1, _RemapMin, _RemapMax, _HealthFill);
				o.fillEdge = worldPosAdjusted.y + vertHeight;
				
				// float4 unwrappedVertex = mul(mul(UNITY_MATRIX_M, UNITY_MATRIX_P), float4(v.uv.x, v.uv.y, 0.0, 1.0));
				
                // o.vertex = unwrappedVertex;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				// float vertHeight = remap(_RemapMin, _RemapMax, 0, 1, i.objectSpaceVertex.g);
				float hurt = step(i.fillEdge, 0.5);
				float healthy = 1.0 - hurt;
			
                fixed4 col = (_HealthyColor * healthy) + (_HurtColor * hurt);
                return col;
            }
            ENDHLSL
        }
    }
}
