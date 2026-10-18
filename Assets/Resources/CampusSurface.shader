Shader "BinaryLift/Campus Surface"
{
    Properties {
        _Color ("Colour", Color) = (0.3,0.2,0.12,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.25
        _Grain ("Wood grain (0 is paving)", Range(0,1)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        fixed4 _Color; half _Glossiness, _Grain;
        struct Input { float3 worldPos; float3 worldNormal; };
        void surf(Input IN, inout SurfaceOutputStandard o) {
            float3 p=IN.worldPos;
            float grain=0.84+0.16*sin(p.x*65+sin(p.y*2+p.z*3)*3);
            float2 tile=frac(p.xz*1.3);
            float seam=step(.035,tile.x)*step(.035,tile.y);
            float variation=0.90+0.10*sin(floor(p.x*1.3)*3+floor(p.z*1.3)*7);
            o.Albedo=_Color.rgb*lerp(lerp(.42,variation,seam),grain,_Grain);
            o.Smoothness=_Glossiness*lerp(seam,1,_Grain);
            o.Metallic=0; o.Alpha=1;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
