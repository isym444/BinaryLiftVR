Shader "BinaryLift/CalfCoat"
{
    Properties { _Glossiness ("Smoothness", Range(0,1)) = 0.07 }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        struct Input { float4 coatColor; };
        half _Glossiness;
        void vert(inout appdata_full v, out Input o) { UNITY_INITIALIZE_OUTPUT(Input,o); o.coatColor=v.color; }
        void surf(Input IN, inout SurfaceOutputStandard o) { o.Albedo=IN.coatColor.rgb; o.Metallic=0; o.Smoothness=_Glossiness; o.Alpha=1; }
        ENDCG
    }
    FallBack "Diffuse"
}
