layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inColor;
layout(location = 2) in vec2 inTexCoord;
layout(location = 3) in vec3 normal;

layout(location = 0) out vec3 fragColor;
layout(location = 1) out vec2 fragTexCoord;
layout(location = 2) out vec3 aNormal;
layout(location = 3) out vec3 fragPos;
layout(location = 4) out vec4 fragPosLightSpace;


void main(){
    gl_Position = SCENE.projection * SCENE.view * ENTITY.world * vec4(inPosition, 1.0);
    aNormal = mat3(transpose(inverse(ENTITY.world))) * normal; 
    fragPos = vec3(ENTITY.world * vec4(inPosition, 1.0));
    fragPosLightSpace = SCENE.directional.lightSpaceMatrix * vec4(fragPos, 1.0);
    fragTexCoord = inTexCoord;
    fragColor = vec3(1,1,1);
}