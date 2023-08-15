Shader "Custom/neo_planet" {
    Subshader {  
        Pass {
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
            
            float4 vert(uint id:SV_VertexID, out float3 p:TEXCOORD0, out float3 n:TEXCOORD1) : SV_POSITION {
                float f = id;
                float v = f - 6.0 * floor(f/6.0);
                f = (f - v) / 6.;
                float a = f - 64.0 * floor(f/64.0);
                f = (f-a) / 64.;
                float b = f-16.;
                a += (v - 2.0 * floor(v/2.0));
                b += v==2. || v>=4. ? 1.0 : 0.0;
                a = a/64.*6.28318;
                b = b/64.*6.28318;
                p = float3(cos(a)*cos(b), sin(b), sin(a)*cos(b));
                n = normalize(p);
                p += float3(50.0, 0.0, 0.0);
                return UnityObjectToClipPos(float4(p, 1.0));
            }
    
            float4 frag(float4 s:SV_POSITION, float3 p:TEXCOORD0, float3 n:TEXCOORD1) : SV_Target {  
                return float4(max(dot(normalize(_WorldSpaceLightPos0).xyz, n),0.0).xxx + UNITY_LIGHTMODEL_AMBIENT, 1.0);
            }
            ENDCG
        }
        
    }
}
