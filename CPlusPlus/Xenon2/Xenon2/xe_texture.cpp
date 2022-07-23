#include "xenon.h"
#include <stb_image.h>
void XeTexture::SetImageBuffer(void* pixelData, int width, int height, bool generateMipmaps) {
	if (generateMipmaps)
		mipLevels = static_cast<uint32_t>(std::floor(std::log2(std::max(width, height)))) + 1;
	else
		mipLevels = 1;

	VkBuffer stagingBuffer;
	VkDeviceMemory stagingBufferMemory;
	VkDeviceSize imageSize = width * height * 4;

	XeCreateBuffer(Engine, imageSize, VK_BUFFER_USAGE_TRANSFER_SRC_BIT, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT, stagingBuffer, stagingBufferMemory);

	void* data;
	vkMapMemory(Engine->deviceManager.Device, stagingBufferMemory, 0, imageSize, 0, &data);
	memcpy(data, pixelData, static_cast<size_t>(imageSize));
	vkUnmapMemory(Engine->deviceManager.Device, stagingBufferMemory);

	XeCreateImage(Engine, width, height, mipLevels, VK_SAMPLE_COUNT_1_BIT, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_TILING_OPTIMAL, VK_IMAGE_USAGE_TRANSFER_SRC_BIT | VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT, Image, ImageMemory);

	Engine->utility.transitionImageLayout(Image, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_LAYOUT_UNDEFINED, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, mipLevels);

	XeCopyBufferToImage(Engine, stagingBuffer, Image, width, height);

	vkDestroyBuffer(Engine->deviceManager.Device, stagingBuffer, nullptr);
	vkFreeMemory(Engine->deviceManager.Device, stagingBufferMemory, nullptr);

	ImageView = XeCreateImageView(Engine, Image, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_ASPECT_COLOR_BIT, mipLevels);
	
	XeTexture::generateMipmaps(Image, VK_FORMAT_R8G8B8A8_SRGB, width, height, mipLevels);

	createTextureSampler();
}

void XeTexture::createTextureSampler() {
	VkSamplerCreateInfo samplerInfo{};
	samplerInfo.sType = VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO;
	samplerInfo.magFilter = VK_FILTER_LINEAR;
	samplerInfo.minFilter = VK_FILTER_LINEAR;
	samplerInfo.addressModeU = VK_SAMPLER_ADDRESS_MODE_REPEAT;
	samplerInfo.addressModeV = VK_SAMPLER_ADDRESS_MODE_REPEAT;
	samplerInfo.addressModeW = VK_SAMPLER_ADDRESS_MODE_REPEAT;
	samplerInfo.anisotropyEnable = VK_TRUE;
	VkPhysicalDeviceProperties properties{};
	vkGetPhysicalDeviceProperties(Engine->deviceManager.PhysicalDevice, &properties);
	samplerInfo.maxAnisotropy = properties.limits.maxSamplerAnisotropy;
	samplerInfo.borderColor = VK_BORDER_COLOR_INT_OPAQUE_BLACK;
	samplerInfo.unnormalizedCoordinates = VK_FALSE;
	samplerInfo.compareEnable = VK_FALSE;
	samplerInfo.compareOp = VK_COMPARE_OP_ALWAYS;
	samplerInfo.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
	samplerInfo.mipLodBias = 0.0f;
	samplerInfo.minLod = 0.0f;
	samplerInfo.maxLod = 0.0f;
	samplerInfo.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
	samplerInfo.minLod = 0.0f; // Optional
	samplerInfo.maxLod = static_cast<float>(mipLevels);
	samplerInfo.mipLodBias = 0.0f; // Optional
	if (vkCreateSampler(Engine->deviceManager.Device, &samplerInfo, nullptr, &Sampler) != VK_SUCCESS) {
		throw std::runtime_error("failed to create texture sampler!");
	}
}

void XeTexture::generateMipmaps(VkImage image, VkFormat imageFormat, int32_t texWidth, int32_t texHeight, uint32_t mipLevels, char layerCount) {
	// Check if image format supports linear blitting
	VkFormatProperties formatProperties;
	vkGetPhysicalDeviceFormatProperties(Engine->deviceManager.PhysicalDevice, imageFormat, &formatProperties);

	if (!(formatProperties.optimalTilingFeatures & VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_LINEAR_BIT)) {
		throw std::runtime_error("texture image format does not support linear blitting!");
	}

	XeLinkedCommandBuffer cBuffer = XeBeginCommands(Engine);
	VkCommandBuffer commandBuffer = cBuffer.CommandBuffer;

	VkImageMemoryBarrier barrier{};
	barrier.sType = VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER;
	barrier.image = image;
	barrier.srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED;
	barrier.dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED;
	barrier.subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
	barrier.subresourceRange.baseArrayLayer = 0;
	barrier.subresourceRange.layerCount = layerCount;
	barrier.subresourceRange.levelCount = 1;

	int32_t mipWidth = texWidth;
	int32_t mipHeight = texHeight;

	for (uint32_t i = 1; i < mipLevels; i++) {
		barrier.subresourceRange.baseMipLevel = i - 1;
		barrier.oldLayout = VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL;
		barrier.newLayout = VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL;
		barrier.srcAccessMask = VK_ACCESS_TRANSFER_WRITE_BIT;
		barrier.dstAccessMask = VK_ACCESS_TRANSFER_READ_BIT;

		vkCmdPipelineBarrier(commandBuffer,
			VK_PIPELINE_STAGE_TRANSFER_BIT, VK_PIPELINE_STAGE_TRANSFER_BIT, 0,
			0, nullptr,
			0, nullptr,
			1, &barrier);

		VkImageBlit blit{};
		blit.srcOffsets[0] = { 0, 0, 0 };
		blit.srcOffsets[1] = { mipWidth, mipHeight, 1 };
		blit.srcSubresource.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
		blit.srcSubresource.mipLevel = i - 1;
		blit.srcSubresource.baseArrayLayer = 0;
		blit.srcSubresource.layerCount = 1;
		blit.dstOffsets[0] = { 0, 0, 0 };
		blit.dstOffsets[1] = { mipWidth > 1 ? mipWidth / 2 : 1, mipHeight > 1 ? mipHeight / 2 : 1, 1 };
		blit.dstSubresource.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
		blit.dstSubresource.mipLevel = i;
		blit.dstSubresource.baseArrayLayer = 0;
		blit.dstSubresource.layerCount = 1;

		vkCmdBlitImage(commandBuffer,
			image, VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL,
			image, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
			1, &blit,
			VK_FILTER_LINEAR);

		barrier.oldLayout = VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL;
		barrier.newLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
		barrier.srcAccessMask = VK_ACCESS_TRANSFER_READ_BIT;
		barrier.dstAccessMask = VK_ACCESS_SHADER_READ_BIT;

		vkCmdPipelineBarrier(commandBuffer,
			VK_PIPELINE_STAGE_TRANSFER_BIT, VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT, 0,
			0, nullptr,
			0, nullptr,
			1, &barrier);

		if (mipWidth > 1) mipWidth /= 2;
		if (mipHeight > 1) mipHeight /= 2;
	}

	barrier.subresourceRange.baseMipLevel = mipLevels - 1;
	barrier.oldLayout = VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL;
	barrier.newLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
	barrier.srcAccessMask = VK_ACCESS_TRANSFER_WRITE_BIT;
	barrier.dstAccessMask = VK_ACCESS_SHADER_READ_BIT;

	vkCmdPipelineBarrier(commandBuffer,
		VK_PIPELINE_STAGE_TRANSFER_BIT, VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT, 0,
		0, nullptr,
		0, nullptr,
		1, &barrier);

	XeEndCommands(cBuffer);
}

void XeTexture::Cleanup() {
	vkDestroyImage(XeGetDevice(Engine), Image, nullptr);
	vkDestroyImageView(XeGetDevice(Engine), ImageView, nullptr);
	vkFreeMemory(XeGetDevice(Engine), ImageMemory, nullptr);
	vkDestroySampler(XeGetDevice(Engine), Sampler, nullptr);
}

void XeCubemap::Init() {
	stbi_uc* textureData[6];
	int inittexWidth, inittexHeight, texChannels;
	textureData[0] = stbi_load(Front.c_str(), &inittexWidth, &inittexHeight, &texChannels, 4);

	int texWidth, texHeight;
	textureData[1] = stbi_load(Back.c_str(), &texWidth, &texHeight, &texChannels, 4);
	if (texWidth != inittexWidth || inittexHeight != inittexHeight)
		throw std::runtime_error("Cubemap could not be initialized: images are not the same size: " + Back);

	textureData[2] = stbi_load(Up.c_str(), &texWidth, &texHeight, &texChannels, 4);
	if (texWidth != inittexWidth || inittexHeight != inittexHeight)
		throw std::runtime_error("Cubemap could not be initialized: images are not the same size: " + Up);

	textureData[3] = stbi_load(Down.c_str(), &texWidth, &texHeight, &texChannels, 4);
	if (texWidth != inittexWidth || inittexHeight != inittexHeight)
		throw std::runtime_error("Cubemap could not be initialized: images are not the same size: " + Down);

	textureData[4] = stbi_load(Right.c_str(), &texWidth, &texHeight, &texChannels, 4);
	if (texWidth != inittexWidth || inittexHeight != inittexHeight)
		throw std::runtime_error("Cubemap could not be initialized: images are not the same size: " + Right);

	textureData[5] = stbi_load(Left.c_str(), &texWidth, &texHeight, &texChannels, 4);
	if (texWidth != inittexWidth || inittexHeight != inittexHeight)
		throw std::runtime_error("Cubemap could not be initialized: images are not the same size: " + Left);

	const VkDeviceSize imageSize = texWidth * texHeight * 4 * 6;
	const VkDeviceSize layerSize = imageSize / 6;

	VkBuffer stagingBuffer;
	VkDeviceMemory stagingMemory;

	XeCreateBuffer(this->Engine, imageSize, VK_BUFFER_USAGE_TRANSFER_SRC_BIT, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT, stagingBuffer, stagingMemory);

	void* data;
	
	vkMapMemory(XeGetDevice(this->Engine), stagingMemory, 0, imageSize, 0, &data);
	for (uint8_t i = 0; i < 6; ++i)
	{
		memcpy(static_cast<char*>(data) + (layerSize * i), textureData[i], layerSize);
	}
	XeCreateImage(this->Engine, texWidth, texHeight, 1, VK_SAMPLE_COUNT_1_BIT, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_TILING_OPTIMAL, VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT, Image, ImageMemory, 6);
	


	Engine->utility.transitionImageLayout(Image, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_LAYOUT_UNDEFINED, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, 1, 6);

	XeCopyBufferToImage(Engine, stagingBuffer, Image, texWidth, texHeight);

	XeCubemap::XeTexture::generateMipmaps(Image, VK_FORMAT_R8G8B8A8_SRGB, texWidth, texHeight, 1, 6);

	VkImageViewCreateInfo viewInfo{};
	viewInfo.sType = VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO;
	viewInfo.image = Image;
	viewInfo.viewType = VK_IMAGE_VIEW_TYPE_CUBE;
	viewInfo.format = VK_FORMAT_R8G8B8A8_SRGB;
	viewInfo.subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
	viewInfo.subresourceRange.baseMipLevel = 0;
	viewInfo.subresourceRange.levelCount = 1;
	viewInfo.subresourceRange.baseArrayLayer = 0;
	viewInfo.subresourceRange.layerCount = 6;
	viewInfo.subresourceRange.levelCount = 1;

	VkImageView imageView;
	if (vkCreateImageView(Engine->deviceManager.Device, &viewInfo, nullptr, &imageView) != VK_SUCCESS) {
		throw std::runtime_error("failed to create texture image view for texture cube.");
	}

	ImageView = imageView;

	

	VkSamplerCreateInfo samplerInfo{};
	samplerInfo.sType = VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO;
	samplerInfo.magFilter = VK_FILTER_LINEAR;
	samplerInfo.minFilter = VK_FILTER_LINEAR;
	samplerInfo.addressModeU = VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE;
	samplerInfo.addressModeV = VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE;
	samplerInfo.addressModeW = VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE;
	samplerInfo.anisotropyEnable = VK_TRUE;
	VkPhysicalDeviceProperties properties{};
	vkGetPhysicalDeviceProperties(Engine->deviceManager.PhysicalDevice, &properties);
	samplerInfo.maxAnisotropy = properties.limits.maxSamplerAnisotropy;
	samplerInfo.borderColor = VK_BORDER_COLOR_INT_OPAQUE_WHITE;
	samplerInfo.unnormalizedCoordinates = VK_FALSE;
	samplerInfo.compareEnable = VK_FALSE;
	samplerInfo.compareOp = VK_COMPARE_OP_NEVER;
	samplerInfo.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
	samplerInfo.mipLodBias = 0.0f;
	samplerInfo.minLod = 0.0f;
	samplerInfo.maxLod = 0.0f;
	samplerInfo.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
	samplerInfo.minLod = 0.0f; // Optional
	samplerInfo.maxLod = static_cast<float>(1);
	samplerInfo.mipLodBias = 0.0f; // Optional
	if (vkCreateSampler(Engine->deviceManager.Device, &samplerInfo, nullptr, &Sampler) != VK_SUCCESS) {
		throw std::runtime_error("failed to create texture sampler!");
	}

	vkFreeMemory(XeGetDevice(this->Engine), stagingMemory, nullptr);
	vkDestroyBuffer(XeGetDevice(this->Engine), stagingBuffer, nullptr);
}
void XeCubemap::Cleanup() {
	vkDestroyImage(XeGetDevice(Engine), Image, nullptr);
	vkDestroyImageView(XeGetDevice(Engine), ImageView, nullptr);
	vkFreeMemory(XeGetDevice(Engine), ImageMemory, nullptr);
	vkDestroySampler(XeGetDevice(Engine), Sampler, nullptr);
}
XeCubemap::XeCubemap(std::string* images) : XeTexture() {

	if (sizeof(images) < 6) {
		throw std::runtime_error("Cubemap requires 6 images");
	}
	Front = images[0];
	Back = images[1];
	Up = images[2];
	Down = images[3];
	Right = images[4];
	Left = images[5];
}