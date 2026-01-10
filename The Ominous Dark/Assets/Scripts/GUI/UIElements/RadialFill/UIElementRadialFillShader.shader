Shader "UI/UIElementRadialFill"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0,1)) = 1
        _StartAngle ("Start Angle", Range(0,360)) = 0
        _Clockwise ("Clockwise", Range(-1,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _FillAmount;
            float _StartAngle;
            float _Clockwise;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // center w UV
                float2 center = float2(0.5, 0.5);
                float2 dir = i.uv - center;

                // kąt od środka
                float angle = atan2(dir.y, dir.x); // [-pi, pi]
                angle = angle < 0 ? angle + 2*3.1415926 : angle; // [0,2pi]

                float start = radians(_StartAngle);
                float fill = _FillAmount * 6.2831853; // 2*pi*FillAmount

                if (_Clockwise < 0)
                {
                    // dla przeciwnie
                    angle = 6.2831853 - angle;
                }

                float diff = angle - start;
                if (diff < 0) diff += 6.2831853;

                fixed4 col = tex2D(_MainTex, i.uv);
                if (diff > fill) col.a = 0; // poza fill → przezroczystość
                return col;
            }
            ENDCG
        }
    }
}