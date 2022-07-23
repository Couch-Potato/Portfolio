float4x4 World;
float4x4 View;
float4x4 Projection;

texture colorTexture;

float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseLightColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float ToonThresholds[4] = { 0.95,0.5, 0.2, 0.03 };
float ToonBrightnessLevels[5] = { 1.0, 0.8, 0.6, 0.35, 0.01 };


sampler2D colorSampler = sampler_state
{
	Texture = <ColorMap>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
};

struct VertexShaderInput {
	float4 position : POSITION0;
	float3 normal	:NORMAL0;
	float2 uv		: TEXCOORD0;
};

struct VertexShaderOutput {
	float4 position : POSITION0;
	float3 normal   : TEXCOORD1;
	float2 uv		: TEXCOORD0;
};

VertexShaderOutput std_VS(VertexShaderInput input) {
	VertexShaderOutput output;
	float4 worldPosition = mul(input.position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.position = mul(viewPosition, Projection);

	output.normal = normalize(mul(input.normal, World));
	output.uv = input.uv;
	return output;
}

float4 std_PS(VertexShaderOutput input) : COLOR0{

	float lightIntensity = dot(normalize(DiffuseLightDirection),
		 input.normal);
	if (lightIntensity < 0)
		lightIntensity = 0;

	float4 color = tex2D(colorSampler, input.uv) *
			 DiffuseLightColor * DiffuseIntensity;
	color.a = 1;

	if (lightIntensity > ToonThresholds[0])
		color *= ToonBrightnessLevels[0];
	else if (lightIntensity > ToonThresholds[1])
		color *= ToonBrightnessLevels[1];
	else if (lightIntensity > ToonThresholds[2])
		color *= ToonBrightnessLevels[2];
	else if (lightIntensity > ToonThresholds[3])
		color *= ToonBrightnessLevels[3];
	else
		color *= ToonBrightnessLevels[4];
			return color;
}

technique Toon {
	pass p0 {
		VertexShader = compile vs_2_0 std_VS();
		PixelShader = compile ps_2_0 std_PS();
	}
}