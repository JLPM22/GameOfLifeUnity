Shader "Custom/GameOfLifeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Count2Color ("Count 2 Color", COLOR) = (1,1,1,1)
        _Count3AliveColor ("Count 3 Alive Color", COLOR) = (1,1,1,1)
        _Count3DeadColor ("Count 3 Dead Color", COLOR) = (0,0,0,0)
        _DeadColor("Dead Color", COLOR) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            uniform fixed4 _Count2Color;
            uniform fixed4 _Count3AliveColor;
            uniform fixed4 _Count3DeadColor;
            uniform fixed4 _DeadColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 aliveColor = _Count2Color * col.r +
                                    _Count3AliveColor * col.g +
                                    _Count3DeadColor * col.b;
                fixed4 finalColor = aliveColor * col.a + _DeadColor * (1 - col.a);
                finalColor.a = 1;
                return finalColor;
            }
            ENDCG
        }
    }
}
