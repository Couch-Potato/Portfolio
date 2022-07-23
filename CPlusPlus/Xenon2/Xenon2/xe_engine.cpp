#include "xenon.h"
#include "xe_logger.h"
#define DEBUG_MODE true

int XenonEngine::BeginEngine(GLFWwindow* window, int logLevel, const char* module_n) {
	Window = window;
	vkt_log::init(logLevel);
	PLOGI << "Routine Started:: BEFORE INIT";
	BeforeInit();
	PLOGN << "Starting vulkan instance";

	try {
		initForVulkan();
		initMemoryManager();
		initScriptEngine();
	}
	catch (const std::exception& e) {
		PLOGF << "Failed to initialize vulkan: " << e.what();
		return EXIT_FAILURE;
	}
	PLOGI << "Created instance.";
	PLOGI << "Bootstrapping scene...";
	try {
		XeBootstrapScene(this, ConstructInitialScene());
	}
	catch(const std::exception& e) {
		PLOGF <<"Failed to bootstrap scene: "<< e.what();
		return EXIT_FAILURE;
	}
	PLOGI << "Routine Started:: AFTER INIT";
	try {
		if (!DEBUG_MODE)
			AfterInit();
	}
	catch (const std::exception& e) {
		PLOGF << "Fatal error in initialization: " << e.what();
		return EXIT_FAILURE;
	}
	if (DEBUG_MODE)
		AfterInit();
	try {
		//scriptEngine.InitIsolate(module_n);
		XeImportBootstrappables(this);
		//v8::Isolate::Scope isolate_scope(scriptEngine.isolate);
		// Create a stack-allocated handle scope.
		//v8::HandleScope handle_scope(scriptEngine.isolate);
		bootstrapCoreScripts();
		runComponents();
	}
	catch (const std::exception& e) {
		PLOGF << "Fatal error in script engine initialization: " << e.what();
	}
	try {
		mainLoop();
	}
	catch (const std::exception& e) {
		PLOGF << "Fatal error in main loop: " << e.what();
	}

	try {
		cleanup();
	}
	catch (const std::exception& e) {
		PLOGF << "Fatal error in cleanup: " << e.what();
	}
	PLOGI << "Routine Started:: AFTER CLEANUP";
	AfterCleanup();
	return EXIT_SUCCESS;
}

void XenonEngine::Run() {
	throw std::runtime_error("Cannot use the abstract engine. Create your own loser!");
}

void XenonEngine::initForVulkan() {
	PLOGV << "Making instance";
	makeInstance();
	PLOGV << "Setting up debug messengers";
	setupDebugMessenger();
	PLOGV << "Building utility";
	utility.Initialize(this);
	PLOGV << "Creating draw surface";
	drawSurface.InitializeSurface(Window, this);
	PLOGV << "Initializing graphics cards";
	deviceManager.InitializeDevices(this);
	PLOGV << "Creating sawp chain";
	drawSurface.InitializeSwapChain();
	PLOGV << "Initializing renderer";
	renderer.Initialize(this);
	PLOGV << "Buffering command pool";
	InitializeCommandPool();
	PLOGV << "Creating color resources";
	drawSurface.CreateColorResources();
	PLOGV << "Creating framebuffers";
	createFramebuffers();
	PLOGV << "Creating synchronization system";
	createSync();
	
}

void XenonEngine::cleanup() {
	drawSurface.Cleanup();
	deviceManager.Cleanup();
	renderer.Cleanup();
	scene.Cleanup();

	for (size_t i = 0; i < MAX_FRAMES_IN_FLIGHT; i++) {
		vkDestroySemaphore(XeGetDevice(this), renderFinishedSemaphores[i], nullptr);
		vkDestroySemaphore(XeGetDevice(this), imageAvailableSemaphores[i], nullptr);
		vkDestroyFence(XeGetDevice(this), inFlightFences[i], nullptr);
	}

	vkDestroyCommandPool(XeGetDevice(this), commandPool, nullptr);
	DestroyDebugUtilsMessengerEXT(instance, debugMessenger, nullptr);
	vkDestroyInstance(instance, nullptr);
}

void XenonEngine::InitializeCommandPool() {
	QueueFamilyIndices queueFamilyIndices = deviceManager.findQueueFamilies(deviceManager.PhysicalDevice);
	VkCommandPoolCreateInfo poolInfo{};
	poolInfo.sType = VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO;
	poolInfo.queueFamilyIndex = queueFamilyIndices.graphicsFamily.value();
	poolInfo.flags = 0; // Optional
	if (vkCreateCommandPool(deviceManager.Device, &poolInfo, nullptr, &commandPool) != VK_SUCCESS) {
		throw std::runtime_error("failed to create command pool!");
	}
}

void XenonEngine::setupDebugMessenger() {
	if (!enableValidationLayers) return;

	VkDebugUtilsMessengerCreateInfoEXT createInfo;
	populateDebugMessengerCreateInfo(createInfo);

	if (CreateDebugUtilsMessengerEXT(instance, &createInfo, nullptr, &debugMessenger) != VK_SUCCESS) {
		throw std::runtime_error("failed to set up debug messenger!");
	}
}


VkResult XenonEngine::CreateDebugUtilsMessengerEXT(VkInstance instance, const VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo, const VkAllocationCallbacks* pAllocator, VkDebugUtilsMessengerEXT* pDebugMessenger) {
	auto func = (PFN_vkCreateDebugUtilsMessengerEXT)vkGetInstanceProcAddr(instance, "vkCreateDebugUtilsMessengerEXT");
	if (func != nullptr) {
		return func(instance, pCreateInfo, pAllocator, pDebugMessenger);
	}
	else {
		return VK_ERROR_EXTENSION_NOT_PRESENT;
	}
}

void XenonEngine::DestroyDebugUtilsMessengerEXT(VkInstance instance, VkDebugUtilsMessengerEXT debugMessenger, const VkAllocationCallbacks* pAllocator) {
	auto func = (PFN_vkDestroyDebugUtilsMessengerEXT)vkGetInstanceProcAddr(instance, "vkDestroyDebugUtilsMessengerEXT");
	if (func != nullptr) {
		func(instance, debugMessenger, pAllocator);
	}
}

std::vector<const char*> XenonEngine::getRequiredExtensions() {
	uint32_t glfwExtensionCount = 0;
	const char** glfwExtensions;
	glfwExtensions = glfwGetRequiredInstanceExtensions(&glfwExtensionCount);

	std::vector<const char*> extensions(glfwExtensions, glfwExtensions + glfwExtensionCount);

	if (enableValidationLayers) {
		extensions.push_back(VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
	}

	return extensions;
}

bool XenonEngine::checkValidationLayerSupport() {
	uint32_t layerCount;
	vkEnumerateInstanceLayerProperties(&layerCount, nullptr);

	std::vector<VkLayerProperties> availableLayers(layerCount);
	vkEnumerateInstanceLayerProperties(&layerCount, availableLayers.data());
	for (const char* layerName : validationLayers) {
		bool layerFound = false;

		for (const auto& layerProperties : availableLayers) {
			if (strcmp(layerName, layerProperties.layerName) == 0) {
				layerFound = true;
				break;
			}
		}

		if (!layerFound) {
			return false;
		}
	}

	return true;
}

static VKAPI_ATTR VkBool32 VKAPI_CALL debugCallback(
	VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity,
	VkDebugUtilsMessageTypeFlagsEXT messageType,
	const VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
	void* pUserData) {


	if (messageSeverity == VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT) {
		PLOGV << pCallbackData->pMessage;
	}
	if (messageSeverity == VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT) {
		PLOGI << pCallbackData->pMessage;
	}
	if (messageSeverity == VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT) {
		PLOGW << pCallbackData->pMessage;
	}
	if (messageSeverity == VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT) {
		PLOGE << pCallbackData->pMessage;
	}

	return VK_FALSE;
}

void XenonEngine::populateDebugMessengerCreateInfo(VkDebugUtilsMessengerCreateInfoEXT& createInfo) {
	createInfo = {};
	createInfo.sType = VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT;
	createInfo.messageSeverity = VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT;
	createInfo.messageType = VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT;
	createInfo.pfnUserCallback = debugCallback;
}

void XenonEngine::createFramebuffers() {
	swapChainFramebuffers.resize(drawSurface.swapChainImageViews.size());

	for (size_t i = 0; i < drawSurface.swapChainImageViews.size(); i++) {
		std::array<VkImageView, 3> attachments = {
			drawSurface.colorImageView,
			drawSurface.depthImageView,
			drawSurface.swapChainImageViews[i]
		};

		VkFramebufferCreateInfo framebufferInfo{};
		framebufferInfo.sType = VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO;
		framebufferInfo.renderPass = renderer.RenderPass;
		framebufferInfo.attachmentCount = static_cast<uint32_t>(attachments.size());
		framebufferInfo.pAttachments = attachments.data();
		framebufferInfo.width = drawSurface.swapChainExtent.width;
		framebufferInfo.height = drawSurface.swapChainExtent.height;
		framebufferInfo.layers = 1;

		if (vkCreateFramebuffer(XeGetDevice(this), &framebufferInfo, nullptr, &swapChainFramebuffers[i]) != VK_SUCCESS) {
			throw std::runtime_error("failed to create framebuffer!");
		}
	}
}

void XenonEngine::makeInstance() {
	if (enableValidationLayers && !checkValidationLayerSupport()) {
		throw std::runtime_error("validation layers requested, but not available!");
	}

	VkApplicationInfo info{};
	info.sType = VK_STRUCTURE_TYPE_APPLICATION_INFO;
	info.pApplicationName = "Novovu";
	info.applicationVersion = VK_MAKE_VERSION(1, 0, 0);
	info.pEngineName = "Novovu Xenon";
	info.engineVersion = VK_MAKE_VERSION(1, 0, 0);
	info.apiVersion = VK_API_VERSION_1_1;

	VkInstanceCreateInfo createInfo{};
	createInfo.sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO;
	createInfo.pApplicationInfo = &info;

	uint32_t glfwExtensionCount = 0;
	const char** glfwExtensions;

	glfwExtensions = glfwGetRequiredInstanceExtensions(&glfwExtensionCount);

	createInfo.enabledExtensionCount = glfwExtensionCount;
	createInfo.ppEnabledExtensionNames = glfwExtensions;

	auto extensions = getRequiredExtensions();
	createInfo.enabledExtensionCount = static_cast<uint32_t>(extensions.size());
	createInfo.ppEnabledExtensionNames = extensions.data();

	VkDebugUtilsMessengerCreateInfoEXT debugCreateInfo;
	if (enableValidationLayers) {
		createInfo.enabledLayerCount = static_cast<uint32_t>(validationLayers.size());
		createInfo.ppEnabledLayerNames = validationLayers.data();

		populateDebugMessengerCreateInfo(debugCreateInfo);
		createInfo.pNext = (VkDebugUtilsMessengerCreateInfoEXT*)&debugCreateInfo;
	}
	else {
		createInfo.enabledLayerCount = 0;

		createInfo.pNext = nullptr;
	}

	PLOGI << "Creating instance...";

	if (vkCreateInstance(&createInfo, nullptr, &instance) != VK_SUCCESS) {
		PLOGE << "Failed to create vulkan instance";
		throw std::runtime_error("failed to create instance!");
	}

	PLOGI << "Instance created";

	uint32_t extensionCount = 0;
	vkEnumerateInstanceExtensionProperties(nullptr, &extensionCount, nullptr);
	std::vector<VkExtensionProperties> extensionsD(extensionCount);
	vkEnumerateInstanceExtensionProperties(nullptr, &extensionCount, extensionsD.data());

}

void XenonEngine::createSync() {
	imageAvailableSemaphores.resize(MAX_FRAMES_IN_FLIGHT);
	renderFinishedSemaphores.resize(MAX_FRAMES_IN_FLIGHT);
	inFlightFences.resize(MAX_FRAMES_IN_FLIGHT);
	imagesInFlight.resize(drawSurface.swapChainImages.size(), VK_NULL_HANDLE);

	VkSemaphoreCreateInfo semaphoreInfo{};
	semaphoreInfo.sType = VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO;

	VkFenceCreateInfo fenceInfo{};
	fenceInfo.sType = VK_STRUCTURE_TYPE_FENCE_CREATE_INFO;
	fenceInfo.flags = VK_FENCE_CREATE_SIGNALED_BIT;

	for (size_t i = 0; i < MAX_FRAMES_IN_FLIGHT; i++) {
		if (vkCreateSemaphore(XeGetDevice(this), &semaphoreInfo, nullptr, &imageAvailableSemaphores[i]) != VK_SUCCESS ||
			vkCreateSemaphore(XeGetDevice(this), &semaphoreInfo, nullptr, &renderFinishedSemaphores[i]) != VK_SUCCESS ||
			vkCreateFence(XeGetDevice(this), &fenceInfo, nullptr, &inFlightFences[i]) != VK_SUCCESS) {

			throw std::runtime_error("failed to create synchronization objects for a frame!");
		}
	}
}

void XenonEngine::mainLoop() {
	while (!glfwWindowShouldClose(Window)) {
		glfwPollEvents();
		draw();

	}
	vkDeviceWaitIdle(XeGetDevice(this));
}

void XenonEngine::draw() {
	
	vkWaitForFences(XeGetDevice(this), 1, &inFlightFences[currentFrame], VK_TRUE, UINT64_MAX);
	XeGenerateBufferAndDrawScene(&scene);
	uint32_t imageIndex;
	VkResult result = vkAcquireNextImageKHR(XeGetDevice(this), drawSurface.swapChain, UINT64_MAX, imageAvailableSemaphores[currentFrame], VK_NULL_HANDLE, &imageIndex);

	//if (result == VK_ERROR_OUT_OF_DATE_KHR || result == VK_SUBOPTIMAL_KHR || framebufferResized) {
	//	framebufferResized = false;
	////	recreateSwapChain();
	//	return;
	//}
	if (result != VK_SUCCESS && result != VK_SUBOPTIMAL_KHR) {
		throw std::runtime_error("failed to acquire swap chain image!");
	}

	// Check if a previous frame is using this image (i.e. there is its fence to wait on)
	if (imagesInFlight[imageIndex] != VK_NULL_HANDLE) {
		vkWaitForFences(XeGetDevice(this), 1, &imagesInFlight[imageIndex], VK_TRUE, UINT64_MAX);
	}
	// Mark the image as now being in use by this frame
	imagesInFlight[imageIndex] = inFlightFences[currentFrame];




	VkSubmitInfo submitInfo{};
	submitInfo.sType = VK_STRUCTURE_TYPE_SUBMIT_INFO;

	VkSemaphore waitSemaphores[] = { imageAvailableSemaphores[currentFrame] };
	VkPipelineStageFlags waitStages[] = { VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT };
	submitInfo.waitSemaphoreCount = 1;
	submitInfo.pWaitSemaphores = waitSemaphores;
	submitInfo.pWaitDstStageMask = waitStages;
	submitInfo.commandBufferCount = 1;
	submitInfo.pCommandBuffers = scene.GetCommandBuffer(imageIndex);
	VkSemaphore signalSemaphores[] = { renderFinishedSemaphores[currentFrame] };
	submitInfo.signalSemaphoreCount = 1;
	submitInfo.pSignalSemaphores = signalSemaphores;
	vkResetFences(XeGetDevice(this), 1, &inFlightFences[currentFrame]);
	if (vkQueueSubmit(deviceManager.graphicsQueue, 1, &submitInfo, inFlightFences[currentFrame]) != VK_SUCCESS) {
		throw std::runtime_error("failed to submit draw command buffer!");
	}

	VkPresentInfoKHR presentInfo{};
	presentInfo.sType = VK_STRUCTURE_TYPE_PRESENT_INFO_KHR;

	presentInfo.waitSemaphoreCount = 1;
	presentInfo.pWaitSemaphores = signalSemaphores;
	VkSwapchainKHR swapChains[] = { drawSurface.swapChain };
	presentInfo.swapchainCount = 1;
	presentInfo.pSwapchains = swapChains;
	presentInfo.pImageIndices = &imageIndex;
	presentInfo.pResults = nullptr; // Optional
	result = vkQueuePresentKHR(deviceManager.presentQueue, &presentInfo);

	if (result == VK_ERROR_OUT_OF_DATE_KHR || result == VK_SUBOPTIMAL_KHR) {
		//recreateSwapChain();
	}
	else if (result != VK_SUCCESS) {
		throw std::runtime_error("failed to present swap chain image!");
	}
	currentFrame = (currentFrame + 1) % MAX_FRAMES_IN_FLIGHT;
}
void XenonEngine::initMemoryManager() {
	memoryManager.Initialize(this);
}
void XenonEngine::initScriptEngine() {
	scriptEngine.Initialize(this);
}
void XenonEngine::bootstrapCoreScripts() {
	PLOGI << "Starting script engine - bootstrapping core scripts.";
	for (auto const& [key, val] : scriptEngine.coreScripts) {
		PLOGV << "Bootstrapping core script: " << key;
		scriptEngine.coreScripts[key]->Run();
	}
}

void bootstrapComponentEntity(XeMeshedEntity* entity) {
	for (auto const val : entity->cList) {
		PLOGV << "Bootstrapping component: " << val->id << " for entity.";
		val->Run();
	}
}

void XenonEngine::runComponents() {
	PLOGI << "Starting script engine - bootstrapping scene components.";
	for (auto const& [key, val] : scene.components) {
		PLOGV << "Bootstrapping component: " << key << " for scene.";
		val->Run();
	}
	PLOGI << "Starting script engine - bootstrapping entity components.";

	// Be weary about this!!!! Idk if it gets the pointer to the actual object or not!!! I would hope it does....

	for (auto x : scene.entityBuffer) {
		bootstrapComponentEntity(x);
	}
}
