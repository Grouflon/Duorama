Shader "Unlit/S_Compositing"
{
    Properties
    {
        _maskTexture("MaskTexture", 2D) = "white" {}
        _room1Texture("Room1Texture", 2D) = "white" {}
        _room2Texture("Room2Texture", 2D) = "white" {}
        _backgroundColor("BackgroundColor", Color) = (0,0,0,1)
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

            sampler2D _maskTexture;
            sampler2D _room1Texture;
            sampler2D _room2Texture;
            float4 _backgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 maskCol = tex2D(_maskTexture, i.uv);
                float4 room1Col = tex2D(_room1Texture, i.uv);
                float4 room2Col = tex2D(_room2Texture, i.uv);

                fixed background = 1.0 - max(maskCol.x, maskCol.y);
                float4 col = room1Col * maskCol.x + room2Col * maskCol.y;
                col.w = 1.f - maskCol.z;
                return col;
            }
            ENDCG
        }
    }
}
