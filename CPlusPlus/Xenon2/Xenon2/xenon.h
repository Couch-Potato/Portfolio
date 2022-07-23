#pragma once
#define NUM_BONES_PER_VERTEX 4;
#define GLM_ENABLE_EXPERIMENTAL
#define NOMINMAX
#include <string>
#include <vulkan/vulkan.h>
#define GLFW_INCLUDE_VULKAN
#include <GLFW/glfw3.h>
#define GLFW_EXPOSE_NATIVE_WIN32
#include <GLFW/glfw3native.h>
#include <vector>
#include <stdexcept>
#include <plog/Log.h>
#include <optional>
#include <set>
#include <vulkan/vulkan_core.h>
#include <array>
#include "xe_buffer.h"
#include <fstream>
#include <glm/gtx/hash.hpp>
#define GLM_FORCE_RADIANS
#define GLM_FORCE_DEPTH_ZERO_TO_ONE
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <map>
#include <v8.h>

#define NUM_POINT_LIGHTS 4
#define NUM_SPOT_LIGHTS 4

#define SHADOWMAP_DIM 2048
#define DEPTH_FORMAT VK_FORMAT_D16_UNORM
#define DEFAULT_SHADOWMAP_FILTER VK_FILTER_LINEAR


typedef uint64_t owner_t;
typedef uint32_t region_t;

class XenonEngine;

enum {
	XE_LOG_NONE,
	XE_LOG_FATAL,
	XE_LOG_ERROR,
	XE_LOG_WARNING,
	XE_LOG_INFO,
	XE_LOG_DEBUG,
	XE_LOG_VERBOSE
};


struct QueueFamilyIndices {
	std::optional<uint32_t> graphicsFamily;
	std::optional<uint32_t> presentFamily;

	bool isComplete() {
		return graphicsFamily.has_value() && presentFamily.has_value();
	}
};
struct SwapChainSupportDetails {
	VkSurfaceCapabilitiesKHR capabilities;
	std::vector<VkSurfaceFormatKHR> formats;
	std::vector<VkPresentModeKHR> presentModes;
};
class EngineAttached {
public:
	XenonEngine* Engine;
	void Initialize(XenonEngine* engine) {
		Engine = engine;
	}
};

struct FrameBufferAttachment {
	VkImage image;
	VkDeviceMemory mem;
	VkImageView view;
};
struct XeOffscreenPass {
	int32_t width, height;
	VkFramebuffer frameBuffer;
	FrameBufferAttachment depth;
	VkRenderPass renderPass;
	VkSampler depthSampler;
	VkDescriptorImageInfo descriptor;
};
class XeOffScreen : public EngineAttached {
public:
	void Init();
	XeOffscreenPass OffscreenPass;
	VkPipeline pipeline;
	VkPipelineLayout Layout;
private:
	void prepareRenderpass();
	void preparePipeline();
};

class XeUtility:public EngineAttached {
public:
	void createBuffer(VkDeviceSize size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties, VkBuffer& buffer, VkDeviceMemory& bufferMemory);
	void copyBufferToImage(VkBuffer buffer, VkImage image, uint32_t width, uint32_t height);
	void createImage(uint32_t width, uint32_t height, uint32_t mipLevels, VkSampleCountFlagBits numSamples, VkFormat format, VkImageTiling tiling, VkImageUsageFlags usage, VkMemoryPropertyFlags properties, VkImage& image, VkDeviceMemory& imageMemory, char arrayLayers);
	//void createImage(uint32_t width, uint32_t height, uint32_t mipLevels, VkSampleCountFlagBits numSamples, VkFormat format, VkImageTiling tiling, VkImageUsageFlags usage, VkMemoryPropertyFlags properties, VkImage& image, VkDeviceMemory& imageMemory);
	bool hasStencilComponet(VkFormat format) {
		return format == VK_FORMAT_D32_SFLOAT_S8_UINT || format == VK_FORMAT_D24_UNORM_S8_UINT;
	}
	void transitionImageLayout(VkImage image, VkFormat format, VkImageLayout oldLayout, VkImageLayout newLayout, uint32_t mipLevels, char layerCount = 1);
	VkCommandBuffer beginSingleTimeCommands();
	void endSingleTimeCommands(VkCommandBuffer commandBuffer);
	VkImageView createImageView(VkImage image, VkFormat format, VkImageAspectFlags aspectFlags, uint32_t mipLevels);
	uint32_t findMemoryType(uint32_t typeFilter, VkMemoryPropertyFlags properties);
};
/*
* Internal device manager
*/
class XeDeviceManager
{
public:
	void Cleanup();
	char* PhysicalDeviceName;
	VkPhysicalDeviceType PhysicalDeviceType;
	uint32_t PhysicalDeviceVersion;
	VkPhysicalDevice PhysicalDevice;
	VkDevice Device;
	XenonEngine* Engine;
	VkSampleCountFlagBits MSAASamples;
	VkQueue graphicsQueue;
	VkQueue presentQueue;
	VkPhysicalDeviceFeatures deviceFeatures{};
	void InitializeDevices(XenonEngine* engine);
	void InitializeLogicalDevice();
	SwapChainSupportDetails querySwapChainSupport(VkPhysicalDevice device);
	QueueFamilyIndices findQueueFamilies(VkPhysicalDevice device);
	VkSampleCountFlagBits getMaxUsableSampleCount();
	bool isDeviceSuitable(VkPhysicalDevice device);
	bool checkDeviceExtensionSupport(VkPhysicalDevice device);
};

QueueFamilyIndices XeFindQueueFamilies(XenonEngine* engine);


/*
Draw Surface
*/
class XeSurface:public EngineAttached {
public:
	VkSurfaceKHR surface;
	uint32_t ImageCount;
	std::vector<VkImage> swapChainImages;
	VkFormat swapChainImageFormat = VK_FORMAT_B8G8R8A8_SRGB;
	VkSwapchainKHR swapChain;
	VkExtent2D swapChainExtent;
	std::vector<VkImageView> swapChainImageViews;
	void InitializeSurface(GLFWwindow* window, XenonEngine* engine);
	void InitializeSwapChain();
	VkImage colorImage;
	VkDeviceMemory colorImageMemory;
	VkImageView colorImageView;
	VkImage depthImage;
	VkDeviceMemory depthImageMemory;
	VkImageView depthImageView;
	void CreateColorResources();
	void Cleanup();
private:
	void createImageViews();
	VkSurfaceFormatKHR chooseSwapSurfaceFormat(const std::vector<VkSurfaceFormatKHR>& availableFormats);
	VkPresentModeKHR chooseSwapPresentMode(const std::vector<VkPresentModeKHR>& availablePresentModes);
	VkExtent2D chooseSwapExtent(const VkSurfaceCapabilitiesKHR& capabilities);
};
/*
Constructs the render pass
*/
class XeRenderer : public EngineAttached {
public:
	XeOffScreen OffScreenPass;
	VkRenderPass RenderPass;
	void Initialize(XenonEngine* eng){
		EngineAttached::Initialize(eng);
		OffScreenPass.Initialize(eng);
		PLOGV << "Initializing depth pass";
		OffScreenPass.Init();
		PLOGV << "Initializing renderer";
		InitializeRenderer();
	}
	VkFormat findDepthFormat();
	void Cleanup();
private:
	void InitializeRenderer();
	VkFormat findSupportedFormat(const std::vector<VkFormat>& candidates, VkImageTiling tiling, VkFormatFeatureFlags features);
};
/*
* Holds everything relating to an engine intance;
*/

#pragma region Utility Functions

/*
Command buffer linked with a Xenon object
*/
struct XeLinkedCommandBuffer {
	VkCommandBuffer CommandBuffer;
	XenonEngine* LinkedEngine;
};

static VKAPI_ATTR VkBool32 VKAPI_CALL debugCallback(
	VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity,
	VkDebugUtilsMessageTypeFlagsEXT messageType,
	const VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
	void* pUserData);

void XeCreateBuffer(XenonEngine* engine, VkDeviceSize size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties, VkBuffer& buffer, VkDeviceMemory& bufferMemory);
void XeCopyBufferToImage(XenonEngine* engine, VkBuffer buffer, VkImage image, uint32_t width, uint32_t height);
void XeCreateImage(XenonEngine* engine, uint32_t width, uint32_t height, uint32_t mipLevels, VkSampleCountFlagBits numSamples, VkFormat format, VkImageTiling tiling, VkImageUsageFlags usage, VkMemoryPropertyFlags properties, VkImage& image, VkDeviceMemory& imageMemory, char arrayLayers = 1);
VkImageView XeCreateImageView(XenonEngine* engine, VkImage image, VkFormat format, VkImageAspectFlags aspectFlags, uint32_t mipLevels);

void XeTransitionImageLayout(XenonEngine* engine, VkImage image, VkFormat format, VkImageLayout oldLayout, VkImageLayout newLayout, uint32_t mipLevels);

XeLinkedCommandBuffer XeBeginCommands(XenonEngine* engine); 
void XeEndCommands(XeLinkedCommandBuffer buf);

std::vector<char> XeReadFile(const std::string& name);

VkShaderModule XeCreateShaderModule(XenonEngine* engine, const std::vector<char>& code);
VkShaderModule XeCreateShaderModule(XenonEngine* engine, char* code, size_t entrySize);
VkPipelineShaderStageCreateInfo XeCreateShader(XenonEngine* engine, VkShaderStageFlagBits stage, const std::vector<char>& code);
VkPipelineShaderStageCreateInfo XeCreateShader(XenonEngine* engine, VkShaderStageFlagBits stage, VkShaderModule shader);
VkDevice XeGetDevice(XenonEngine* engine);
VkDevice XeGetDevice(EngineAttached* attached);
VkRenderPass XeGetRenderPass(XenonEngine* engine);



#pragma endregion

const int NUM_BONES_PER_VERT = NUM_BONES_PER_VERTEX;

struct Vertex {
	glm::vec3 pos;
	glm::vec3 color;
	glm::vec2 texCoord;
	glm::vec3 normal;
	UINT IDs[NUM_BONES_PER_VERT];
	float Weights[NUM_BONES_PER_VERT];

	static VkVertexInputBindingDescription getBindingDescription() {
		VkVertexInputBindingDescription bindingDescription{};
		bindingDescription.binding = 0;
		bindingDescription.stride = sizeof(Vertex);
		bindingDescription.inputRate = VK_VERTEX_INPUT_RATE_VERTEX;

		return bindingDescription;
	}

	static std::array<VkVertexInputAttributeDescription, 6> getAttributeDescriptions() {
		std::array<VkVertexInputAttributeDescription, 6> attributeDescriptions{};

		attributeDescriptions[0].binding = 0;
		attributeDescriptions[0].location = 0;
		attributeDescriptions[0].format = VK_FORMAT_R32G32B32_SFLOAT;
		attributeDescriptions[0].offset = offsetof(Vertex, pos);

		attributeDescriptions[1].binding = 0;
		attributeDescriptions[1].location = 1;
		attributeDescriptions[1].format = VK_FORMAT_R32G32B32_SFLOAT;
		attributeDescriptions[1].offset = offsetof(Vertex, color);

		attributeDescriptions[2].binding = 0;
		attributeDescriptions[2].location = 2;
		attributeDescriptions[2].format = VK_FORMAT_R32G32_SFLOAT;
		attributeDescriptions[2].offset = offsetof(Vertex, texCoord);

		attributeDescriptions[3].binding = 0;
		attributeDescriptions[3].location = 3;
		attributeDescriptions[3].format = VK_FORMAT_R32G32B32_SFLOAT;
		attributeDescriptions[3].offset = offsetof(Vertex, normal);

		attributeDescriptions[4].binding = 0;
		attributeDescriptions[4].location = 4;
		attributeDescriptions[4].format = VK_FORMAT_A8B8G8R8_UINT_PACK32;
		attributeDescriptions[4].offset = offsetof(Vertex, IDs);

		attributeDescriptions[5].binding = 0;
		attributeDescriptions[5].location = 5;
		attributeDescriptions[5].format = VK_FORMAT_R32G32B32A32_SFLOAT;
		attributeDescriptions[5].offset = offsetof(Vertex, Weights);


		return attributeDescriptions;
	}

	bool operator==(const Vertex& other) const {
		return pos == other.pos && color == other.color && texCoord == other.texCoord && normal == other.normal;
	}
};
namespace std {
	template<> struct hash<Vertex> {
		size_t operator()(Vertex const& vertex) const {
			return ((hash<glm::vec3>()(vertex.pos) ^
				(hash<glm::vec3>()(vertex.color) << 1)) >> 1) ^
				(hash<glm::vec2>()(vertex.texCoord) << 1);
		}
	};
}

struct XE_LIGHT_DIRECTIONAL {
	bool enabled = true;
	glm::vec3 direction;
	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;
	glm::mat4 lightSpaceMatrix;
};

struct XE_LIGHT_POINT {
	bool enabled = true;
	glm::vec3 position;

	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;

	float constant;
	float linear;
	float quadratic;
};

struct XE_LIGHT_SPOT {
	bool enabled = true;
	glm::vec3 position;
	glm::vec3 direction;

	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;

	float cutOff;
	float outerCutOff;

	float constant;
	float linear;
	float quadratic;
};


struct EntityPushConstant{
	glm::mat4 world;
};
struct XeSceneData{
	glm::mat4 view;
	glm::mat4 projection;
	glm::vec3 viewPos;
	XE_LIGHT_DIRECTIONAL directional;
	XE_LIGHT_SPOT spots[NUM_SPOT_LIGHTS];
	XE_LIGHT_POINT points[NUM_POINT_LIGHTS];
};
struct XeDefaultMaterialData{
	float shininess = 32;
};
class XeDisposable {
public:
	//virtual void Cleanup();
};
class XeMultiBuffer :public EngineAttached, public XeDisposable
{
public:
	XeMultiBuffer() {}
	XeMultiBuffer(VkDeviceSize bufferSize);
	XeMultiBuffer(VkDeviceSize bufferSize, void* ix);
	VkDeviceSize BufferSize;
	void Create();
	void Cleanup();
	void Update(uint32_t i, void* data);
	VkBuffer GetBuffer(uint32_t i);
private:
	void* initset = NULL;
	std::vector<VkBuffer> i_buffers;
	std::vector<VkDeviceMemory> i_device_memory;
};
class XeTexture: public EngineAttached {
public:
	VkImage Image;
	VkImageView ImageView;
	VkSampler Sampler;
	VkDeviceMemory ImageMemory;
	uint32_t mipLevels;
	void SetImageBuffer(void* pixelData, int width, int height, bool generateMipmaps = false);
	void Cleanup();
private:
	void createTextureSampler();
protected:
	void generateMipmaps(VkImage image, VkFormat imageFormat, int32_t texWidth, int32_t texHeight, uint32_t mipLevels, char layerCount=1);

};

class XeCubemap : public XeTexture {
public:
	XeCubemap(std::string* images);
	XeCubemap() {}
	std::string Front;
	std::string Back;
	std::string Up;
	std::string Down;
	std::string Right;
	std::string Left;
	void Cleanup();
	void Init();

};

struct XeDescriptorTemplate {
	VkDescriptorType DescriptorType;
	VkShaderStageFlags Stages;
};

#pragma region Descriptor Functions
VkDescriptorSetLayout XeGetLayoutFromTemplate(XenonEngine* engine, std::vector<XeDescriptorTemplate> bindings);
VkDescriptorPool XeGetDescriptorPoolFromTemplate(XenonEngine* engine, std::vector<XeDescriptorTemplate> bindings);
VkWriteDescriptorSet XeTextureToDescriptor(XenonEngine* engine, XeTexture* texture, VkDescriptorSet dest, int bindingId);
VkWriteDescriptorSet XeBufferToDescriptor(XenonEngine* engine, XeMultiBuffer* buffer, VkDescriptorSet dest, uint32_t swapChainImageId, int bindingId);
std::vector<VkDescriptorSet> XeAllocateDescriptorSets(XenonEngine* engine, VkDescriptorSetLayout layout, VkDescriptorPool pool);
void XeWriteDescriptorSets(XenonEngine* engine, std::vector<VkWriteDescriptorSet>writes);
#pragma endregion



enum XeLightingType {
	XE_LIGHTING_TYPE_DIRECTIONAL,
	XE_LIGHTING_TYPE_POINT,
	XE_LIGHTING_TYPE_SPOT,
	XE_LIGHTING_TYPE_NONE
};

class XeLight {
public:
	XeLightingType LightType = XE_LIGHTING_TYPE_NONE;
	glm::vec3 Direction;
	glm::vec3 Ambient;
	glm::vec3 Diffuse;
	glm::vec3 Specular;
	glm::vec3 Position;
	float Constant = 1.0f;
	float Linear = 0.09f;
	float Quadratic = 0.032f;
	float Radius;
	float OuterRadius;
	XeLight(){}
	XeLight(glm::vec3 dorpos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec, XeLightingType lightType) {
		if (lightType == XeLightingType::XE_LIGHTING_TYPE_DIRECTIONAL)
			Direction = dorpos;
		else if (lightType == XeLightingType::XE_LIGHTING_TYPE_POINT)
			Position = dorpos;
		else
			throw std::runtime_error("Could not construct light, it has an invalid lighting type for the parameters specified");
		Ambient = ambient;
		Diffuse = diffuse;
		Specular = spec;
		LightType = lightType;
	}
	XeLight(glm::vec3 dir, glm::vec3 pos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec, float radii, float oradii) {
		Ambient = ambient;
		Diffuse = diffuse;
		Specular = spec;
		Direction = dir;
		Position = pos;
		Radius = radii;
		OuterRadius = oradii;
		LightType = XeLightingType::XE_LIGHTING_TYPE_SPOT;
	}
	XE_LIGHT_SPOT GetSpot() {
		if (LightType == XeLightingType::XE_LIGHTING_TYPE_SPOT) {
			XE_LIGHT_SPOT spot{};
			spot.direction = glm::vec3(Direction);
			spot.position = glm::vec3(Position);
			spot.ambient = glm::vec3(Ambient);
			spot.specular = glm::vec3(Specular);
			spot.diffuse = glm::vec3(Diffuse);
			spot.cutOff = glm::cos(glm::radians(Radius));
			spot.outerCutOff = glm::cos(glm::radians(OuterRadius));

			spot.constant = Constant;
			spot.linear = Linear;
			spot.quadratic = Quadratic;


			return spot;
		}
		else {
			throw std::runtime_error("Invalid light getter.");
		}
	}
	XE_LIGHT_DIRECTIONAL GetDirectional() {
		if (LightType == XeLightingType::XE_LIGHTING_TYPE_DIRECTIONAL) {
			XE_LIGHT_DIRECTIONAL direct{};
			direct.direction = glm::vec3(Direction);
			direct.ambient = glm::vec3(Ambient);
			direct.specular = glm::vec3(Specular);
			direct.diffuse = glm::vec3(Diffuse);
			glm::mat4 lightProj = glm::ortho(-10.0f, 10.0f, -10.0f, 10.0f, 1.0f, 7.5f);
			//TODO:: TURN DIRECTIONAL DATA INTO MATRIX!!!
			glm::mat4 lightView = glm::lookAt(glm::vec3(-2.0f, 4.0f, -1.0f),
				glm::vec3(0.0f, 0.0f, 0.0f),
				glm::vec3(0.0f, 1.0f, 0.0f));

			direct.lightSpaceMatrix = lightProj * lightView;
			return direct;
		}
		else {
			throw std::runtime_error("Invalid light getter.");
		}
	}
	XE_LIGHT_POINT GetPoint() {
		if (LightType == XeLightingType::XE_LIGHTING_TYPE_POINT) {
			XE_LIGHT_POINT direct{};
			direct.position = glm::vec3(Direction);
			direct.ambient = glm::vec3(Ambient);
			direct.specular = glm::vec3(Specular);
			direct.diffuse = glm::vec3(Diffuse);
			return direct;
		}
		else {
			throw std::runtime_error("Invalid light getter.");
		}
	}
};

class XePipeline: public EngineAttached, public XeDisposable {
public:
	VkPipelineShaderStageCreateInfo VertexShader;
	VkPipelineShaderStageCreateInfo FragmentShader;
	VkPipelineLayout Layout;
	VkPipeline Pipeline;
	void InititalizePipeline(VkDescriptorSetLayout SetLayout, VkPipelineShaderStageCreateInfo vertex, VkPipelineShaderStageCreateInfo frag, bool isDepthOnly = false);
	void Cleanup();
};
struct XeShaderSet {
	VkPipelineShaderStageCreateInfo VertexShader;
	VkPipelineShaderStageCreateInfo DepthVertexShader;
	VkPipelineShaderStageCreateInfo FragmentShader;
};
class XeMaterial : public EngineAttached, public XeDisposable {
public:
	XeMaterial() {}
	XeShaderSet Shaders;
	VkDescriptorSetLayout DSetLayout;
	VkDescriptorPool DescriptorPool;
	std::vector<VkDescriptorSet> DescriptorSets;
	XePipeline Pipeline;
	XePipeline ShadowPipeline;
	void Bind(char drawId, VkCommandBuffer commandBuffer);
	virtual void HandleBind(char drawId, VkCommandBuffer commandBuffer);
	virtual void Cleanup();
	virtual void InitializeMaterial();
protected:
	void BindPipeline(VkCommandBuffer commandBuffer);
	void InitializePipeline();
};
void XeBindMaterial(XeMaterial* material, char drawId, VkCommandBuffer commandBuffer);
void XeBindMaterialToDepth(XeMaterial* material, char drawId, VkCommandBuffer commandBuffer);
class TestMaterial : public XeMaterial {
	XeMultiBuffer SceneBuffer;
	XeTexture Diffuse;
	XeMultiBuffer MaterialDataBuffer;
	virtual void HandleBind(char drawId, VkCommandBuffer commandBuffer);
	virtual void InitializeMaterial();
public:
	void Cleanup();
};

class XeDefaultMaterial : public XeMaterial {
public:
	XeDefaultMaterial() : XeMaterial() {
		SceneBuffer = XeMultiBuffer(sizeof(XeSceneData));
		MaterialDataBuffer = XeMultiBuffer(sizeof(XeDefaultMaterialData));
	}
	XeDefaultMaterialData MaterialData;
	XeTexture Diffuse;
	XeTexture Specular;
	void InitializeMaterial();
	void HandleBind(char drawId, VkCommandBuffer commandBuffer);
	void Cleanup();
private:
	XeMultiBuffer SceneBuffer;
	XeMultiBuffer MaterialDataBuffer;
	
};

size_t XeHashString(std::string str);

enum XeBufferType {
	UNIFORM,
	TEXTURE
};

struct XeBufferItem {
	XeBufferType type;
	XeMultiBuffer internalBuffer;
	VkImageView imageView;
	VkSampler imageSampler;
};

class XeDynamicMaterial : public XeMaterial {
public:
	XeDynamicMaterial() : XeMaterial() {
		SceneBuffer = XeMultiBuffer(sizeof(XeSceneData));
	}
	XeDefaultMaterialData MaterialData;
	void InitializeMaterial();
	XeTexture* LinkTexture(XeTexture texture);
	XeBufferItem* InsertBuffer(XeBufferItem bufferData);
	void HandleBind(char drawId, VkCommandBuffer commandBuffer);
	void Cleanup();
	size_t MaterialId;
private:
	XeMultiBuffer SceneBuffer;
	std::vector<XeBufferItem> buffers;
	std::vector<XeTexture> linkedTextures;
};



class XeSkyboxMaterial : public XeMaterial {
public:
	XeSkyboxMaterial() : XeMaterial() {
		
	}
	void InitializeMaterial();
	void HandleBind(char drawId, VkCommandBuffer buffer);
	void Cleanup();
private:

};




struct XeMesh {
	std::vector<Vertex> Vertices;
	std::vector<uint32_t> indices;
};

struct XeBufferBinding {
	VkBuffer Buffer; 
	VkDeviceMemory DeviceMemory;
};

XeBufferBinding XeCreateBufferBinding(XenonEngine* engine, VkDeviceSize bufferSize, void* buffer, VkBufferUsageFlags flags);
void XeCleanupBuffer(XenonEngine* engine, XeBufferBinding binding);

class XeEntity: public EngineAttached {
public:
	XeDynamicMaterial* Material;
	EntityPushConstant PushConstant;
	void Draw(VkCommandBuffer commandBuffer);
	virtual void Cleanup();
	virtual void DrawCmd(VkCommandBuffer commandBuffer);
protected:
};

class XeComponent;

class XeMeshedEntity : public XeEntity {
public:
	owner_t handle;
	XeMesh Mesh;
	XeBufferBinding VertexBuffer;
	XeBufferBinding IndexBuffer;
	std::vector<XeMeshedEntity* > children;
	std::vector<XeComponent*> cList;
	std::map<size_t, XeComponent* > components;
	glm::vec3 Scale = glm::vec3(1.0f);
	glm::quat Orientation = glm::quat();
	glm::vec3 Position = glm::vec3(0);
	void InitializeMeshBuffer();
	void Cleanup();
	void Update();
	void Draw(VkCommandBuffer commandBuffer);
	void DrawCmd2(VkCommandBuffer commandBuffer);
protected:
	
};

class XeComponent : public EngineAttached {
	std::map<std::size_t, std::string> scripts;
	
	bool isAttachedToEntity;
public:
	XeMeshedEntity* linkedEntity;
	v8::Local<v8::Context> context;
	XeComponent() {};
	size_t id;
	void Run();
	void AddScript(std::size_t id, std::string content);
	void Attach(XeMeshedEntity* entity);
	void Dispose(); // todo
	v8::Global<v8::Value> exports;
private:
	
};

class XeBootstrappable : public EngineAttached {
	
	std::map<std::size_t, std::string> scripts;
public:
	v8::Local<v8::Context> context;
	XeBootstrappable() {};
	size_t id;
	void Run();
	void AddScript(std::size_t id, std::string content);
	void Dispose(); // todo
	v8::Global<v8::Value> exports;
};

class XeSkybox : public XeEntity {
public:
	XeMesh Mesh;
	XeCubemap Cubemap;
	XeBufferBinding VertexBuffer;
	XeBufferBinding IndexBuffer;
	void Draw(VkCommandBuffer commandBuffer);
	void DrawCmd2(VkCommandBuffer commandBuffer);
	void InitializeMeshBuffer();
};

void XeDrawEntity(VkCommandBuffer commandBuffer, XeMeshedEntity* entity);
void XeDrawSkybox(VkCommandBuffer commandBuffer, XeSkybox* skybox);

enum XeMaterialType {
	XE_MATERIAL_DEFAULT
};

enum XeTextureImportChannels {
	XE_GRAY = 1,
	XE_GRAY_ALPHA = 2,
	XE_RGB = 3,
	XE_RGB_ALPHA = 4
};

enum XeShaderImportMethod {
	XE_NO_COMPILE,
	XE_PRE_COMPILED,
};

class XeShaderImportInfo {
public:
	std::string ImportLocation = "";
	XeShaderImportMethod Method = XeShaderImportMethod::XE_PRE_COMPILED;
};

class XeTextureImportInfo {
public:
	XeTextureImportChannels Channels = XeTextureImportChannels::XE_RGB_ALPHA;
	std::string ImportLocation = "";
};

class XeMaterialCreateInfo {
public:
	XeShaderImportInfo FragmentShader;
	XeShaderImportInfo VertexShader;
	XeTexture Diffuse;
	XeTexture Specular;
	XeMaterialType MaterialType = XeMaterialType::XE_MATERIAL_DEFAULT;
};

enum XeEntityType {
	XE_ENTITY_MESHED_ENTITY
};

enum XeMeshType {
	XE_MESH_FBX,
	XE_MESH_OBJ
};

class XeMeshCreateInfo {
public:
	XeMeshType MeshType = XeMeshType::XE_MESH_OBJ;
	std::string ImportLocation = "";
};

typedef uint16_t XeMaterialId;

class XeEntityCreateInfo {
public:
	XeMaterialId MaterialId = -1;
	XeEntityType EntityType = XeEntityType::XE_ENTITY_MESHED_ENTITY;
	XeMeshCreateInfo Mesh;
	glm::vec3 Scale = glm::vec3(1.0f);
	glm::quat Orientation = glm::quat();
	glm::vec3 Position = glm::vec3(0);
};

typedef uint16_t XeEntityAmountInfoSize;

class XeSkyboxCreateInfo {
public:
	std::string Front;
	std::string Back;
	std::string Up;
	std::string Down;
	std::string Right;
	std::string Left;
	XeShaderImportInfo FragmentShader;
	XeShaderImportInfo VertexShader;
	XeMeshCreateInfo Isosphere;
};

class XeSceneCreateInfo {
public:
	XeEntityCreateInfo* Entities = {};
	XeEntityAmountInfoSize EntityAmount = 0;
	XeMaterialCreateInfo* Materials = {};
	XeEntityAmountInfoSize MaterialAmount = 0;
	bool SkyboxEnabled = false;
	XeSkyboxCreateInfo* Skybox = {};
};

class XeCamera {
public:
	glm::vec3 Position = glm::vec3(2.0, 2.0, -3.0);
	glm::vec3 LookAt = glm::vec3(0.0);
	glm::vec3 UpVector = glm::vec3(0.0, 1.0, 0.0);
};

class XeScene : public EngineAttached {

private:
	VkCommandPool commandPool;
	std::vector<VkCommandBuffer> commandBuffers;
	
	std::map<XeDynamicMaterial*, std::vector<XeMeshedEntity*>> entityPairMap;
public:
	std::vector<XeMeshedEntity*> entityBuffer;
	XeLight DirectionalLight;
	XeCamera Camera;
	std::vector<XeLight> Lights;
	VkCommandBuffer* GetCommandBuffer(uint32_t item);
	XeSceneData GetSceneMaterialData();
	XeSkybox Skybox;
	bool SkyboxEnabled = false;
	std::vector<XeDynamicMaterial> materialRegistry;
	std::vector<XeMeshedEntity> entityRegistry;
	void InsertEntity(XeMeshedEntity* entity);
	std::map<size_t, XeComponent* > components;
	void InsertEntity(XeEntityCreateInfo info);
	XeDynamicMaterial* InsertMaterial(XeDynamicMaterial material);
	void InsertMaterial(XeMaterialCreateInfo mcInfo);
	void SetSkybox(XeSkyboxCreateInfo skybox);
	void DeleteEntity(XeEntity entity);
	void DrawScene(VkCommandBuffer commandBuffer, char drawId, bool isDepth = false);
	void CreateCommandPool();
	void GenerateCommandBuffers();
	void Cleanup();
	void Initialize(XenonEngine* Engine);

	void InsertLight(glm::vec3 dir, glm::vec3 pos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec, float radii, float oradii);
	void InsertLight(glm::vec3 dorpos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec);
	void SetDirectionalLight(glm::vec3 dorpos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec);
};



void XeGenerateBufferAndDrawScene(XeScene* scene);
void XeBootstrapScene(XenonEngine* engine, XeSceneCreateInfo* scene);
XeMesh XeImportMeshFromObj(std::string file);
XeMesh XeImportMeshFromFBX(const std::string& file);
XeTexture XeImportTexture(XenonEngine* engine, XeTextureImportInfo inf);
void XeAttachSkyboxToScene(XeScene* sc, XeSkyboxCreateInfo sb);

class XeScriptEngine : public EngineAttached {
public:
	XeScriptEngine() {};
	v8::Isolate* isolate;
	std::vector<v8::Local<v8::Context>> contexts;
	std::vector<XeComponent*> runningComponents;
	//v8::Isolate::Scope isolate_scope;
	//v8::HandleScope handle_scope;
	std::map<size_t, XeBootstrappable*> coreScripts;
	v8::Global<v8::Value> getCoreModule(std::string modName); // todo
	void InitIsolate(const char* su);
};



struct allocated_t {
	void* ptr;
	size_t region_size;
	enum {
		ALLOCATED_ENTITY,
		ALLOCATED_MATERIAL,
		ALLOCATED_BUFFER,
		ALLOCATED_TEXTURE,
		ALLOCATED_MODEL
	} allocation_type;
	owner_t owner;
	std::vector<region_t> linked_regions;
	bool isFreed = false;
};

class XeMemoryAllocator : public EngineAttached {
public:
	template <class T>
	region_t Allocate(T val, owner_t owner = 0) {
		T* tx = new T;
		*tx = val;
		return Allocate(tx, owner);
	}
	template <class T>
	region_t Allocate(T* val, owner_t owner = 0) {
		/*
	Fragmentation information :::
	This memory will fragment. Easily. In future reference it should be built and designed around this set of memory being fragmented by looking to
	put the object in handles which have been vacated.
	*/

		allocated_t alloc{};
		alloc.allocation_type = allocated_t::ALLOCATED_BUFFER;
		if (sizeof(T) == sizeof(XeMeshedEntity)) {
			alloc.allocation_type = allocated_t::ALLOCATED_ENTITY;
		}
		if (sizeof(T) == sizeof(XeMesh)) {
			alloc.allocation_type = allocated_t::ALLOCATED_MODEL;
		}
		if (sizeof(T) == sizeof(XeDynamicMaterial)) {
			alloc.allocation_type = allocated_t::ALLOCATED_MATERIAL;
		}
		if (sizeof(T) == sizeof(XeTexture)) {
			alloc.allocation_type = allocated_t::ALLOCATED_TEXTURE;
		}


		alloc.region_size = sizeof(T);
		alloc.isFreed = false;
		alloc.owner = owner;
		alloc.ptr = val;
		allocatedRegions.push_back(alloc);
		return allocatedRegions.size() - 1;
	}
	std::vector<allocated_t> allocatedRegions;
	void DeAllocate(region_t reg);
	void LinkRegion(region_t parent, region_t child);
	template <class T>
	T* GetInstanceFromHandle(region_t handle) {
		if (handle >= allocatedRegions.size()) {
			throw std::runtime_error("Invalid handle. The handle provided is outside the range of valid handles.");
		}
		auto rgx = allocatedRegions[handle];
		if (rgx.region_size != sizeof(T))
			throw std::runtime_error("Invalid handle. The object that is requested by the handle does not fit the object that is currently stored.");
		if (rgx.isFreed)
			throw std::runtime_error("Invalid handle. The object has been deinstanced and the pooled memory has been freed.");

		return reinterpret_cast<T*>(rgx.ptr);
	}
};



class XenonEngine {
protected:
	virtual XeSceneCreateInfo* ConstructInitialScene() { return nullptr; }
	virtual void AfterInit(){}
	virtual void AfterCleanup(){}
	virtual void BeforeInit(){}
	int BeginEngine(GLFWwindow* window, int logLevel, const char* module_n);
public:
	XenonEngine() {};
	XeMemoryAllocator memoryManager;
	XeDeviceManager deviceManager;
	XeSurface drawSurface;
	XeRenderer renderer;
	VkInstance instance;
	XeScriptEngine scriptEngine;
	VkDebugUtilsMessengerEXT debugMessenger;
	XeUtility utility;
	GLFWwindow* Window;
	VkCommandPool commandPool;
	XeScene scene;
	std::vector<VkFramebuffer> swapChainFramebuffers;
#ifdef NDEBUG
	const bool enableValidationLayers = false;
#else
	const bool enableValidationLayers = true;
#endif
	const std::vector<const char*> validationLayers = {
"VK_LAYER_KHRONOS_validation"
	};

	virtual void Run();
private:
	size_t currentFrame = 0;
	const int MAX_FRAMES_IN_FLIGHT = 2;
	std::vector<VkFence> inFlightFences;
	std::vector<VkFence> imagesInFlight;
	std::vector<VkSemaphore> imageAvailableSemaphores;
	std::vector<VkSemaphore> renderFinishedSemaphores;
	VkSemaphore imageAvailableSemaphore;
	VkSemaphore renderFinishedSemaphore;
	void initForVulkan();
	void InitializeCommandPool();
	void setupDebugMessenger();
	VkResult CreateDebugUtilsMessengerEXT(VkInstance instance, const VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo, const VkAllocationCallbacks* pAllocator, VkDebugUtilsMessengerEXT* pDebugMessenger);
	void DestroyDebugUtilsMessengerEXT(VkInstance instance, VkDebugUtilsMessengerEXT debugMessenger, const VkAllocationCallbacks* pAllocator);
	std::vector<const char*> getRequiredExtensions();
	bool checkValidationLayerSupport();
	void populateDebugMessengerCreateInfo(VkDebugUtilsMessengerCreateInfoEXT& createInfo);
	void makeInstance();
	void createFramebuffers();
	void createSync();
	void initMemoryManager();
	void initScriptEngine();
	void bootstrapCoreScripts(); 
	void runComponents(); 
	
	void draw();
	void mainLoop();

	void cleanup();
};

template <class T>
region_t XeAlloc(XenonEngine* engine, T val, owner_t owner = 0) {
	return engine->memoryManager.Allocate(val, owner);
}

template <class T>
region_t XeAlloc(XenonEngine* engine, T* val, owner_t owner = 0) {
	return engine->memoryManager.Allocate(val, owner);
}

template <class T>
region_t XeAlloc(XenonEngine* engine, owner_t owner = 0) {
	return engine->memoryManager.Allocate(T(), owner);
}

void XeFree(XenonEngine* engine, region_t val);

template <class T>
T* XeGetInstance(XenonEngine* engine, region_t handle) {
	return engine->memoryManager.GetInstanceFromHandle<T>(handle);
}

bool XeDoesOwnInstance(XenonEngine* engine, region_t handle, owner_t owner);

owner_t XeGetMemoryOwner(XenonEngine* engine, region_t handle);

void XeLinkMemory(XenonEngine* engine, region_t parent, region_t child);

template <class T>
void XeMemSet(XenonEngine* engine, region_t handle, T val) {
	memcpy(engine->memoryManager.allocatedRegions[handle].ptr, &val, sizeof(T));
}

XeMeshedEntity* XE_FACTORY_MESHED_ENTITY(XenonEngine* engine, XeEntityCreateInfo info);
XeDefaultMaterial XE_FACTORY_DEFAULT_MATERIAL(XenonEngine* engine, XeMaterialCreateInfo info);

void XeImportArchive(std::string name);
XeMeshedEntity ImportEntityFromArchive(XenonEngine* engine, size_t name, XeScene* parentScene);
XeScene XeImportSceneFromArchive(XenonEngine* engine, size_t name);
XeTexture ImportTextureFromArchive(XenonEngine* engine, size_t name);
XeDynamicMaterial ImportMaterialFromArchive(XenonEngine* engine, size_t name);
XeComponent* XeImportComponentFromArchive(XenonEngine* engine, size_t compName, XeMeshedEntity* linked = nullptr);
v8::Isolate* XeGetIsolate(XenonEngine* engine);
void XeImportBootstrappables(XenonEngine* engine);


void XeBindComponentToScene(XeScene* scene, XeComponent componet); // todo
void XeBindComponentToEntity(XeMeshedEntity* entity, XeComponent component); // todo
void XeBootstrapCoreScript(XenonEngine* engine, XeBootstrappable boot); // todo

v8::Global<v8::Value>* XeGetComponent(XeMeshedEntity* entity, std::string component_name);
v8::Global<v8::Value>* XeGetComponent(XeMeshedEntity* entity, size_t hash); 
v8::Global<v8::Value>* XeGetCoreLib(XenonEngine* engine, std::string name); 
v8::Global<v8::Value>* XeGetCoreLib(XenonEngine* engine, size_t hash); 

bool XeDoesComponentExist(XeMeshedEntity* entity, std::string component_name);
bool XeDoesComponentExist(XeMeshedEntity* entity, size_t hash);

bool XeDoesCoreLibExist(XenonEngine* engine, std::string component_name);
bool XeDoesCoreLibExist(XenonEngine* engine, size_t hash);


struct XeGetScriptFromContextResult {
	XeComponent* component;
	XeBootstrappable* bootstrappable;
	bool isComponent;
	bool resultFound = false;
	owner_t script_id;
};

XeGetScriptFromContextResult XeGetScriptFromContext(XenonEngine* engine, v8::Local<v8::Context> ctx);