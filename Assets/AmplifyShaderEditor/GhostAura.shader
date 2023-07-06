// Upgrade NOTE: upgraded instancing buffer 'NewAmplifyShader' to new syntax.

// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_Color0("Color 0", Color) = (0.1381275,0.2119534,0.3018868,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha One
		
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		UNITY_INSTANCING_BUFFER_START(NewAmplifyShader)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color0)
#define _Color0_arr NewAmplifyShader
		UNITY_INSTANCING_BUFFER_END(NewAmplifyShader)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Color0_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color0_arr, _Color0);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV2 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode2 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV2, 1.2 ) );
			float4 temp_output_7_0 = ( float4( 0,0,0,0 ) + ( _Color0_Instance * fresnelNode2 ) );
			o.Albedo = temp_output_7_0.rgb;
			o.Emission = temp_output_7_0.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-57.0822,31.85985;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;New Amplify Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;5;True;False;0;True;TransparentCutout;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;49;False;;232;False;;142;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;5;False;8;5;False;;1;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FresnelNode;2;-516.761,335.6539;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-650.9576,128.4045;Inherit;False;InstancedProperty;_Color0;Color 0;1;0;Create;True;0;0;0;False;0;False;0.1381275,0.2119534,0.3018868,0;0.2863564,0.4235382,0.5188679,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-381.8949,175.8053;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-526.1614,-81.98113;Inherit;False;Constant;_Color1;Color 1;2;0;Create;True;0;0;0;False;0;False;0.6792453,0.1769909,0.1313635,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-258.5308,83.71194;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-722.8921,450.6449;Inherit;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;0;False;0;False;1.2;0;0;0;0;1;FLOAT;0
WireConnection;0;0;7;0
WireConnection;0;2;7;0
WireConnection;2;3;3;0
WireConnection;4;0;1;0
WireConnection;4;1;2;0
WireConnection;7;1;4;0
ASEEND*/
//CHKSM=6D135F80611B9D9EC7A4300B3D7BFF4480B9D921