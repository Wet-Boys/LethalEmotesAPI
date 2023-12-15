Shader "Unlit/WheelSegment"
{
    Properties
    {
        _MainColor ("Main Color (RGBA)", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                
                #include "UnityCG.cginc"
                
                struct appdata {
                    float4 vert: POSITION;
                };
                
                struct v2f {
                    float4 vert: SV_POSITION;
                };
                
                float4 _MainColor;
                
                v2f vert (appdata v) {
                    v2f o;
                    o.vert = UnityObjectToClipPos(v.vert); 
                    return o;
                }
                
                fixed4 frag(v2f i) : COLOR {
                    return _MainColor;
                }
            ENDCG
        }
        
//         Pass {
//         
//             CGPROGRAM
//                 #pragma vertex vert
//                 #pragma fragment frag
//                                 
//                 #include "UnityCG.cginc"
//             
//                 float _Border;
//                 float4 _BorderColor;
//             
//                 struct appdata {
//                     float4 vert: POSITION;
//                     float3 normal: NORMAL;
//                 };
//                 
//                 struct v2f {
//                     float4 vert: SV_POSITION;
//                 };
//                 
//                 v2f vert(appdata v) {
//                     float4 pos = v.vert;
//                     
//                     pos.xyz = v.normal * _Border;
//                     
//                     v2f o;
//                     o.vert = UnityObjectToClipPos(pos);
//                     return o;
//                 }
//                 
//                 fixed4 frag(v2f i) : SV_Target {
//                     return _BorderColor;
//                 }
//             ENDCG
//         }
    }
}
