#include "xenon.h"
void XeRenderer::InitializeRenderer() {
	VkAttachmentDescription colorAttachment{};
	colorAttachment.format = Engine->drawSurface.swapChainImageFormat;
	colorAttachment.samples = Engine->deviceManager.MSAASamples;
	colorAttachment.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;
	colorAttachment.storeOp = VK_ATTACHMENT_STORE_OP_STORE;
	colorAttachment.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
	colorAttachment.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
	colorAttachment.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
	colorAttachment.finalLayout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL;

	VkAttachmentDescription depthAttachment{};
	depthAttachment.format = findDepthFormat();
	depthAttachment.samples = Engine->deviceManager.MSAASamples;
	depthAttachment.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;
	depthAttachment.storeOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
	depthAttachment.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
	depthAttachment.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
	depthAttachment.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
	depthAttachment.finalLayout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL;

	VkAttachmentDescription colorAttachmentResolve{};
	colorAttachmentResolve.format = Engine->drawSurface.swapChainImageFormat;
	colorAttachmentResolve.samples = VK_SAMPLE_COUNT_1_BIT;
	colorAttachmentResolve.loadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
	colorAttachmentResolve.storeOp = VK_ATTACHMENT_STORE_OP_STORE;
	colorAttachmentResolve.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
	colorAttachmentResolve.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
	colorAttachmentResolve.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
	colorAttachmentResolve.finalLayout = VK_IMAGE_LAYOUT_PRESENT_SRC_KHR;

	VkAttachmentReference colorAttachmentRef{};
	colorAttachmentRef.attachment = 0;
	colorAttachmentRef.layout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL;

	VkAttachmentReference depthAttachmentRef{};
	depthAttachmentRef.attachment = 1;
	depthAttachmentRef.layout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL;

	VkAttachmentReference colorAttachmentResolveRef{};
	colorAttachmentResolveRef.attachment = 2;
	colorAttachmentResolveRef.layout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL;

	VkSubpassDescription subpass{};
	subpass.pipelineBindPoint = VK_PIPELINE_BIND_POINT_GRAPHICS;
	subpass.colorAttachmentCount = 1;
	subpass.pColorAttachments = &colorAttachmentRef;
	subpass.pDepthStencilAttachment = &depthAttachmentRef;
	subpass.pResolveAttachments = &colorAttachmentResolveRef;

	VkSubpassDependency dependency{};
	dependency.srcSubpass = VK_SUBPASS_EXTERNAL;
	dependency.dstSubpass = 0;
	dependency.srcStageMask = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
	dependency.srcAccessMask = 0;
	dependency.dstStageMask = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
	dependency.dstAccessMask = VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT | VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT;

	std::array<VkAttachmentDescription, 3> attachments = { colorAttachment, depthAttachment, colorAttachmentResolve };
	VkRenderPassCreateInfo renderPassInfo{};
	renderPassInfo.sType = VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO;
	renderPassInfo.attachmentCount = static_cast<uint32_t>(attachments.size());
	renderPassInfo.pAttachments = attachments.data();
	renderPassInfo.subpassCount = 1;
	renderPassInfo.pSubpasses = &subpass;
	renderPassInfo.dependencyCount = 1;
	renderPassInfo.pDependencies = &dependency;

	if (vkCreateRenderPass(Engine->deviceManager.Device, &renderPassInfo, nullptr, &RenderPass) != VK_SUCCESS) {
		throw std::runtime_error("failed to create render pass!");
	}
}
VkFormat XeRenderer::findDepthFormat() {
	return findSupportedFormat(
		{ VK_FORMAT_D32_SFLOAT, VK_FORMAT_D32_SFLOAT_S8_UINT, VK_FORMAT_D24_UNORM_S8_UINT, VK_FORMAT_D16_UNORM },
		VK_IMAGE_TILING_OPTIMAL,
		VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT
	);
}
VkFormat XeRenderer::findSupportedFormat(const std::vector<VkFormat>& candidates, VkImageTiling tiling, VkFormatFeatureFlags features) {
	for (VkFormat format : candidates) {
		VkFormatProperties props;
		vkGetPhysicalDeviceFormatProperties(Engine->deviceManager.PhysicalDevice, format, &props);
		if (tiling == VK_IMAGE_TILING_LINEAR && (props.linearTilingFeatures & features) == features) {
			return format;
		}
		else if (tiling == VK_IMAGE_TILING_OPTIMAL && (props.optimalTilingFeatures & features) == features) {
			return format;
		}
	}
	throw std::runtime_error("failed to find supported format!");
}

void XeRenderer::Cleanup() {
	vkDestroyRenderPass(XeGetDevice(Engine), RenderPass, nullptr);
}

void XeOffScreen::Init() {
	OffscreenPass.width = SHADOWMAP_DIM;
	OffscreenPass.height = SHADOWMAP_DIM;

	VkImageCreateInfo image{};
	image.imageType = VK_IMAGE_TYPE_2D;
	image.extent.width = OffscreenPass.width;
	image.extent.height = OffscreenPass.height;
	image.extent.depth = 1;
	image.mipLevels = 1;
	image.arrayLayers = 1;
	image.samples = VK_SAMPLE_COUNT_1_BIT;
	image.tiling = VK_IMAGE_TILING_OPTIMAL;
	image.format = Engine->renderer.findDepthFormat();
	image.sType = VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO;
	image.usage = VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT | VK_IMAGE_USAGE_SAMPLED_BIT;

	PLOGV << "Preparing offscreen image";

	if (vkCreateImage(XeGetDevice(this), &image, nullptr, &OffscreenPass.depth.image) != VK_SUCCESS) {
		throw std::runtime_error("Failed to prepare offscreen image");
	}

	VkMemoryAllocateInfo memAlloc = VkMemoryAllocateInfo{};
	memAlloc.sType = VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO;
	VkMemoryRequirements memReqs;

	PLOGV << "Getting memory requirements for shadow buffer";

	vkGetImageMemoryRequirements(XeGetDevice(this), OffscreenPass.depth.image, &memReqs);
	memAlloc.allocationSize = memReqs.size;
	memAlloc.memoryTypeIndex = Engine->utility.findMemoryType(memReqs.memoryTypeBits, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);
	PLOGV << memReqs.size << "bytes required";

	memAlloc.allocationSize = memReqs.size;
	vkAllocateMemory(XeGetDevice(this), &memAlloc, nullptr, &OffscreenPass.depth.mem);
	vkBindImageMemory(XeGetDevice(this), OffscreenPass.depth.image, OffscreenPass.depth.mem, 0);

	PLOGV << "Bound depth image memory";

	VkImageViewCreateInfo depthStencilView = VkImageViewCreateInfo{};
	depthStencilView.sType = VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO;
	depthStencilView.viewType = VK_IMAGE_VIEW_TYPE_2D;
	depthStencilView.format = Engine->renderer.findDepthFormat();
	depthStencilView.subresourceRange = {};
	depthStencilView.subresourceRange.aspectMask = VK_IMAGE_ASPECT_DEPTH_BIT;
	depthStencilView.subresourceRange.baseMipLevel = 0;
	depthStencilView.subresourceRange.levelCount = 1;
	depthStencilView.subresourceRange.baseArrayLayer = 0;
	depthStencilView.subresourceRange.layerCount = 1;
	depthStencilView.image = OffscreenPass.depth.image;

	PLOGV << "Creating image view";
	if (vkCreateImageView(XeGetDevice(this), &depthStencilView, nullptr, &OffscreenPass.depth.view) != VK_SUCCESS) {
		throw std::runtime_error("failed to create an image view for the depth pass!");
	}

	VkSamplerCreateInfo sampler = VkSamplerCreateInfo{};
	sampler.sType = VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO;
	sampler.magFilter = DEFAULT_SHADOWMAP_FILTER;
	sampler.minFilter = DEFAULT_SHADOWMAP_FILTER;
	sampler.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
	sampler.addressModeU = VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE;
	sampler.addressModeV = sampler.addressModeU;
	sampler.addressModeW = sampler.addressModeU;
	sampler.mipLodBias = 0.0f;
	sampler.maxAnisotropy = 1.0f;
	sampler.minLod = 0.0f;
	sampler.maxLod = 1.0f;
	sampler.borderColor = VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE;

	PLOGV << "Creating sampler for depth image"
		;
	if (vkCreateSampler(Engine->deviceManager.Device, &sampler, nullptr, &OffscreenPass.depthSampler) != VK_SUCCESS) {
		throw std::runtime_error("failed to create texture sampler for depth pass!");
	}

	PLOGV << "Preparing renderpass for depth buffer";

	prepareRenderpass();

	VkFramebufferCreateInfo fbufCreateInfo = VkFramebufferCreateInfo{};
	fbufCreateInfo.sType = VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO;
	fbufCreateInfo.renderPass = OffscreenPass.renderPass;
	fbufCreateInfo.attachmentCount = 1;
	fbufCreateInfo.pAttachments = &OffscreenPass.depth.view;
	fbufCreateInfo.width = OffscreenPass.width;
	fbufCreateInfo.height = OffscreenPass.height;
	fbufCreateInfo.layers = 1;

	PLOGV << "Creating framebuffer";

	if (vkCreateFramebuffer(XeGetDevice(this), &fbufCreateInfo, nullptr, &OffscreenPass.frameBuffer) != VK_SUCCESS) {
		throw std::runtime_error("failed to create framebuffer for depth pass!");
	}
}

void XeOffScreen::prepareRenderpass() {
	VkAttachmentDescription attachmentDescription{};
	attachmentDescription.format = Engine->renderer.findDepthFormat();
	attachmentDescription.samples = VK_SAMPLE_COUNT_1_BIT;
	attachmentDescription.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;							// Clear depth at beginning of the render pass
	attachmentDescription.storeOp = VK_ATTACHMENT_STORE_OP_STORE;						// We will read from depth, so it's important to store the depth attachment results
	attachmentDescription.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
	attachmentDescription.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
	attachmentDescription.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;					// We don't care about initial layout of the attachment
	attachmentDescription.finalLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;// Attachment will be transitioned to shader read at render pass end

	VkAttachmentReference depthReference = {};
	depthReference.attachment = 0;
	depthReference.layout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL;			// Attachment will be used as depth/stencil during render pass

	VkSubpassDescription subpass = {};
	subpass.pipelineBindPoint = VK_PIPELINE_BIND_POINT_GRAPHICS;
	subpass.colorAttachmentCount = 0;													// No color attachments
	subpass.pDepthStencilAttachment = &depthReference;									// Reference to our depth attachment

	// Use subpass dependencies for layout transitions
	std::array<VkSubpassDependency, 2> dependencies;

	dependencies[0].srcSubpass = VK_SUBPASS_EXTERNAL;
	dependencies[0].dstSubpass = 0;
	dependencies[0].srcStageMask = VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT;
	dependencies[0].dstStageMask = VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
	dependencies[0].srcAccessMask = VK_ACCESS_SHADER_READ_BIT;
	dependencies[0].dstAccessMask = VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT;
	dependencies[0].dependencyFlags = VK_DEPENDENCY_BY_REGION_BIT;

	dependencies[1].srcSubpass = 0;
	dependencies[1].dstSubpass = VK_SUBPASS_EXTERNAL;
	dependencies[1].srcStageMask = VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT;
	dependencies[1].dstStageMask = VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT;
	dependencies[1].srcAccessMask = VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT;
	dependencies[1].dstAccessMask = VK_ACCESS_SHADER_READ_BIT;
	dependencies[1].dependencyFlags = VK_DEPENDENCY_BY_REGION_BIT;

	VkRenderPassCreateInfo renderPassCreateInfo = VkRenderPassCreateInfo{};
	renderPassCreateInfo.sType = VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO;
	renderPassCreateInfo.attachmentCount = 1;
	renderPassCreateInfo.pAttachments = &attachmentDescription;
	renderPassCreateInfo.subpassCount = 1;
	renderPassCreateInfo.pSubpasses = &subpass;
	renderPassCreateInfo.dependencyCount = static_cast<uint32_t>(dependencies.size());
	renderPassCreateInfo.pDependencies = dependencies.data();

	if (vkCreateRenderPass(XeGetDevice(this), &renderPassCreateInfo, nullptr, &OffscreenPass.renderPass) != VK_SUCCESS) {
		throw std::runtime_error("Could not create renderpass for depth buffer");

	}

	
}

//void XeOffScreen::preparePipeline() {
//
//	VkPipelineInputAssemblyStateCreateInfo inputAssembly{};
//	inputAssembly.sType = VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO;
//	inputAssembly.topology = VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST;
//	inputAssembly.primitiveRestartEnable = VK_FALSE;
//
//	VkViewport viewport{};
//	viewport.x = 0.0f;
//	viewport.y = 0.0f;
//	viewport.width = (float)Engine->drawSurface.swapChainExtent.width;
//	viewport.height = (float)Engine->drawSurface.swapChainExtent.height;
//	viewport.minDepth = 0.0f;
//	viewport.maxDepth = 1.0f;
//
//	VkRect2D scissor{};
//	scissor.offset = { 0, 0 };
//	scissor.extent = Engine->drawSurface.swapChainExtent;
//
//	VkPipelineViewportStateCreateInfo viewportState{};
//	viewportState.sType = VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO;
//	viewportState.viewportCount = 1;
//	viewportState.pViewports = &viewport;
//	viewportState.scissorCount = 1;
//	viewportState.pScissors = &scissor;
//
//	VkPipelineRasterizationStateCreateInfo rasterizer{};
//	rasterizer.sType = VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO;
//	rasterizer.depthClampEnable = VK_FALSE;
//	rasterizer.rasterizerDiscardEnable = VK_FALSE;
//	rasterizer.polygonMode = VK_POLYGON_MODE_FILL;
//	rasterizer.lineWidth = 1.0f;
//	rasterizer.cullMode = VK_CULL_MODE_BACK_BIT;
//	rasterizer.frontFace = VK_FRONT_FACE_CLOCKWISE;
//	rasterizer.depthBiasEnable = VK_FALSE;
//	rasterizer.depthBiasConstantFactor = 0.0f; // Optional
//	rasterizer.depthBiasClamp = 0.0f; // Optional
//	rasterizer.depthBiasSlopeFactor = 0.0f; // Option
//
//
//
//	VkPipelineMultisampleStateCreateInfo multisampling{};
//	multisampling.sType = VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO;
//	multisampling.sampleShadingEnable = VK_FALSE;
//	multisampling.rasterizationSamples = VK_SAMPLE_COUNT_1_BIT;
//	multisampling.minSampleShading = 1.0f; // Optional
//	multisampling.pSampleMask = nullptr; // Optional
//	multisampling.alphaToCoverageEnable = VK_FALSE; // Optional
//	multisampling.alphaToOneEnable = VK_FALSE; // Optional
//
//	VkPipelineColorBlendAttachmentState colorBlendAttachment{};
//	colorBlendAttachment.colorWriteMask = VK_COLOR_COMPONENT_R_BIT | VK_COLOR_COMPONENT_G_BIT | VK_COLOR_COMPONENT_B_BIT | VK_COLOR_COMPONENT_A_BIT;
//	colorBlendAttachment.blendEnable = VK_FALSE;
//	colorBlendAttachment.srcColorBlendFactor = VK_BLEND_FACTOR_ONE; // Optional
//	colorBlendAttachment.dstColorBlendFactor = VK_BLEND_FACTOR_ZERO; // Optional
//	colorBlendAttachment.colorBlendOp = VK_BLEND_OP_ADD; // Optional
//	colorBlendAttachment.srcAlphaBlendFactor = VK_BLEND_FACTOR_ONE; // Optional
//	colorBlendAttachment.dstAlphaBlendFactor = VK_BLEND_FACTOR_ZERO; // Optional
//	colorBlendAttachment.alphaBlendOp = VK_BLEND_OP_ADD; // Optional
//
//	VkPipelineColorBlendStateCreateInfo colorBlending{};
//	colorBlending.sType = VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
//	colorBlending.logicOpEnable = VK_FALSE;
//	colorBlending.logicOp = VK_LOGIC_OP_COPY; // Optional
//	colorBlending.attachmentCount = 1;
//	colorBlending.pAttachments = &colorBlendAttachment;
//	colorBlending.blendConstants[0] = 0.0f; // Optional
//	colorBlending.blendConstants[1] = 0.0f; // Optional
//	colorBlending.blendConstants[2] = 0.0f; // Optional
//	colorBlending.blendConstants[3] = 0.0f; // Optional
//
//	VkDynamicState dynamicStates[] = {
//VK_DYNAMIC_STATE_VIEWPORT,
//VK_DYNAMIC_STATE_SCISSOR
//	};
//
//	VkPipelineDynamicStateCreateInfo dynamicState{};
//	dynamicState.sType = VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO;
//	dynamicState.dynamicStateCount = 2;
//	dynamicState.pDynamicStates = dynamicStates;
//
//	VkPushConstantRange push_constant{};
//	push_constant.offset = 0;
//	push_constant.size = sizeof(EntityPushConstant);
//	push_constant.stageFlags = VK_SHADER_STAGE_VERTEX_BIT;
//
//	VkPipelineLayoutCreateInfo pipelineLayoutInfo{};
//	pipelineLayoutInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO;
//	pipelineLayoutInfo.setLayoutCount = 1; // Optional
//	pipelineLayoutInfo.pSetLayouts = &dSetLayout; // Optional
//	pipelineLayoutInfo.pushConstantRangeCount = 1; // Optional
//	pipelineLayoutInfo.pPushConstantRanges = &push_constant; // Optional
//
//	if (vkCreatePipelineLayout(Engine->deviceManager.Device, &pipelineLayoutInfo, nullptr, &Layout) != VK_SUCCESS) {
//		throw std::runtime_error("failed to create pipeline layout!");
//	}
//
//	VkPipelineDepthStencilStateCreateInfo depthStencil{};
//	depthStencil.sType = VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO;
//	depthStencil.depthTestEnable = VK_TRUE;
//	depthStencil.depthWriteEnable = VK_TRUE;
//	depthStencil.depthCompareOp = VK_COMPARE_OP_LESS;
//	depthStencil.depthBoundsTestEnable = VK_FALSE;
//	depthStencil.minDepthBounds = 0.0f; // Optional
//	depthStencil.maxDepthBounds = 1.0f; // Optional
//	depthStencil.stencilTestEnable = VK_FALSE;
//	depthStencil.front = {}; // Optional
//	depthStencil.back = {}; // Optional
//
//	VkGraphicsPipelineCreateInfo pipelineInfo{};
//	pipelineInfo.sType = VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO;
//	pipelineInfo.stageCount = 2;
//	pipelineInfo.pStages = shaderStages;
//	pipelineInfo.pVertexInputState = &vertexInputInfo;
//	pipelineInfo.pInputAssemblyState = &inputAssembly;
//	pipelineInfo.pViewportState = &viewportState;
//	pipelineInfo.pRasterizationState = &rasterizer;
//	pipelineInfo.pMultisampleState = &multisampling;
//	pipelineInfo.pDepthStencilState = &depthStencil;
//	pipelineInfo.pColorBlendState = &colorBlending;
//	pipelineInfo.pDynamicState = nullptr; // Optional
//	pipelineInfo.layout = Layout;
//	pipelineInfo.renderPass = XeGetRenderPass(Engine);
//	pipelineInfo.subpass = 0;
//	pipelineInfo.basePipelineHandle = VK_NULL_HANDLE; // Optional
//	pipelineInfo.basePipelineIndex = -1; // Optional
//
//	if (vkCreateGraphicsPipelines(XeGetDevice(Engine), VK_NULL_HANDLE, 1, &pipelineInfo, nullptr, &Pipeline) != VK_SUCCESS) {
//		throw std::runtime_error("failed to create graphics pipeline!");
//	}
//}