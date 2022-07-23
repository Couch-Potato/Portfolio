#version 450
#extension GL_ARB_separate_shader_objects : enable

layout(set = 0, binding = 0) uniform UniformBufferObject {
    mat4 view;
    mat4 projection;
    float ambientStrength;
    float specStrength;
    vec3 lightPos;
    vec3 lightColor;
    vec3 viewPos;
} sbo;


layout(location = 0) in vec3 inPosition;
layout(location = 3) in vec3 normal;

layout(location = 1) out vec3 fragTexCoord;
layout(location = 2) out vec3 aNormal;

void main() {
    gl_Position = sbo.projection * sbo.view * vec4(inPosition, 1.0);
    fragTexCoord = inPosition;
    aNormal = normal;
}