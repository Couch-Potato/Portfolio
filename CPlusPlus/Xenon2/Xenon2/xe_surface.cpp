#include "xenon.h"

void XeSurface::InitializeSurface(GLFWwindow* window, XenonEngine* engine) {
	Initialize(engine);
	if (glfwCreateWindowSurface(engine->instance, window, nullptr, &surface) != VK_SUCCESS) {
		throw std::runtime_error("failed to create window surface!");
	}
}

void XeSurface::CreateColorResources() {
	// COLOR
	VkFormat colorFormat = swapChainImageFormat;
	XeCreateImage(Engine, swapChainExtent.width, swapChainExtent.height, 1, Engine->deviceManager.MSAASamples, colorFormat, VK_IMAGE_TILING_OPTIMAL, VK_IMAGE_USAGE_TRANSIENT_ATTACHMENT_BIT | VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT, colorImage, colorImageMemory);
	colorImageView = XeCreateImageView(Engine, colorImage, colorFormat, VK_IMAGE_ASPECT_COLOR_BIT, 1);

	// DEPTH
	VkFormat depthFormat = Engine->renderer.findDepthFormat();
	XeCreateImage(Engine, swapChainExtent.width, swapChainExtent.height, 1, Engine->deviceManager.MSAASamples, depthFormat, VK_IMAGE_TILING_OPTIMAL, VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT, depthImage, depthImageMemory);
	depthImageView = XeCreateImageView(Engine, depthImage, depthFormat, VK_IMAGE_ASPECT_DEPTH_BIT, 1);
	XeTransitionImageLayout(Engine, depthImage, depthFormat, VK_IMAGE_LAYOUT_UNDEFINED, VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL, 1);

}

void XeSurface::InitializeSwapChain() {
	SwapChainSupportDetails swapChainSupport = Engine->deviceManager.querySwapChainSupport(Engine->deviceManager.PhysicalDevice);

	VkSurfaceFormatKHR surfaceFormat = chooseSwapSurfaceFormat(swapChainSupport.formats);
	VkPresentModeKHR presentMode = chooseSwapPresentMode(swapChainSupport.presentModes);
	VkExtent2D extent = chooseSwapExtent(swapChainSupport.capabilities);
	swapChainExtent = extent;
	uint32_t imageCount = swapChainSupport.capabilities.minImageCount + 1;
	if (swapChainSupport.capabilities.maxImageCount > 0 && imageCount > swapChainSupport.capabilities.maxImageCount) {
		imageCount = swapChainSupport.capabilities.maxImageCount;
	}
	VkSwapchainCreateInfoKHR createInfo{};
	createInfo.sType = VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR;
	createInfo.surface = surface;
	createInfo.minImageCount = imageCount;
	createInfo.imageFormat = surfaceFormat.format;
	createInfo.imageColorSpace = surfaceFormat.colorSpace;
	createInfo.imageExtent = extent;
	createInfo.imageArrayLayers = 1;
	createInfo.imageUsage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT;
	QueueFamilyIndices indices = Engine->deviceManager.findQueueFamilies(Engine->deviceManager.PhysicalDevice);
	uint32_t queueFamilyIndices[] = { indices.graphicsFamily.value(), indices.presentFamily.value() };

	if (indices.graphicsFamily != indices.presentFamily) {
		createInfo.imageSharingMode = VK_SHARING_MODE_CONCURRENT;
		createInfo.queueFamilyIndexCount = 2;
		createInfo.pQueueFamilyIndices = queueFamilyIndices;
	}
	else {
		createInfo.imageSharingMode = VK_SHARING_MODE_EXCLUSIVE;
		createInfo.queueFamilyIndexCount = 0; // Optional
		createInfo.pQueueFamilyIndices = nullptr; // Optional
	}
	createInfo.preTransform = swapChainSupport.capabilities.currentTransform;
	createInfo.compositeAlpha = VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR;
	createInfo.presentMode = presentMode;
	createInfo.clipped = VK_TRUE;
	createInfo.oldSwapchain = VK_NULL_HANDLE;
	if (vkCreateSwapchainKHR(Engine->deviceManager.Device, &createInfo, nullptr, &swapChain) != VK_SUCCESS) {
		throw std::runtime_error("failed to create swap chain!");
	}
	vkGetSwapchainImagesKHR(Engine->deviceManager.Device, swapChain, &imageCount, nullptr);
	swapChainImages.resize(imageCount);
	vkGetSwapchainImagesKHR(Engine->deviceManager.Device, swapChain, &imageCount, swapChainImages.data());
	createImageViews();
}
	void XeSurface::createImageViews() {
		swapChainImageViews.resize(swapChainImages.size());

		for (uint32_t i = 0; i < swapChainImages.size(); i++) {
			swapChainImageViews[i] = XeCreateImageView(Engine, swapChainImages[i], swapChainImageFormat, VK_IMAGE_ASPECT_COLOR_BIT, 1);
		}
	}
	VkSurfaceFormatKHR XeSurface::chooseSwapSurfaceFormat(const std::vector<VkSurfaceFormatKHR>& availableFormats) {
		for (const auto& availableFormat : availableFormats) {
			if (availableFormat.format == VK_FORMAT_B8G8R8A8_SRGB && availableFormat.colorSpace == VK_COLOR_SPACE_SRGB_NONLINEAR_KHR) {
				return availableFormat;
			}
		}
		return availableFormats[0];
	}
	VkPresentModeKHR XeSurface::chooseSwapPresentMode(const std::vector<VkPresentModeKHR>& availablePresentModes) {
		for (const auto& availablePresentMode : availablePresentModes) {
			if (availablePresentMode == VK_PRESENT_MODE_MAILBOX_KHR) {
				return availablePresentMode;
			}
		}

		return VK_PRESENT_MODE_FIFO_KHR;
	}
	VkExtent2D XeSurface::chooseSwapExtent(const VkSurfaceCapabilitiesKHR& capabilities) {
		if (capabilities.currentExtent.width != UINT32_MAX) {
			return capabilities.currentExtent;
		}
		else {
			int width, height;
			glfwGetFramebufferSize(Engine->Window, &width, &height);

			VkExtent2D actualExtent = {
				static_cast<uint32_t>(width),
				static_cast<uint32_t>(height)
			};

			actualExtent.width = std::clamp(actualExtent.width, capabilities.minImageExtent.width, capabilities.maxImageExtent.width);
			actualExtent.height = std::clamp(actualExtent.height, capabilities.minImageExtent.height, capabilities.maxImageExtent.height);

			return actualExtent;
		}
	}

	void XeSurface::Cleanup() {
		vkDestroyImageView(XeGetDevice(Engine), colorImageView, nullptr);
		vkDestroyImage(XeGetDevice(Engine), colorImage, nullptr);
		vkFreeMemory(XeGetDevice(Engine), colorImageMemory, nullptr);
		for (size_t i = 0; i < Engine->swapChainFramebuffers.size(); i++) {
			vkDestroyFramebuffer(XeGetDevice(Engine), Engine->swapChainFramebuffers[i], nullptr);
		}
		
		for (size_t i = 0; i < swapChainImageViews.size(); i++) {
			vkDestroyImageView(XeGetDevice(Engine), swapChainImageViews[i], nullptr);
		}
		vkDestroySwapchainKHR(XeGetDevice(Engine), swapChain, nullptr);
		vkDestroySurfaceKHR(Engine->instance, surface, nullptr);
	}