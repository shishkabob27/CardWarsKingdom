Shader "KFF/Unlit_Desaturate"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
        _EffectAmount ("Effect Amount", Range(0,1)) = 1
        _AdditiveAmount ("Additive Amount", Range(0,1)) = 0
        _Color ("Color Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        LOD 200
        Pass
        {
            Tags { "LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform float4 _Color;
            uniform float _EffectAmount;
            uniform float _AdditiveAmount;
            uniform sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float gray = dot(col.rgb, float3(0.3, 0.59, 0.11));
                float3 desaturatedColor = lerp(col.rgb, float3(gray, gray, gray), _EffectAmount);
                float3 finalColor = lerp(desaturatedColor, _Color.rgb, _AdditiveAmount);

                return fixed4(finalColor, col.a);
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
