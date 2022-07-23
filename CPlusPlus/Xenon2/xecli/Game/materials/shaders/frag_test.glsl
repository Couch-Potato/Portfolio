#include <DystroLib/Frag>
layout(location = 0) in vec3 fragColor;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in vec3 aNormal;
layout(location = 3) in vec3 fragPos;
layout(location = 4) in vec4 fragPosLightSpace;

layout(location = 0) out vec4 outColor;

void main(){
    vec3 result = vec3(0,0,0);

    result = vec3(texture(diffuse, TexCoords));

    
    outColor = texture(diffuse, TexCoords);

    outColor = vec4(outColor.rgb,1.0);
}