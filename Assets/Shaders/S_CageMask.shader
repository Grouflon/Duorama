Shader "Unlit/CageMask"
{
    Properties
    {
        _height("Height", float) = 1
        _edgeWidth("EdgeWidth", float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" } 
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _height;
            float _edgeWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 faceUV : TEXCOORD0;
                float2 surfaceUV : TEXCOORD1;
            };

            struct v2f
            {
                float2 faceUV : TEXCOORD0;
                float2 surfaceUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.faceUV = v.faceUV;
                o.surfaceUV = v.surfaceUV;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 size = float2(1, 1);
                float3 edgeColor = float3(1, 1, 1);
                float3 faceColor = float3(1, 1, 1);
                if (i.faceUV.y >= 0.1 && i.faceUV.y <= 0.9)
                {
                    size = float2(1, _height);
                }

                if (i.faceUV.x < 0.4)
                {
                    faceColor = float3(1, 0, 0);
                }
                else if (i.faceUV.x > 0.6)
                {
                    faceColor = float3(0, 1, 0);
                }

                float threshold = _edgeWidth * 0.1;
                if (
                    i.surfaceUV.y < threshold / size.x ||
                    i.surfaceUV.y > 1.0 - (threshold / size.x) ||
                    i.surfaceUV.x < threshold / size.y ||
                    i.surfaceUV.x > 1.0 - (threshold / size.y)
                    )
                {
                    edgeColor = float3(0,0,0);
                }

                return float4(edgeColor.xyz * faceColor.xyz, 1.f);
            }
            ENDCG
        }
    }
}
