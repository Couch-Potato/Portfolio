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
} sbo;

layout(set = 0, binding = 2) uniform MaterialBufferObject {
    float shininess;
} material;


layout(binding = 1) uniform sampler2D diffuse;
layout(binding = 3) uniform sampler2D specular;
layout(binding = 4) uniform sampler2D shadowMap;

layout(location = 0) in vec3 fragColor;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in vec3 aNormal;
layout(location = 3) in vec3 fragPos;
layout(location = 4) in vec4 fragPosLightSpace;

layout(location = 0) out vec4 outColor;

vec3 CalcDirLight(XE_LIGHT_DIRECTIONAL light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(XE_LIGHT_POINT light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(XE_LIGHT_SPOT light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 upk(vec4 Avec);
float Constant = 1.0;
float Linear = 0.09;
float Quadratic = 0.032;


float ShadowCalculation()
{
     // perform perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // check whether current frag pos is in shadow
    float bias = 0.005;
    float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;  

    return shadow;
}

void main() {

    vec3 norm = normalize(aNormal);
    vec3 viewDir = normalize(sbo.viewPos - fragPos);

    vec3 result = vec3(0, 0, 0);

      if(sbo.directional.enabled) {
        result += clamp(CalcDirLight(sbo.directional, norm, viewDir), vec3(0.0), vec3(1.0));
      }
      
    
     for(int i = 0; i < 4; i++)
        result += clamp(CalcPointLight(sbo.points[i], norm, fragPos, viewDir), vec3(0.0), vec3(1.0));    

     for(int i = 0; i < 4; i++)
        result += clamp(CalcSpotLight(sbo.spots[i], norm, fragPos, viewDir), vec3(0.0), vec3(1.0));  

    outColor = vec4(result, 1);
}

vec3 CalcDirLight(XE_LIGHT_DIRECTIONAL light, vec3 normal, vec3 viewDir)
{
    if (light.enabled == false)
        return vec3(0,0,0);

    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // combine results
    vec3 ambient = light.ambient * vec3(texture(diffuse, TexCoords));
    vec3 diffuse = (light.ambient-light.specular) * diff * vec3(texture(diffuse, TexCoords)); // NEED TO FIX LATER!!!
    //vec3 diffuse = vec3(0);
    vec3 specular = light.specular * spec * vec3(texture(specular, TexCoords));
    //vec3 specular = vec3(0);
    float shadow = ShadowCalculation();
    return (ambient + (1.0 - shadow) * (diffuse + specular));
}

// calculates the color when using a point light.
vec3 CalcPointLight(XE_LIGHT_POINT light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    if (light.enabled == false)
        return vec3(0,0,0);

    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distanceZ = length(light.position - fragPos);
    float attenuation = 1.0 / (Constant + Linear * distanceZ + Quadratic * (distanceZ * distanceZ));    
    // combine results
    vec3 ambient = light.ambient * vec3(texture(diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(specular, TexCoords));
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
    // return vec3(light.constant, light.linear, light.quadratic);
}

// calculates the color when using a spot light.
vec3 CalcSpotLight(XE_LIGHT_SPOT light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    if (light.enabled == false)
        return vec3(0,0,0);

    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (Constant + Linear * distance + Quadratic * (distance * distance));    
    // spotlight intensity
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    // combine results
    vec3 ambient = light.ambient * vec3(texture(diffuse, TexCoords));
    vec3 diffuse = (light.ambient-light.specular) * diff * vec3(texture(diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(specular, TexCoords));
    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;

    
    return (ambient + diffuse + specular);
}


vec3 upk(vec4 Avec){
    return vec3(Avec.x, Avec.y, Avec.z);
}