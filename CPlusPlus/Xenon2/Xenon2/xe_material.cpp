#include "xenon.h"
#include "xe_descriptor_template.h"

void XeDefaultMaterial::InitializeMaterial() {
	
	auto bindings = std::vector<XeDescriptorTemplate>{
		XE_UNIFORM_BUFFER_ALL,
		XE_IMAGE_SAMPLER,
		XE_UNIFORM_BUFFER_ALL,
		XE_IMAGE_SAMPLER,
		XE_IMAGE_SAMPLER
	};
	auto ly = XeGetLayoutFromTemplate(Engine, bindings);
	DSetLayout = ly;
	DescriptorPool = XeGetDescriptorPoolFromTemplate(Engine, bindings);
	DescriptorSets = XeAllocateDescriptorSets(Engine, ly, DescriptorPool);

	SceneBuffer.Initialize(Engine);
	SceneBuffer.Create();
	MaterialDataBuffer.Initialize(Engine);
	MaterialDataBuffer.Create();

	for (int i = 0; i < DescriptorSets.size(); i++) {
		auto writes = std::vector<VkWriteDescriptorSet>();
		writes.resize(5);

		VkDescriptorBufferInfo sbufferInfo{};
		sbufferInfo.buffer = SceneBuffer.GetBuffer(i);
		sbufferInfo.offset = 0;
		sbufferInfo.range = SceneBuffer.BufferSize;

		VkDescriptorBufferInfo mbufferInfo{};
		mbufferInfo.buffer = MaterialDataBuffer.GetBuffer(i);
		mbufferInfo.offset = 0;
		mbufferInfo.range = MaterialDataBuffer.BufferSize;

		VkDescriptorImageInfo imageInfo{};
		imageInfo.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
		imageInfo.imageView = Diffuse.ImageView;
		imageInfo.sampler = Diffuse.Sampler;

		VkDescriptorImageInfo specInfo{};
		specInfo.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
		specInfo.imageView = Specular.ImageView;
		specInfo.sampler = Specular.Sampler;

		VkDescriptorImageInfo depthInfo{};
		depthInfo.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
		depthInfo.imageView = Engine->renderer.OffScreenPass.OffscreenPass.depth.view;
		depthInfo.sampler = Engine->renderer.OffScreenPass.OffscreenPass.depthSampler;

		writes[0].sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
		writes[0].dstSet = DescriptorSets[i];
		writes[0].dstBinding = 0;
		writes[0].dstArrayElement = 0;
		writes[0].descriptorType = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
		writes[0].descriptorCount = 1;
		writes[0].pBufferInfo = &sbufferInfo;

		writes[1].sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
		writes[1].dstSet = DescriptorSets[i];
		writes[1].dstBinding = 1;
		writes[1].dstArrayElement = 0;
		writes[1].descriptorType = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
		writes[1].descriptorCount = 1;
		writes[1].pImageInfo = &imageInfo;

		writes[2].sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
		writes[2].dstSet = DescriptorSets[i];
		writes[2].dstBinding = 2;
		writes[2].dstArrayElement = 0;
		writes[2].descriptorType = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
		writes[2].descriptorCount = 1;
		writes[2].pBufferInfo = &mbufferInfo;


		writes[3].sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
		writes[3].dstSet = DescriptorSets[i];
		writes[3].dstBinding = 3;
		writes[3].dstArrayElement = 0;
		writes[3].descriptorType = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
		writes[3].descriptorCount = 1;
		writes[3].pImageInfo = &specInfo;

		writes[4].sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
		writes[4].dstSet = DescriptorSets[i];
		writes[4].dstBinding = 4;
		writes[4].dstArrayElement = 0;
		writes[4].descriptorType = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
		writes[4].descriptorCount = 1;
		writes[4].pImageInfo = &depthInfo;


		/*writes[0] = XeBufferToDescriptor(Engine, &SceneBuffer, DescriptorSets[i], i, 0);
		writes[1] = XeTextureToDescriptor(Engine, &Diffuse, DescriptorSets[i], 1);
		writes[2] = XeBufferToDescriptor(Engine, &MaterialDataBuffer, DescriptorSets[i], i, 2);
		*/
		XeWriteDescriptorSets(Engine, writes);
	}
	XeDefaultMaterial::XeMaterial::InitializePipeline();

	for (int i = 0; i < Engine->drawSurface.swapChainImages.size(); i++) {
		MaterialDataBuffer.Update(i, &MaterialData);
	}
}


void XeDynamicMaterial::InitializeMaterial() {
	auto bindings = std::vector<XeDescriptorTemplate>{};
	std::vector<XeBufferItem> boundBuffers;

	bindings.push_back(XE_UNIFORM_BUFFER_ALL);
	SceneBuffer.Initialize(Engine);
	SceneBuffer.Create();
	XeBufferItem item{};
	item.type = XeBufferType::UNIFORM;
	item.internalBuffer = SceneBuffer;
	boundBuffers.push_back(item);


	PLOGV << "Creating buffer bindings for " << MaterialId;
	for (auto buffer : buffers) {
		if (buffer.type == XeBufferType::TEXTURE) {
			bindings.push_back(XE_IMAGE_SAMPLER);
			boundBuffers.push_back(buffer);
			
		}
		else if (buffer.type == XeBufferType::UNIFORM) {
			bindings.push_back(XE_UNIFORM_BUFFER_ALL);
			buffer.internalBuffer.Initialize(Engine);
			buffer.internalBuffer.Create();
			boundBuffers.push_back(buffer);
		}
	}

	PLOGV << "Building descriptor template for " << MaterialId;
	auto ly = XeGetLayoutFromTemplate(Engine, bindings);
	DSetLayout = ly;
	DescriptorPool = XeGetDescriptorPoolFromTemplate(Engine, bindings);
	DescriptorSets = XeAllocateDescriptorSets(Engine, ly, DescriptorPool);
	
	PLOGV << "Building descriptor writes for " << MaterialId;

	for (int i = 0; i < DescriptorSets.size(); i++) {
		auto writes = std::vector<VkWriteDescriptorSet>();

		for (auto buffer : boundBuffers) {
			if (buffer.type == XeBufferType::TEXTURE) {
				VkDescriptorImageInfo imageInfo{};
				imageInfo.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
				imageInfo.imageView = buffer.imageView;
				imageInfo.sampler = buffer.imageSampler;

				VkWriteDescriptorSet writex{};
				writex.sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
				writex.dstSet = DescriptorSets[i];
				writex.dstBinding = writes.size();
				writex.dstArrayElement = 0;
				writex.descriptorType = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
				writex.descriptorCount = 1;
				writex.pImageInfo = &imageInfo;

				writes.push_back(writex);
			}
			if (buffer.type == XeBufferType::UNIFORM) {
				VkDescriptorBufferInfo mbufferInfo{};
				mbufferInfo.buffer = buffer.internalBuffer.GetBuffer(i);
				mbufferInfo.offset = 0;
				mbufferInfo.range = buffer.internalBuffer.BufferSize;


				VkWriteDescriptorSet writex{};
				writex.sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
				writex.dstSet = DescriptorSets[i];
				writex.dstBinding = writes.size() ;
				writex.dstArrayElement = 0;
				writex.descriptorType = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
				writex.descriptorCount = 1;
				writex.pBufferInfo = &mbufferInfo;

				writes.push_back(writex);
			}
		}


		XeWriteDescriptorSets(Engine, writes);
	}
	PLOGV << "Inititalizing material pipeline " << MaterialId;
	XeDynamicMaterial::XeMaterial::InitializePipeline();
}

XeBufferItem* XeDynamicMaterial::InsertBuffer(XeBufferItem buffer) {
	buffers.push_back(buffer);
	return &buffers[buffers.size() - 1];
}

void XeDynamicMaterial::HandleBind(char drawId, VkCommandBuffer bffr) {
	auto sceneData = Engine->scene.GetSceneMaterialData();
	SceneBuffer.Update(static_cast<uint32_t>(drawId), &sceneData);
}


void XeDefaultMaterial::HandleBind(char drawId, VkCommandBuffer bffr) {
	auto sceneData = Engine->scene.GetSceneMaterialData();
	SceneBuffer.Update(static_cast<uint32_t>(drawId), &sceneData);
}

void XeDefaultMaterial::Cleanup() {
	XeDefaultMaterial::XeMaterial::Cleanup();
	Diffuse.Cleanup();
	MaterialDataBuffer.Cleanup();
	SceneBuffer.Cleanup();
}

void XeDynamicMaterial::Cleanup() {
	XeDynamicMaterial::XeMaterial::Cleanup();
	SceneBuffer.Cleanup();
	for (auto buffer : buffers) {
		if (buffer.type == XeBufferType::UNIFORM)
			buffer.internalBuffer.Cleanup();
	}
	for (auto texture : linkedTextures) {
		texture.Cleanup();
	}
}
XeTexture* XeDynamicMaterial::LinkTexture(XeTexture t) {
	linkedTextures.push_back(t);
	auto ptr = &linkedTextures[linkedTextures.size() - 1];
	return ptr;
}

