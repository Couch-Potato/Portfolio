#version 450
#extension GL_ARB_separate_shader_objects : enable

struct XE_LIGHT_DIRECTIONAL {
	bool enabled;
	vec3 direction;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
    mat4 lightSpaceMatrix;
};

struct XE_LIGHT_POINT {
	bool enabled;
	vec3 position;
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float constant;
	float linear;
	float quadratic;
};

struct XE_LIGHT_SPOT {
	bool enabled;
	vec3 position;
	vec3 direction;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float cutOff;
	float outerCutOff;

	float constant;
	float linear;
	float quadratic;
};

layout(set = 0, binding = 0) uniform UniformBufferObject {
    mat4 view;
    mat4 projection;
    vec3 viewPos;
	XE_LIGHT_DIRECTIONAL directional;
	XE_LIGHT_SPOT spots[4];
	XE_LIGHT_POINT points[4];
} SCENE;

layout( push_constant ) uniform constants {
    mat4 world;
} ENTITY;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inColor;
layout(location = 2) in vec2 inTexCoord;
layout(location = 3) in vec3 normal;

layout(location = 0) out vec3 fragColor;
layout(location = 1) out vec2 fragTexCoord;
layout(location = 2) out vec3 aNormal;
layout(location = 3) out vec3 fragPos;
layout(location = 4) out vec4 fragPosLightSpace;

void main() {
    gl_Position = sbo.projection * sbo.view * pc.world * vec4(inPosition, 1.0);
    aNormal = mat3(transpose(inverse(pc.world))) * normal; 
    fragPos = vec3(pc.world * vec4(inPosition, 1.0));
    fragPosLightSpace = sbo.directional.lightSpaceMatrix * vec4(fragPos, 1.0);
    fragTexCoord = inTexCoord;
    fragColor = vec3(1,1,1);
}