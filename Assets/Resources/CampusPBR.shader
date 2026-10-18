Shader "BinaryLift/Campus PBR"
{
    Properties {
        _Color ("Tint",Color)=(1,1,1,1)
        _MainTex ("Base colour",2D)="white" {}
        _BumpMap ("Normal",2D)="bump" {}
        _Roughness ("Roughness",2D)="white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        sampler2D _MainTex,_BumpMap,_Roughness;fixed4 _Color;
        struct Input {float2 uv_MainTex;};
        void surf(Input i,inout SurfaceOutputStandard o) {
            o.Albedo=tex2D(_MainTex,i.uv_MainTex).rgb*_Color.rgb;
            o.Normal=UnpackNormal(tex2D(_BumpMap,i.uv_MainTex));
            o.Smoothness=(1-tex2D(_Roughness,i.uv_MainTex).r)*.6;
            o.Metallic=0;o.Alpha=1;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
