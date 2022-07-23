#include "xe_buffer.h"
XeMultiBuffer::XeMultiBuffer(VkDeviceSize bufferSize) {
	BufferSize = bufferSize;
}
XeMultiBuffer::XeMultiBuffer(VkDeviceSize bufferSize, void* ix) {
	BufferSize = bufferSize;
	initset = ix;
}
void XeMultiBuffer::Create() {
	i_buffers.resize(Engine->drawSurface.swapChainImages.size());
	i_device_memory.resize(Engine->drawSurface.swapChainImages.size());

	for (size_t i = 0; i < Engine->drawSurface.swapChainImages.size(); i++) {
		XeCreateBuffer(Engine, BufferSize, VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT, i_buffers[i], i_device_memory[i]);
		if (initset != NULL) {
			Update(i, initset);
		}
	}
}

void XeMultiBuffer::Cleanup() {
	for (size_t i = 0; i < Engine->drawSurface.swapChainImages.size(); i++) {
		vkDestroyBuffer(Engine->deviceManager.Device, i_buffers[i], nullptr);
		vkFreeMemory(Engine->deviceManager.Device, i_device_memory[i], nullptr);
	}
}
void XeMultiBuffer::Update(uint32_t i, void* Xdata) {
	void* data;
	vkMapMemory(Engine->deviceManager.Device, i_device_memory[i], 0, BufferSize, 0, &data);
	memcpy(data, Xdata, BufferSize);
	vkUnmapMemory(Engine->deviceManager.Device, i_device_memory[i]);
}
VkBuffer XeMultiBuffer::GetBuffer(uint32_t i) {
	return i_buffers[i];
}