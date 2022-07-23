#include "xenon.h"
void XeCreateBuffer(XenonEngine* engine, VkDeviceSize size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties, VkBuffer& buffer, VkDeviceMemory& bufferMemory) {
	engine->utility.createBuffer(size, usage, properties, buffer, bufferMemory);
}
void XeCopyBufferToImage(XenonEngine* engine, VkBuffer buffer, VkImage image, uint32_t width, uint32_t height) {
	engine->utility.copyBufferToImage(buffer, image, width, height);
}
void XeCreateImage(XenonEngine* engine, uint32_t width, uint32_t height, uint32_t mipLevels, VkSampleCountFlagBits numSamples, VkFormat format, VkImageTiling tiling, VkImageUsageFlags usage, VkMemoryPropertyFlags properties, VkImage& image, VkDeviceMemory& imageMemory, char arrayLayers) {
	engine->utility.createImage(width, height, mipLevels, numSamples, format, tiling, usage, properties, image, imageMemory, arrayLayers);
}
VkImageView XeCreateImageView(XenonEngine* engine, VkImage image, VkFormat format, VkImageAspectFlags aspectFlags, uint32_t mipLevels) {
	return engine->utility.createImageView(image, format, aspectFlags, mipLevels);
}
XeLinkedCommandBuffer XeBeginCommands(XenonEngine* engine) {
	XeLinkedCommandBuffer command{};
	command.CommandBuffer = engine->utility.beginSingleTimeCommands();
	command.LinkedEngine = engine;
	return command;
}
void XeEndCommands(XeLinkedCommandBuffer buf) {
	buf.LinkedEngine->utility.endSingleTimeCommands(buf.CommandBuffer);
}