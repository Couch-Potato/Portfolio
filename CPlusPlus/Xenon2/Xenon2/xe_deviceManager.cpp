#include "xenon.h"
void XeDeviceManager::InitializeDevices(XenonEngine* engine) {
	/*
	Creates the physical device
	*/
	Engine = engine;
	uint32_t deviceCount = 0;
	vkEnumeratePhysicalDevices(engine->instance, &deviceCount, nullptr);
	if (deviceCount == 0) {
		throw std::runtime_error("failed to find GPUs with Vulkan support!");
	}
	std::vector<VkPhysicalDevice> devices(deviceCount);
	vkEnumeratePhysicalDevices(engine->instance, &deviceCount, devices.data());
	for (const auto& device : devices) {
		if (isDeviceSuitable(device)) {
			PhysicalDevice = device;
			
			MSAASamples = getMaxUsableSampleCount();
			break;
		}
	}
	if (PhysicalDevice == VK_NULL_HANDLE) {
		throw std::runtime_error("failed to find a suitable GPU!");
	}

	VkPhysicalDeviceProperties physicalDeviceProperties;
	vkGetPhysicalDeviceProperties(PhysicalDevice, &physicalDeviceProperties);
	PhysicalDeviceName = physicalDeviceProperties.deviceName;
	PhysicalDeviceVersion = physicalDeviceProperties.driverVersion;
	PhysicalDeviceType = physicalDeviceProperties.deviceType;
	PLOGV << "Graphics Card: " << physicalDeviceProperties.deviceName;
	PLOGV << "Driver Version: " << physicalDeviceProperties.driverVersion;
	PLOGV << "Device Type" << physicalDeviceProperties.deviceType;
	/*
	Creates the logical device
	*/

	InitializeLogicalDevice();
}
VkPhysicalDeviceFeatures deviceFeatures{};
const std::vector<const char*> deviceExtensions = {
VK_KHR_SWAPCHAIN_EXTENSION_NAME
};
void XeDeviceManager::InitializeLogicalDevice() {
	QueueFamilyIndices indices = findQueueFamilies(PhysicalDevice);
	std::vector<VkDeviceQueueCreateInfo> queueCreateInfos;
	std::set<uint32_t> uniqueQueueFamilies = { indices.graphicsFamily.value(), indices.presentFamily.value() };

	float queuePriority = 1.0f;
	for (uint32_t queueFamily : uniqueQueueFamilies) {
		VkDeviceQueueCreateInfo queueCreateInfo{};
		queueCreateInfo.sType = VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO;
		queueCreateInfo.queueFamilyIndex = queueFamily;
		queueCreateInfo.queueCount = 1;
		queueCreateInfo.pQueuePriorities = &queuePriority;
		queueCreateInfos.push_back(queueCreateInfo);
		PLOGV << "Got queue family: " << queueFamily;
	}

	VkDeviceCreateInfo createInfo{};
	createInfo.sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO;
	createInfo.queueCreateInfoCount = static_cast<uint32_t>(queueCreateInfos.size());
	createInfo.pQueueCreateInfos = queueCreateInfos.data();
	deviceFeatures.samplerAnisotropy = VK_TRUE;

	createInfo.pEnabledFeatures = &deviceFeatures;
	createInfo.enabledExtensionCount = 0;

	createInfo.enabledExtensionCount = static_cast<uint32_t>(deviceExtensions.size());
	createInfo.ppEnabledExtensionNames = deviceExtensions.data();

	if (Engine->enableValidationLayers) {
		createInfo.enabledLayerCount = static_cast<uint32_t>(Engine->validationLayers.size());
		createInfo.ppEnabledLayerNames = Engine->validationLayers.data();
	}
	else {
		createInfo.enabledLayerCount = 0;
	}
	if (vkCreateDevice(PhysicalDevice, &createInfo, nullptr, &Device) != VK_SUCCESS) {
		throw std::runtime_error("failed to create logical device!");
	}

	PLOGV << "Graphics queue indicies: " << indices.graphicsFamily.value();
	PLOGV << "Presentation queue indicies: " << indices.presentFamily.value();
	vkGetDeviceQueue(Device, indices.graphicsFamily.value(), 0, &graphicsQueue);
	vkGetDeviceQueue(Device, indices.presentFamily.value(), 0, &presentQueue);
}

SwapChainSupportDetails XeDeviceManager::querySwapChainSupport(VkPhysicalDevice device) {
	SwapChainSupportDetails details;

	vkGetPhysicalDeviceSurfaceCapabilitiesKHR(device, Engine->drawSurface.surface, &details.capabilities);


	uint32_t formatCount;
	vkGetPhysicalDeviceSurfaceFormatsKHR(device, Engine->drawSurface.surface, &formatCount, nullptr);

	if (formatCount != 0) {
		details.formats.resize(formatCount);
		vkGetPhysicalDeviceSurfaceFormatsKHR(device, Engine->drawSurface.surface, &formatCount, details.formats.data());
	}

	uint32_t presentModeCount;
	vkGetPhysicalDeviceSurfacePresentModesKHR(device, Engine->drawSurface.surface, &presentModeCount, nullptr);

	if (presentModeCount != 0) {
		details.presentModes.resize(presentModeCount);
		vkGetPhysicalDeviceSurfacePresentModesKHR(device, Engine->drawSurface.surface, &presentModeCount, details.presentModes.data());
	}


	return details;
}

QueueFamilyIndices XeDeviceManager::findQueueFamilies(VkPhysicalDevice device) {
	QueueFamilyIndices indices;

	uint32_t queueFamilyCount = 0;
	vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, nullptr);

	std::vector<VkQueueFamilyProperties> queueFamilies(queueFamilyCount);
	vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, queueFamilies.data());
	int i = 0;
	for (const auto& queueFamily : queueFamilies) {
		if (queueFamily.queueFlags & VK_QUEUE_GRAPHICS_BIT) {
			indices.graphicsFamily = i;
		}
		VkBool32 presentSupport = false;
		vkGetPhysicalDeviceSurfaceSupportKHR(device, i, Engine->drawSurface.surface, &presentSupport);
		if (presentSupport) {
			indices.presentFamily = i;
		}
		if (indices.isComplete()) {
			break;
		}

		i++;
	}

	return indices;
}

	VkSampleCountFlagBits XeDeviceManager::getMaxUsableSampleCount() {
		VkPhysicalDeviceProperties physicalDeviceProperties;
		vkGetPhysicalDeviceProperties(PhysicalDevice, &physicalDeviceProperties);

		VkSampleCountFlags counts = physicalDeviceProperties.limits.framebufferColorSampleCounts & physicalDeviceProperties.limits.framebufferDepthSampleCounts;
		if (counts & VK_SAMPLE_COUNT_64_BIT) { return VK_SAMPLE_COUNT_64_BIT; }
		if (counts & VK_SAMPLE_COUNT_32_BIT) { return VK_SAMPLE_COUNT_32_BIT; }
		if (counts & VK_SAMPLE_COUNT_16_BIT) { return VK_SAMPLE_COUNT_16_BIT; }
		if (counts & VK_SAMPLE_COUNT_8_BIT) { return VK_SAMPLE_COUNT_8_BIT; }
		if (counts & VK_SAMPLE_COUNT_4_BIT) { return VK_SAMPLE_COUNT_4_BIT; }
		if (counts & VK_SAMPLE_COUNT_2_BIT) { return VK_SAMPLE_COUNT_2_BIT; }

		return VK_SAMPLE_COUNT_1_BIT;
	}
	bool XeDeviceManager::isDeviceSuitable(VkPhysicalDevice device) {
		QueueFamilyIndices indices = findQueueFamilies(device);
		bool extensionsSupported = checkDeviceExtensionSupport(device);
		bool swapChainAdequate = false;
		if (extensionsSupported) {
			SwapChainSupportDetails swapChainSupport = querySwapChainSupport(device);
			swapChainAdequate = !swapChainSupport.formats.empty() && !swapChainSupport.presentModes.empty();
		}
		VkPhysicalDeviceFeatures supportedFeatures;
		vkGetPhysicalDeviceFeatures(device, &supportedFeatures);

		return indices.isComplete() && extensionsSupported && swapChainAdequate && supportedFeatures.samplerAnisotropy;
	}


	bool XeDeviceManager::checkDeviceExtensionSupport(VkPhysicalDevice device) {
		uint32_t extensionCount;
		vkEnumerateDeviceExtensionProperties(device, nullptr, &extensionCount, nullptr);

		std::vector<VkExtensionProperties> availableExtensions(extensionCount);
		vkEnumerateDeviceExtensionProperties(device, nullptr, &extensionCount, availableExtensions.data());

		std::set<std::string> requiredExtensions(deviceExtensions.begin(), deviceExtensions.end());

		for (const auto& extension : availableExtensions) {
			requiredExtensions.erase(extension.extensionName);
		}

		return requiredExtensions.empty();
	}

	void XeDeviceManager::Cleanup() {
		vkDestroyDevice(Device, nullptr);
	}