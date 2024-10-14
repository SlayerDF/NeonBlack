Shader "Custom/ShadowZoneShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _BlurSize ("Blur Size", Range(0, 5)) = 1.0
        _NoiseStrength ("Noise Strength", Range(0.0, 1.0)) = 0.1
        _NoiseScale ("Noise Scale", Range(1.0, 100.0)) = 10.0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        Cull Off

        GrabPass {}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _BlurSize;

            half4 frag(v2f i) : SV_Target
            {
                half4 color = 0;

                #define ADDPIXEL(weight,kernelX) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uv.x + _GrabTexture_TexelSize.x * kernelX * _BlurSize, i.uv.y, i.uv.z, i.uv.w))) * weight

                color += ADDPIXEL(0.05, 4.0);
                color += ADDPIXEL(0.09, 3.0);
                color += ADDPIXEL(0.12, 2.0);
                color += ADDPIXEL(0.15, 1.0);
                color += ADDPIXEL(0.18, 0.0);
                color += ADDPIXEL(0.15, -1.0);
                color += ADDPIXEL(0.12, -2.0);
                color += ADDPIXEL(0.09, -3.0);
                color += ADDPIXEL(0.05, -4.0);

                return color;
            }
            ENDCG
        }

        GrabPass {}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _BlurSize;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = 0;

                #define ADDPIXEL(weight,kernelY) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uv.x, i.uv.y + _GrabTexture_TexelSize.y * kernelY * _BlurSize, i.uv.z, i.uv.w))) * weight

                color += ADDPIXEL(0.05, 4.0);
                color += ADDPIXEL(0.09, 3.0);
                color += ADDPIXEL(0.12, 2.0);
                color += ADDPIXEL(0.15, 1.0);
                color += ADDPIXEL(0.18, 0.0);
                color += ADDPIXEL(0.15, -1.0);
                color += ADDPIXEL(0.12, -2.0);
                color += ADDPIXEL(0.09, -3.0);
                color += ADDPIXEL(0.05, -4.0);

                return color;
            }
            ENDCG
        }

        GrabPass {}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
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
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _GrabTexture;
            fixed4 _Color;
            float _NoiseStrength;
            float _NoiseScale;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uv));

                // Apply noise
                float noiseValue = frac(sin(dot((i.uv * _NoiseScale).xy, float2(12.9898, 78.233))) * 43758.5453);
                color.rgb += noiseValue * _NoiseStrength;

                // Apply color
                color.rgb = lerp(color.rgb, color.rgb * _Color.rgb, _Color.a);

                return color;
            }
            ENDCG
        }
    }
}