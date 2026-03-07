Shader "Custom/2D_SDF_Fake3D"
{
    Properties
    {
        _Shape ("Shape (0=Circle,1=Square,2=Triangle)", Float) = 0
        _Color ("Base Color", Color) = (1,1,1,1)
        _LightDir ("Light Direction", Vector) = (0.3,0.6,0.7,0)
        _Rotation ("Rotation XYZ (degrees)", Vector) = (0,0,0,0)
        _Size ("Size", Float) = 0.4
        _Softness ("Edge Softness", Float) = 0.01
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv       : TEXCOORD0;
            };

            float _Shape;
            float4 _Color;
            float4 _LightDir;
            float4 _Rotation;
            float _Size;
            float _Softness;

            float3x3 RotationMatrix(float3 r)
            {
                r = radians(r);

                float cx = cos(r.x);
                float sx = sin(r.x);
                float cy = cos(r.y);
                float sy = sin(r.y);
                float cz = cos(r.z);
                float sz = sin(r.z);

                float3x3 rotX = float3x3(
                    1,0,0,
                    0,cx,-sx,
                    0,sx,cx
                );

                float3x3 rotY = float3x3(
                    cy,0,sy,
                    0,1,0,
                    -sy,0,cy
                );

                float3x3 rotZ = float3x3(
                    cz,-sz,0,
                    sz,cz,0,
                    0,0,1
                );

                return mul(rotZ, mul(rotY, rotX));
            }

            // ---- SDF FUNCTIONS ----

            float sdCircle(float2 p, float r)
            {
                return length(p) - r;
            }

            float sdBox(float2 p, float2 b)
            {
                float2 d = abs(p) - b;
                return length(max(d,0)) + min(max(d.x,d.y),0);
            }

            float sdTriangle(float2 p)
            {
                const float k = sqrt(3.0);
                p.x = abs(p.x) - 1.0;
                p.y = p.y + 1.0/k;
                if( p.x + k*p.y > 0.0 )
                    p = float2(p.x - k*p.y, -k*p.x - p.y)/2.0;
                p.x -= clamp(p.x, -2.0, 0.0);
                return -length(p)*sign(p.y);
            }

            float map(float2 p)
            {
                if (_Shape < 0.5)
                    return sdCircle(p, _Size);

                else if (_Shape < 1.5)
                    return sdBox(p, float2(_Size,_Size));

                else
                    return sdTriangle(p/_Size)*_Size;
            }

            // Approximate gradient
            float3 GetNormal(float2 p)
            {
                float e = 0.001;

                float d  = map(p);
                float dx = map(p + float2(e,0));
                float dy = map(p + float2(0,e));

                float2 grad = float2(dx - d, dy - d);

                return normalize(float3(grad, -1.0));
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.position = TransformObjectToHClip(v.vertex);
                o.uv = v.uv * 2.0 - 1.0;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 p = float3(i.uv, 0);

                float3x3 rot = RotationMatrix(_Rotation.xyz);
                p = mul(rot, p);

                float d = map(p.xy);

                float alpha = smoothstep(_Softness, 0, d);

                if (alpha <= 0) discard;

                float3 normal = GetNormal(p.xy);
                normal = mul(rot, normal);

                float3 lightDir = normalize(_LightDir.xyz);

                float NdotL = saturate(dot(normal, lightDir));

                float3 color = _Color.rgb * (0.2 + 0.8 * NdotL);

                return float4(color, alpha);
            }

            ENDHLSL
        }
    }
}