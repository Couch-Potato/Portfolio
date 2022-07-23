#pragma once
#include "xe_descriptor_template.h"

XeDescriptorTemplate XE_IMAGE_SAMPLER{ VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER , VK_SHADER_STAGE_FRAGMENT_BIT };
XeDescriptorTemplate XE_UNIFORM_BUFFER_FRAGMENT{ VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER , VK_SHADER_STAGE_FRAGMENT_BIT };
XeDescriptorTemplate XE_UNIFORM_BUFFER_VERTEX{ VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER , VK_SHADER_STAGE_VERTEX_BIT };
XeDescriptorTemplate XE_UNIFORM_BUFFER_ALL{ VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER , VK_SHADER_STAGE_ALL };

VkDescriptorSetLayout XeGetLayoutFromTemplate(XenonEngine* engine, std::vector<XeDescriptorTemplate> bindings) {
	std::vector<VkDescriptorSetLayoutBinding> inner_bindings;
	inner_bindings.resize(bindings.size());
	for (size_t i = 0; i < bindings.size(); i++) {
		VkDescriptorSetLayoutBinding dynBinding{};
		dynBinding.binding = i;
		dynBinding.descriptorType = bindings[i].DescriptorType;
		dynBinding.descriptorCount = 1;
		dynBinding.stageFlags = bindings[i].Stages;
		dynBinding.pImmutableSamplers = nullptr;
		inner_bindings[i] = dynBinding;
	}
	VkDescriptorSetLayoutCreateInfo layoutInfo{};
	layoutInfo.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO;
	layoutInfo.bindingCount = static_cast<uint32_t>(inner_bindings.size());
	layoutInfo.pBindings = inner_bindings.data();

	VkDescriptorSetLayout descriptorLayout;

	if (vkCreateDescriptorSetLayout(engine->deviceManager.Device, &layoutInfo, nullptr, &descriptorLayout) != VK_SUCCESS) {
		throw std::runtime_error("failed to create descriptor set layout!");
	}

	return descriptorLayout;
}
VkDescriptorPool XeGetDescriptorPoolFromTemplate(XenonEngine* engine, std::vector<XeDescriptorTemplate> bindings) {
	std::vector<VkDescriptorPoolSize> poolSizes{};
	poolSizes.resize(bindings.size());
	for (size_t i = 0; i < bindings.size(); i++) {
		poolSizes[i] = VkDescriptorPoolSize{};
		poolSizes[i].type = bindings[i].DescriptorType;
		poolSizes[i].descriptorCount = static_cast<uint32_t>(engine->drawSurface.swapChainImages.size());
	}

	VkDescriptorPoolCreateInfo poolInfo{};
	poolInfo.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO;
	poolInfo.poolSizeCount = static_cast<uint32_t>(poolSizes.size());
	poolInfo.pPoolSizes = poolSizes.data();
	poolInfo.maxSets = static_cast<uint32_t>(engine->drawSurface.swapChainImages.size());

	VkDescriptorPool descriptorPool;

	if (vkCreateDescriptorPool(engine->deviceManager.Device, &poolInfo, nullptr, &descriptorPool) != VK_SUCCESS) {
		throw std::runtime_error("failed to create descriptor pool!");
	}

	return descriptorPool;
}



VkWriteDescriptorSet XeTextureToDescriptor(XenonEngine* engine, XeTexture* texture,VkDescriptorSet dest, int bindingId) {
	VkDescriptorImageInfo imageInfo{};
	imageInfo.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
	imageInfo.imageView = texture->ImageView;
	imageInfo.sampler = texture->Sampler;

	VkWriteDescriptorSet set{};
	set.sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
	set.dstSet = dest;
	set.dstArrayElement = 0;
	set.dstBinding = bindingId;
	set.descriptorType = XE_IMAGE_SAMPLER.DescriptorType;
	set.descriptorCount = 1;
	set.pImageInfo = &imageInfo;

	return set;
}
VkWriteDescriptorSet XeBufferToDescriptor(XenonEngine* engine, XeMultiBuffer* buffer, VkDescriptorSet dest, uint32_t swapChainImageId, int bindingId) {
	VkDescriptorBufferInfo bufferInfo{};
	bufferInfo.buffer = buffer->GetBuffer(swapChainImageId);
	bufferInfo.offset = 0;
	bufferInfo.range = buffer->BufferSize;

	VkWriteDescriptorSet set{};
	set.sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
	set.dstSet = dest;
	set.dstArrayElement = 0;
	set.dstBinding = bindingId;
	set.descriptorType = XE_UNIFORM_BUFFER_ALL.DescriptorType;
	set.descriptorCount = 1;
	set.pBufferInfo = &bufferInfo;

	return set;
}
std::vector<VkDescriptorSet> XeAllocateDescriptorSets(XenonEngine* engine, VkDescriptorSetLayout layout, VkDescriptorPool pool) {
	std::vector<VkDescriptorSetLayout> layouts(engine->drawSurface.swapChainImages.size(), layout);
	VkDescriptorSetAllocateInfo allocInfo{};
	allocInfo.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO;
	allocInfo.descriptorPool = pool;
	allocInfo.descriptorSetCount = static_cast<uint32_t>(engine->drawSurface.swapChainImages.size());
	allocInfo.pSetLayouts = layouts.data();
	std::vector<VkDescriptorSet> descriptorSets;
	descriptorSets.resize(engine->drawSurface.swapChainImages.size());
	if (vkAllocateDescriptorSets(XeGetDevice(engine), &allocInfo, descriptorSets.data()) != VK_SUCCESS) {
		throw std::runtime_error("failed to allocate descriptor sets!");
	}
	return descriptorSets;
}
void XeWriteDescriptorSets(XenonEngine* engine, std::vector<VkWriteDescriptorSet>writes) {
	try {
		vkUpdateDescriptorSets(XeGetDevice(engine), writes.size(), writes.data(), 0, nullptr);
	}
	catch (std::exception what){
		throw std::runtime_error("Failed to write descriptor set. See above for validation errors.");
	}
	
}
