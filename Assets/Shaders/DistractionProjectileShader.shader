Shader "Custom/DistractionProjectileShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Displacement ("Displacement Amount", Range(-1, 1)) = 0.1
        _NoiseScale ("Noise Scale", Float) = 1.0
        _TimeSpeed ("Time Speed", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Displacement;
            float _NoiseScale;
            float _TimeSpeed;

            // Simple 3D noise function
            float Noise(float3 p)
            {
                return sin(p.x * 10.0 + p.y * 10.0 + p.z * 10.0);
            }

            v2f vert(appdata v)
            {
                v2f o;

                // Add time-based displacement
                float time = _Time * _TimeSpeed;
                float noise = (Noise(v.vertex.xyz * _NoiseScale + time) * 0.5 + 0.5);
                // Adding time for dynamic displacement
                v.vertex.xyz += v.normal * _Displacement * noise;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Pass the UVs for texture sampling
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor; // Return the texture color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}