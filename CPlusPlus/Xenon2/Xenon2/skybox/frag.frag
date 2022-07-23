#version 450
#extension GL_ARB_separate_shader_objects : enable

float CubeSize = 512;

layout(binding = 1) uniform samplerCube diffuse;
layout(location = 1) in vec3 TexCoords;
layout(location = 2) in vec3 aNormal;
layout(location = 0) out vec4 outColor;

vec3 FixCubeLookup(vec3 v, int level);

vec3 FixCubeLookup(vec3 v, int level)
{
	float M = max(max(abs(v.x), abs(v.y)), abs(v.z));
	//float size = CubeSize >> level;
	//float scale = (size - 1) / size;

	float scale = 1 - exp2(level) / 512;
	if (abs(v.x) != M) v.x *= scale;
	if (abs(v.y) != M) v.y *= scale;
	if (abs(v.z) != M) v.z *= scale;
	return v;
}

void main() {
    vec3 normal = normalize(aNormal);
    outColor = texture(diffuse, FixCubeLookup(TexCoords, 4));
}
