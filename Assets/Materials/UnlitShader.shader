// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnlitShadows"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader{
    Pass {

        // 1.) This will be the base forward rendering pass in which ambient, vertex, and
        // main directional light will be applied. Additional lights will need additional passes
        // using the "ForwardAdd" lightmode.
        // see: http://docs.unity3d.com/Manual/SL-PassTags.html
        Tags { "LightMode" = "ForwardBase" }

        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        // 2.) This matches the "forward base" of the LightMode tag to ensure the shader compiles
        // properly for the forward bass pass. As with the LightMode tag, for any additional lights
        // this would be changed from _fwdbase to _fwdadd.
        #pragma multi_compile_fwdbase

        // 3.) Reference the Unity library that includes all the lighting shadow macros
        #include "AutoLight.cginc"

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 texcoord : TEXCOORD0;

            // 4.) The LIGHTING_COORDS macro (defined in AutoLight.cginc) defines the parameters needed to sample 
            // the shadow map. The (0,1) specifies which unused TEXCOORD semantics to hold the sampled values - 
            // As I'm not using any texcoords in this shader, I can use TEXCOORD0 and TEXCOORD1 for the shadow 
            // sampling. If I was already using TEXCOORD for UV coordinates, say, I could specify
            // LIGHTING_COORDS(1,2) instead to use TEXCOORD1 and TEXCOORD2.
            LIGHTING_COORDS(1,2)
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _Color;
        
        static const float ShadowStrength = 0.1;

        v2f vert(appdata_base v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.texcoord = v.texcoord.xy;

            // 5.) The TRANSFER_VERTEX_TO_FRAGMENT macro populates the chosen LIGHTING_COORDS in the v2f structure
            // with appropriate values to sample from the shadow/lighting map
            TRANSFER_VERTEX_TO_FRAGMENT(o);

            return o;
        }

        fixed4 frag(v2f i) : COLOR {

            // 6.) The LIGHT_ATTENUATION samples the shadowmap (using the coordinates calculated by TRANSFER_VERTEX_TO_FRAGMENT
            // and stored in the structure defined by LIGHTING_COORDS), and returns the value as a float.
            float attenuation = LIGHT_ATTENUATION(i);
            attenuation = lerp(1, step(0.75, attenuation), ShadowStrength);

            float4 texColor = tex2D(_MainTex, i.texcoord);

            return texColor * _Color * attenuation;
        }

        ENDCG
    }
    }

    // 7.) To receive or cast a shadow, shaders must implement the appropriate "Shadow Collector" or "Shadow Caster" pass.
    // Although we haven't explicitly done so in this shader, if these passes are missing they will be read from a fallback
    // shader instead, so specify one here to import the collector/caster passes used in that fallback.
    Fallback "VertexLit"
}