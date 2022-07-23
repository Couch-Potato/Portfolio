#pragma once
#include "xenon.h"
#ifndef NO_LOAD_ENGINE 
#include "xe_engine.h"
#endif


class XeTexture: public EngineAttached {
public:

	VkImage Image;
	VkImageView ImageView;
	VkSampler Sampler;
	VkDeviceMemory ImageMemory;
	uint32_t mipLevels;

	void SetImageBuffer(void* pixelData, int width, int height, bool generateMipmaps = false) {
		if (generateMipmaps)
			mipLevels = static_cast<uint32_t>(std::floor(std::log2(std::max(width, height)))) + 1;
		else
			mipLevels = 1;

		VkBuffer stagingBuffer;
		VkBuffer stagingBufferMemory;
		VkDeviceSize imageSize = width * height * 4;

		Engine->utility.CreateBuffer(imageSize, VK_BUFFER_USAGE_TRANSFER_SRC_BIT, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT, stagingBuffer, stagingBufferMemory);

		void* data;
		vkMapMemory(Engine->deviceManager.Device, stagingBufferMemory, 0, imageSize, 0, &data);
		memcpy(data, pixelData, static_cast<size_t>(imageSize));
		vkUnmapMemory(Engine->deviceManager.Device, stagingBufferMemory);

		Engine->utility.createImage(width, height, mipLevels, VK_SAMPLE_COUNT_1_BIT, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_TILING_OPTIMAL, VK_IMAGE_USAGE_TRANSFER_SRC_BIT | VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT, Image, ImageMemory);

		Engine->utility.transitionImageLayout(Image, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_LAYOUT_UNDEFINED, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, mipLevels);

		Engine->utility.copyBufferToImage(stagingBuffer, Image, width, height);

		vkDestroyBuffer(Engine->deviceManager.Device, stagingBuffer, nullptr);
		vkFreeMemory(Engine->deviceManager.Device, stagingBufferMemory, nullptr);

	}

private:
	

};