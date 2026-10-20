Shader "BinaryLift/World Sandstone"
{
    Properties { _MainTex ("Masonry", 2D) = "white" {} _Color ("Tint", Color) = (1,1,1,1) }
    SubShader {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 3.0
        sampler2D _MainTex; fixed4 _Color;
        struct Input { float3 worldPos; float3 worldNormal; };
        void surf(Input IN, inout SurfaceOutput o) {
            float3 n=abs(IN.worldNormal);
            float2 uv=n.y>max(n.x,n.z)?IN.worldPos.xz:(n.x>n.z?IN.worldPos.zy:IN.worldPos.xy);
            o.Albedo=tex2D(_MainTex,uv/3.0).rgb*_Color.rgb; o.Alpha=1;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
