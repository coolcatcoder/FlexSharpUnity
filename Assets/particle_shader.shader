Shader "particle_shader"
{
	Properties
	{

	}

	SubShader
	{
		Tags { "RenderType" = "AlphaTest" "DisableBatching" = "True" "RenderPipeline" = "UniversalPipeline" }
		LOD 100

		Pass
		{
			//Tags { "LightMode" = "ForwardBase"}
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 rayDir : TEXCOORD0;
				float3 rayOrigin : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;

				// get world position of vertex
				// using float4(v.vertex.xyz, 1.0) instead of v.vertex to match Unity's code
				float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));

				// calculate and world space ray direction and origin for interpolation
				o.rayDir = worldPos - _WorldSpaceCameraPos.xyz;
				o.rayOrigin = _WorldSpaceCameraPos.xyz;

				o.pos = UnityWorldToClipPos(worldPos);

				return o;
			}

			float sphIntersect(float3 ro, float3 rd, float4 sph)
			{
				float3 oc = ro - sph.xyz;
				float b = dot(oc, rd);
				float c = dot(oc, oc) - sph.w * sph.w;
				float h = b * b - c;
				if (h < 0.0) return -1.0;
				h = sqrt(h);
				return -b - h;
			}

			half3 _LightColor0;

			half4 frag(v2f i, out float outDepth : SV_Depth) : SV_Target
			{
				// ray origin
				float3 rayOrigin = i.rayOrigin;

				// normalize ray vector
				float3 rayDir = normalize(i.rayDir);

				// sphere position
				float3 spherePos = unity_ObjectToWorld._m03_m13_m23;

				// ray box intersection
				float rayHit = sphIntersect(rayOrigin, rayDir, float4(spherePos, 0.5));

				// above function returns -1 if there's no intersection
				clip(rayHit);

				// calculate world space position from ray, front hit ray length, and ray origin
				float3 worldPos = rayDir * rayHit + rayOrigin;

				// world space surface normal
				float3 worldNormal = normalize(worldPos - spherePos);

				// basic lighting
				half3 worldLightDir = _WorldSpaceLightPos0.xyz;
				half ndotl = saturate(dot(worldNormal, worldLightDir));
				half3 lighting = _LightColor0 * ndotl;

				// ambient lighting
				half3 ambient = ShadeSH9(float4(worldNormal, 1));
				lighting += ambient;

				// output modified depth
				float4 clipPos = UnityWorldToClipPos(worldPos);
				outDepth = clipPos.z / clipPos.w;

				return half4(lighting, 1.0);
			}
			ENDHLSL
		}
	}
}
