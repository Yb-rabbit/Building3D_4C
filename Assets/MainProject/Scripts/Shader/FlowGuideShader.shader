Shader "Custom/FlowGuideShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,0,1) // 默认黄色
        _Speed ("Flow Speed", Float) = 2.0
        _Tiling ("Tiling", Float) = 5.0 // 虚线密度
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;
            float _Tiling;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // 重点：这里使用了世界坐标的X和Z来计算UV，这样线在空间中就是连续的
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = float2(worldPos.x + worldPos.z, 0); 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 创建流动的UV偏移
                float2 flowUV = i.uv * _Tiling;
                flowUV.x -= _Time.y * _Speed; // 随时间流动

                // 简单的条纹效果
                float stripe = sin(flowUV.x * 3.14159);
                stripe = step(0, stripe); // 将正弦波变成黑白方块

                // 混合颜色
                fixed4 col = _Color * stripe;
                return col;
            }
            ENDCG
        }
    }
}
