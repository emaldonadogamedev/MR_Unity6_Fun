// 7/1/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

Shader "Custom/VotingChoiceCenterPreview"
{
    Properties
    {
        _Color ("Base Color", Color) = (0, 1, 0, 0.5) // Default color is semi-transparent green
        _EdgeColor ("Edge Highlight Color", Color) = (1, 1, 1, 1) // Default edge color is white
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.2 // Controls the width of the edge highlight
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            // Enable transparency blending
            Blend SrcAlpha OneMinusSrcAlpha
            // Disable depth writing to avoid sorting issues with transparency
            ZWrite Off
            // Disable backface culling so both sides of the object are visible
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Vertex input structure
            struct appdata
            {
                float4 vertex : POSITION; // Vertex position
                float3 normal : NORMAL;  // Vertex normal
            };

            // Vertex-to-fragment structure
            struct v2f
            {
                float4 pos : SV_POSITION; // Screen-space position
                float3 worldNormal : TEXCOORD0; // World-space normal
                float3 worldPos : TEXCOORD1; // World-space position
            };

            // Shader properties
            fixed4 _Color; // Base color (green or red)
            fixed4 _EdgeColor; // Edge highlight color
            float _EdgeWidth; // Edge width for the Fresnel effect

            // Vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                // Transform the vertex position to clip space
                o.pos = UnityObjectToClipPos(v.vertex);
                // Transform the normal to world space
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                // Transform the vertex position to world space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the Fresnel effect for edge highlighting
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos); // Direction from the camera to the fragment
                float fresnel = pow(1.0 - dot(viewDir, i.worldNormal), _EdgeWidth); // Fresnel term

                // Combine the base color and the edge highlight color
                fixed4 baseColor = _Color; // Base color (green or red)
                fixed4 edgeColor = _EdgeColor * fresnel; // Edge highlight color modulated by the Fresnel effect

                // Final color is a combination of the base color and the edge highlight
                return baseColor + edgeColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}