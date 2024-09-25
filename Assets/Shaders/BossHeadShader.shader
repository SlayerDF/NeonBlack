Shader "Custom/BossHeadVFX"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)

        // Mask
        _MaskCol ("Mask Color", Color) = (1.0, 0.0, 0.0, 1.0)
        _MaskSensitivity ("Mask Threshold Sensitivity", Range(0,1)) = 0.5
        _MaskSmooth ("Mask Smoothing", Range(0,1)) = 0.5

        // Overlay
        _OverlayTex ("Overlay Texture", 2D) = "white" {}
        _OverlayColor ("Overlay Color", Color) = (1, 1, 1, 0.5)
        _OverlayMoveSpeed ("Overlay Move Speed", Range(0, 10)) = 1
        _OverlayOffset ("Overlay Offset", Range(0, 1)) = 0.3
        _OverlayRotation ("Overlay Rotation", Range(0, 360)) = 45
        _OverlayRandomFactor ("Overlay Random Factor", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="true"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha Cull Off
        LOD 200

        // Transparency fix
        Pass
        {
            ZWrite On
            ColorMask 0
        }

        // Mask
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                half2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Speed;

            float4 _MaskCol;
            float _MaskSensitivity;
            float _MaskSmooth;


            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                return OUT;
            }

            float4 frag(v2f i) : COLOR
            {
                float2 uv = i.texcoord.xy;
                float4 c = tex2D(_MainTex, uv);

                float maskY = 0.2989 * _MaskCol.r + 0.5866 * _MaskCol.g + 0.1145 * _MaskCol.b;
                float maskCr = 0.7132 * (_MaskCol.r - maskY);
                float maskCb = 0.5647 * (_MaskCol.b - maskY);

                float Y = 0.2989 * c.r + 0.5866 * c.g + 0.1145 * c.b;
                float Cr = 0.7132 * (c.r - Y);
                float Cb = 0.5647 * (c.b - Y);

                float blendValue = smoothstep(_MaskSensitivity, _MaskSensitivity + _MaskSmooth,
                                              distance(float2(Cr, Cb), float2(maskCr, maskCb)));

                return float4(c.rgb, c.a * blendValue);
            }
            ENDCG
        }

        // Overlay
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _OverlayTex;
            float4 _Color;
            float4 _OverlayColor;
            float _OverlayMoveSpeed;
            float _OverlayOffset;
            float _OverlayRotation;
            float _OverlayRandomFactor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoordMain : TEXCOORD0;
                float2 texcoordOverlay : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoordMain = IN.uv;
                OUT.texcoordOverlay = IN.uv;
                return OUT;
            }

            float2 RotateUV(float2 uv, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                uv -= 0.5; // смещаем координаты в центр
                float2 rotatedUV = float2(c * uv.x - s * uv.y, s * uv.x + c * uv.y);
                return rotatedUV + 0.5;
            }

            float EdgeBlend(float2 uv)
            {
                float blendU = smoothstep(0.0, 0.1, uv.x) * smoothstep(0.0, 0.1, 1.0 - uv.x);
                float blendV = smoothstep(0.0, 0.1, uv.y) * smoothstep(0.0, 0.1, 1.0 - uv.y);
                return blendU * blendV;
            }

            float2 RandomizeUV(float2 uv, float randomFactor)
            {
                uv += randomFactor * sin(_Time.y + uv.x * 10.0) * 0.1;
                return uv;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.texcoordMain);

                float2 overlayUV = i.texcoordOverlay;

                overlayUV += _OverlayOffset;
                overlayUV = RotateUV(overlayUV, radians(_OverlayRotation));

                overlayUV = RandomizeUV(overlayUV, _OverlayRandomFactor);

                overlayUV.y += _Time.y * _OverlayMoveSpeed;
                overlayUV = frac(overlayUV);

                fixed4 overlayColor = tex2D(_OverlayTex, overlayUV) * _OverlayColor;

                float edgeBlend = EdgeBlend(overlayUV);
                overlayColor *= edgeBlend;

                return baseColor * _Color.a + overlayColor * overlayColor.a;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}