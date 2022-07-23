float4x4 WorldMatrix;
float4x4 ViewMatrix;
float4x4 ProjectionMatrix;

float4 AmbienceColor = float4(0.1f, 0.1f, 0.1f, 1.0f);

// For Diffuse Lightning
float4x4 WorldInverseTransposeMatrix;
float3 DiffuseLightDirection = float3(-1.0f, 0.0f, 0.0f);
float4 DiffuseColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

// For Texture
texture ModelTexture;
sampler2D TextureSampler = sampler_state {
	Texture = (ModelTexture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

// For Specular Lighting
float ShininessFactor = 10.0f;
float4 SpecularColor = float4(1.0f, 1.0f, 1.0f, 1.0f);
float3 ViewVector = float3(1.0f, 0.0f, 0.0f);

// For Reflection Lighting
Texture EnvironmentTexture;
samplerCUBE EnvironmentSampler = sampler_state
{
	texture = <EnvironmentTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	// For Diffuse Lightning
	float4 NormalVector : NORMAL0;
	// For Texture
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	// For Diffuse Lightning
	float4 VertexColor : COLOR0;
	// For Texture    
	float2 TextureCoordinate : TEXCOORD0;
	// For Specular Shading  
	float3 NormalVector : TEXCOORD1;
	// For Reflection
	float3 ReflectionVector : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, WorldMatrix);
	float4 viewPosition = mul(worldPosition, ViewMatrix);
	output.Position = mul(viewPosition, ProjectionMatrix);

	// For Diffuse Lighting
	float4 normal = normalize(mul(input.NormalVector, WorldInverseTransposeMatrix));
	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.VertexColor = saturate(DiffuseColor * lightIntensity);

	// For Texture
	output.TextureCoordinate = input.TextureCoordinate;

	// For Specular Lighting
	output.NormalVector = normal;

	// For Reflection
	float4 VertexPosition = mul(input.Position, WorldMatrix);
	float3 ViewDirection = ViewVector - VertexPosition;
	output.ReflectionVector = reflect(-normalize(ViewDirection), normalize(normal));

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// For Texture
	float4 VertexTextureColor = tex2D(TextureSampler, input.TextureCoordinate);
	VertexTextureColor.a = 1;

	// For Specular Lighting
	float3 light = normalize(DiffuseLightDirection);
	float3 normal = normalize(input.NormalVector);
	float3 r = normalize(2 * dot(light, normal) * normal - light);
	float3 v = normalize(mul(normalize(ViewVector), WorldMatrix));

	float dotProduct = dot(r, v);
	float4 specular = SpecularColor * max(pow(dotProduct, ShininessFactor), 0) * length(input.VertexColor);


	return saturate(VertexTextureColor * texCUBE(EnvironmentSampler, normalize(input.ReflectionVector)) + specular * 2);
}

technique Reflection
{
	pass Pass1
	{
		VertexShader = compile vs_1_1 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}