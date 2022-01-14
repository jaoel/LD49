Shader "Custom/TheVoid"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Background("Background Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader{
    Pass {

        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata_t {
            float4 vertex : POSITION;
            fixed4 color : COLOR;
            float4 texcoords : TEXCOORD0;
            float texcoordBlend : TEXCOORD1;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            float4 screenPos : TEXCOORD1;
            fixed4 color : COLOR;
        };

        sampler2D _MainTex;
        sampler2D _Background;
        float4 _Color;
        
        static const float ShadowStrength = 0.1;

        v2f vert(appdata_t v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex + float4(0, sin(v.vertex.y * 10 + v.vertex.x * 10 + _Time.x * 50) * 0.1, 0, 0));
            o.texcoord = v.texcoords.xy;
            o.screenPos = ComputeScreenPos(o.pos / o.pos.w);
            o.color = v.color;

            return o;
        }

        fixed4 frag(v2f i) : COLOR{
            float2 spriteUV = i.texcoord;
            float2 backgroundUV = (i.pos.xy / _ScreenParams.xy) - 0.5;
            backgroundUV.x *= _ScreenParams.x / _ScreenParams.y;
            backgroundUV = frac(backgroundUV.xy * 4) + float2(_Time.x, _Time.x * 1.25);

            float4 texColor = tex2D(_MainTex, spriteUV);
            float4 backgroundColor = tex2D(_Background, backgroundUV);

            float edge = texColor.a * i.color.a - 0.1;

            clip(edge);

            // Cant do this, we need a render target
            /*float4 edgeColor = float4(1, 1, 1, 1) * (1 - edge);
            edgeColor = step(0.99, edgeColor);*/

            return backgroundColor/* + edgeColor*/;

        }

        ENDCG
    }
    }
}