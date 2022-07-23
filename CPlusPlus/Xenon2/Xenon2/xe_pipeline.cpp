#pragma once
#include "xenon.h"
#include "xe_descriptor_template.h"

#pragma region Shader

VkShaderModule XeCreateShaderModule(XenonEngine* engine, const std::vector<char>& code) {
	VkShaderModuleCreateInfo createInfo{};
	createInfo.sType = VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;
	createInfo.codeSize = code.size();
	createInfo.pCode = reinterpret_cast<const uint32_t*>(code.data());
	VkShaderModule shaderModule;
	if (vkCreateShaderModule(engine->deviceManager.Device, &createInfo, nullptr, &shaderModule) != VK_SUCCESS) {
		throw std::runtime_error("failed to create shader module!");
	}
	return shaderModule;
}

VkShaderModule XeCreateShaderModule(XenonEngine* engine, char* code, size_t entrySize) {

	PLOGV << "Importing shader: " << entrySize << "bytes";

	VkShaderModuleCreateInfo createInfo{};
	createInfo.sType = VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;
	createInfo.codeSize = entrySize;
	createInfo.pCode = reinterpret_cast<const uint32_t*>(code);
	VkShaderModule shaderModule;
	if (vkCreateShaderModule(engine->deviceManager.Device, &createInfo, nullptr, &shaderModule) != VK_SUCCESS) {
		throw std::runtime_error("failed to create shader module!");
	}
	return shaderModule;
}




VkPipelineShaderStageCreateInfo XeCreateShader(XenonEngine* engine, VkShaderStageFlagBits stage, const std::vector<char>& code) {
	VkPipelineShaderStageCreateInfo p_createInfo{};
	p_createInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
	p_createInfo.stage = stage;
	p_createInfo.module = XeCreateShaderModule(engine, code);
	p_createInfo.pName = "main";

	return p_createInfo;
}

VkPipelineShaderStageCreateInfo XeCreateShader(XenonEngine* engine, VkShaderStageFlagBits stage, VkShaderModule smodule) {
	VkPipelineShaderStageCreateInfo p_createInfo{};
	p_createInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
	p_createInfo.stage = stage;
	p_createInfo.module = smodule;
	p_createInfo.pName = "main";

	return p_createInfo;
}

#pragma endregion

#pragma region Pipeline

void XePipeline::InititalizePipeline(VkDescriptorSetLayout dSetLayout, VkPipelineShaderStageCreateInfo vertex, VkPipelineShaderStageCreateInfo frag, bool isDepthOnly) {

	float ShadowMapSize = SHADOWMAP_DIM;
	VkExtent2D ShadowMapExtent = {
		ShadowMapSize,
		ShadowMapSize
	};

	VkPipelineShaderStageCreateInfo shaderStages[] = { vertex, frag };

	VkPipelineVertexInputStateCreateInfo vertexInputInfo{};
	vertexInputInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO;
	vertexInputInfo.vertexBindingDescriptionCount = 0;
	vertexInputInfo.pVertexBindingDescriptions = nullptr; // Optional
	vertexInputInfo.vertexAttributeDescriptionCount = 0;
	vertexInputInfo.pVertexAttributeDescriptions = nullptr; // Optional

	auto bindingDescription = Vertex::getBindingDescription();
	auto attributeDescriptions = Vertex::getAttributeDescriptions();
	vertexInputInfo.vertexBindingDescriptionCount = 1;
	vertexInputInfo.vertexAttributeDescriptionCount = static_cast<uint32_t>(attributeDescriptions.size());
	vertexInputInfo.pVertexBindingDescriptions = &bindingDescription;
	vertexInputInfo.pVertexAttributeDescriptions = attributeDescriptions.data();

	VkPipelineInputAssemblyStateCreateInfo inputAssembly{};
	inputAssembly.sType = VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO;
	inputAssembly.topology = VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST;
	inputAssembly.primitiveRestartEnable = VK_FALSE;

	VkViewport viewport{};
	viewport.x = 0.0f;
	viewport.y = 0.0f;
	viewport.width = isDepthOnly ? ShadowMapSize : (float)Engine->drawSurface.swapChainExtent.width;
	viewport.height = isDepthOnly ? ShadowMapSize : (float)Engine->drawSurface.swapChainExtent.height;
	viewport.minDepth = 0.0f;
	viewport.maxDepth = 1.0f;

	VkRect2D scissor{};
	scissor.offset = { 0, 0 };
	scissor.extent = ShadowMapExtent;

	VkPipelineViewportStateCreateInfo viewportState{};
	viewportState.sType = VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO;
	viewportState.viewportCount = 1;
	viewportState.pViewports = &viewport;
	viewportState.scissorCount = 1;
	viewportState.pScissors = &scissor;

	VkPipelineRasterizationStateCreateInfo rasterizer{};
	rasterizer.sType = VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO;
	rasterizer.depthClampEnable = VK_FALSE;
	rasterizer.rasterizerDiscardEnable = VK_FALSE;
	rasterizer.polygonMode = VK_POLYGON_MODE_FILL;
	rasterizer.lineWidth = 1.0f;
	rasterizer.cullMode = VK_CULL_MODE_BACK_BIT;
	rasterizer.frontFace = VK_FRONT_FACE_CLOCKWISE;
	rasterizer.depthBiasEnable = VK_FALSE;
	rasterizer.depthBiasConstantFactor = 0.0f; // Optional
	rasterizer.depthBiasClamp = 0.0f; // Optional
	rasterizer.depthBiasSlopeFactor = 0.0f; // Option

	VkPipelineMultisampleStateCreateInfo multisampling{};
	multisampling.sType = VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO;
	multisampling.sampleShadingEnable = VK_FALSE;
	multisampling.rasterizationSamples = isDepthOnly ? VK_SAMPLE_COUNT_1_BIT : Engine->deviceManager.MSAASamples;
	multisampling.minSampleShading = 1.0f; // Optional
	multisampling.pSampleMask = nullptr; // Optional
	multisampling.alphaToCoverageEnable = VK_FALSE; // Optional
	multisampling.alphaToOneEnable = VK_FALSE; // Optional

	VkPipelineColorBlendAttachmentState colorBlendAttachment{};
	colorBlendAttachment.colorWriteMask = VK_COLOR_COMPONENT_R_BIT | VK_COLOR_COMPONENT_G_BIT | VK_COLOR_COMPONENT_B_BIT | VK_COLOR_COMPONENT_A_BIT;
	colorBlendAttachment.blendEnable = VK_FALSE;
	colorBlendAttachment.srcColorBlendFactor = VK_BLEND_FACTOR_ONE; // Optional
	colorBlendAttachment.dstColorBlendFactor = VK_BLEND_FACTOR_ZERO; // Optional
	colorBlendAttachment.colorBlendOp = VK_BLEND_OP_ADD; // Optional
	colorBlendAttachment.srcAlphaBlendFactor = VK_BLEND_FACTOR_ONE; // Optional
	colorBlendAttachment.dstAlphaBlendFactor = VK_BLEND_FACTOR_ZERO; // Optional
	colorBlendAttachment.alphaBlendOp = VK_BLEND_OP_ADD; // Optional

	VkPipelineColorBlendStateCreateInfo colorBlending{};
	colorBlending.sType = VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
	colorBlending.logicOpEnable = VK_FALSE;
	colorBlending.logicOp = VK_LOGIC_OP_COPY; // Optional
	colorBlending.attachmentCount = 1;
	colorBlending.pAttachments = &colorBlendAttachment;
	colorBlending.blendConstants[0] = 0.0f; // Optional
	colorBlending.blendConstants[1] = 0.0f; // Optional
	colorBlending.blendConstants[2] = 0.0f; // Optional
	colorBlending.blendConstants[3] = 0.0f; // Optional

	VkDynamicState dynamicStates[] = {
VK_DYNAMIC_STATE_VIEWPORT,
VK_DYNAMIC_STATE_LINE_WIDTH
	};

	VkPipelineDynamicStateCreateInfo dynamicState{};
	dynamicState.sType = VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO;
	dynamicState.dynamicStateCount = 2;
	dynamicState.pDynamicStates = dynamicStates;

	VkPushConstantRange push_constant{};
	push_constant.offset = 0;
	push_constant.size = sizeof(EntityPushConstant);
	push_constant.stageFlags = VK_SHADER_STAGE_VERTEX_BIT;

	VkPipelineLayoutCreateInfo pipelineLayoutInfo{};
	pipelineLayoutInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO;
	pipelineLayoutInfo.setLayoutCount = 1; // Optional
	pipelineLayoutInfo.pSetLayouts = &dSetLayout; // Optional
	pipelineLayoutInfo.pushConstantRangeCount = 1; // Optional
	pipelineLayoutInfo.pPushConstantRanges = &push_constant; // Optional

	if (vkCreatePipelineLayout(Engine->deviceManager.Device, &pipelineLayoutInfo, nullptr, &Layout) != VK_SUCCESS) {
		throw std::runtime_error("failed to create pipeline layout!");
	}

	VkPipelineDepthStencilStateCreateInfo depthStencil{};
	depthStencil.sType = VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO;
	depthStencil.depthTestEnable = VK_TRUE;
	depthStencil.depthWriteEnable = VK_TRUE;
	depthStencil.depthCompareOp = VK_COMPARE_OP_LESS;
	depthStencil.depthBoundsTestEnable = VK_FALSE;
	depthStencil.minDepthBounds = 0.0f; // Optional
	depthStencil.maxDepthBounds = 1.0f; // Optional
	depthStencil.stencilTestEnable = VK_FALSE;
	depthStencil.front = {}; // Optional
	depthStencil.back = {}; // Optional

	VkGraphicsPipelineCreateInfo pipelineInfo{};
	pipelineInfo.sType = VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO;
	pipelineInfo.stageCount = 2;
	pipelineInfo.pStages = shaderStages;
	pipelineInfo.pVertexInputState = &vertexInputInfo;
	pipelineInfo.pInputAssemblyState = &inputAssembly;
	pipelineInfo.pViewportState = &viewportState;
	pipelineInfo.pRasterizationState = &rasterizer;
	pipelineInfo.pMultisampleState = &multisampling;
	pipelineInfo.pDepthStencilState = &depthStencil;
	pipelineInfo.pColorBlendState = &colorBlending;
	pipelineInfo.pDynamicState = nullptr; // Optional
	pipelineInfo.layout = Layout;
	pipelineInfo.renderPass = isDepthOnly ? Engine->renderer.OffScreenPass.OffscreenPass.renderPass : XeGetRenderPass(Engine);
	pipelineInfo.subpass = 0;
	pipelineInfo.basePipelineHandle = VK_NULL_HANDLE; // Optional
	pipelineInfo.basePipelineIndex = -1; // Optional


	if (isDepthOnly)
	{
		pipelineInfo.stageCount = 1;
		colorBlending.attachmentCount = 0;
		depthStencil.depthCompareOp = VK_COMPARE_OP_LESS_OR_EQUAL;
		rasterizer.depthBiasEnable = VK_TRUE;
	}

	if (vkCreateGraphicsPipelines(XeGetDevice(Engine), VK_NULL_HANDLE, 1, &pipelineInfo, nullptr, &Pipeline) != VK_SUCCESS) {
		throw std::runtime_error("failed to create graphics pipeline!");
	}
}

void XePipeline::Cleanup() {
	vkDestroyPipeline(XeGetDevice(Engine), Pipeline, nullptr);
	vkDestroyPipelineLayout(XeGetDevice(Engine), Layout, nullptr);
}

#pragma endregion

#pragma region Material

void XeMaterial::Bind(char drawId, VkCommandBuffer commandBuffer) {
	BindPipeline(commandBuffer);
	vkCmdBindDescriptorSets(commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, Pipeline.Layout, 0, 1, &DescriptorSets[drawId], 0, nullptr);
	HandleBind(drawId, commandBuffer);
}


void XeMaterial::BindPipeline(VkCommandBuffer commandBuffer) {
	vkCmdBindPipeline(commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, Pipeline.Pipeline);
}

void XeMaterial::Cleanup() {
	Pipeline.Cleanup();
}

void XeMaterial::InitializePipeline() {
	Pipeline.Initialize(Engine);
	PLOGV << "Creating graphics pipeline for specified material";
	Pipeline.InititalizePipeline(DSetLayout, Shaders.VertexShader, Shaders.FragmentShader);
	ShadowPipeline.Initialize(Engine);
	PLOGV << "Creating shadow pipeline for specified material";
	ShadowPipeline.InititalizePipeline(DSetLayout, Shaders.DepthVertexShader, Shaders.FragmentShader, true);
	PLOGV << "Material has been fully initialized";
}


void TestMaterial::InitializeMaterial() {
	auto bindings = std::vector<XeDescriptorTemplate>{
		XE_UNIFORM_BUFFER_ALL,
		XE_IMAGE_SAMPLER,
		XE_UNIFORM_BUFFER_ALL,
	};
	auto ly = XeGetLayoutFromTemplate(Engine, bindings);
	DSetLayout = ly;
	DescriptorPool = XeGetDescriptorPoolFromTemplate(Engine, bindings);
	DescriptorSets = XeAllocateDescriptorSets(Engine,ly, DescriptorPool);

	for (int i = 0; i < DescriptorSets.size(); i++) {
		auto sceneWrite = XeBufferToDescriptor(Engine, &SceneBuffer, DescriptorSets[i], i, 0);
		auto imageWrite = XeTextureToDescriptor(Engine, &Diffuse, DescriptorSets[i], 1);
		auto materialWrite = XeBufferToDescriptor(Engine, &MaterialDataBuffer, DescriptorSets[i], i, 2);

		auto writes = std::vector<VkWriteDescriptorSet>{ sceneWrite, imageWrite, materialWrite };
		XeWriteDescriptorSets(Engine, writes);
	}
}

bool madeBindWarning = false;
void XeMaterial::HandleBind(char drawId, VkCommandBuffer commandBuffer) {
	if (!madeBindWarning) {
		PLOGW << "Material is using non-custom bind: " << this;
		madeBindWarning = true;
	}
	//throw std::runtime_error("You cannot bind using the abstract material. Use a derived material!");
}

void XeMaterial::InitializeMaterial() {
	throw std::runtime_error("You cannot initialize using the abstract material. Use a derived material!");
}

void TestMaterial::HandleBind(char drawId, VkCommandBuffer commandBuffer) {

}



void TestMaterial::Cleanup() {
	TestMaterial::XeMaterial::Cleanup();
	SceneBuffer.Cleanup();
	MaterialDataBuffer.Cleanup();
	Diffuse.Cleanup();
}

#pragma endregion
