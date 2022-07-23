#include "xenon.h"

// Depth bias (and slope) are used to avoid shadowing artifacts
float depthBiasConstant = 1.25f;
// Slope depth bias factor, applied depending on polygon's slope
float depthBiasSlope = 1.75f;


void XeScene::InsertEntity(XeMeshedEntity* entity) {
	auto mat = entity->Material;
	auto entityId = entityBuffer.size();
	entityBuffer.push_back(entity);

	// REMOVED !!!! WE GENERATE ENTITY MAPS ON RESOURCE START NOW BY RESOLVING THEM DIFFERENTLY!!!!

	//if (entityPairMap.find(mat) == entityPairMap.end())
	//{
	//	std::vector<XeMeshedEntity*> ins = std::vector<XeMeshedEntity*>();
	//	ins.push_back(entity);
	//	auto pair = std::pair<XeDynamicMaterial*, std::vector<XeMeshedEntity*>>(mat, ins);
	//	entityPairMap.insert(pair);
	//}
	//else {
	//	entityPairMap[mat].push_back(entity);

	//}
	
}

void XeScene::InsertEntity(XeEntityCreateInfo ifo) {
	XeScene::InsertEntity(XE_FACTORY_MESHED_ENTITY(Engine, ifo));
}

void XeScene::InsertMaterial(XeMaterialCreateInfo ifo) {
	//XeScene::InsertMaterial(XE_FACTORY_DEFAULT_MATERIAL(Engine, ifo));
}

//TODO:::
[[DEPRECATED]]
void XeScene::DeleteEntity(XeEntity entity) {
	
}

void POPULATE_MAP(XeMeshedEntity* ent , std::map<XeDynamicMaterial*, std::vector<XeMeshedEntity*>> * map) {
	if (map->find(ent->Material) == map->end()) {
		std::vector<XeMeshedEntity*> ins = std::vector<XeMeshedEntity*>();
		ins.push_back(ent);
		auto pair = std::pair<XeDynamicMaterial*, std::vector<XeMeshedEntity*>>(ent->Material, ins);
		map->insert(pair);
	}
	else {
		for (auto entx : map->at(ent->Material)) {
			if (entx == ent)
				return;
		}
		map->at(ent->Material).push_back(ent);
	}
	for (auto child : ent->children) {
		POPULATE_MAP(child, map);
	}
}

void XeScene::DrawScene(VkCommandBuffer commandBuffer, char drawId, bool isDepthDraw) {
	if (SkyboxEnabled) {
		//PLOGV << "Drawing skybox :D";
		XeBindMaterial(Skybox.Material, drawId, commandBuffer);
		XeDrawSkybox(commandBuffer, &Skybox);
	}


	// Prepare material map
	for (auto ent : entityBuffer) {
		POPULATE_MAP(ent, &entityPairMap);
	}


	for (auto const& [key, val] : entityPairMap) {
		if (isDepthDraw) XeBindMaterialToDepth(key, drawId, commandBuffer);
		else XeBindMaterial(key, drawId, commandBuffer);
		for (auto ent : val) {
			ent->Material = key;
			XeDrawEntity(commandBuffer, ent);
		}
	}
}

void XeScene::CreateCommandPool() {
	auto qfi = XeFindQueueFamilies(Engine);
	
	VkCommandPoolCreateInfo poolInfo{};
	poolInfo.sType = VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO;
	poolInfo.queueFamilyIndex = qfi.graphicsFamily.value();
	poolInfo.flags = 0; // Optional
	if (vkCreateCommandPool(XeGetDevice(Engine), &poolInfo, nullptr, &commandPool) != VK_SUCCESS) {
		throw std::runtime_error("failed to create command pool!");
	}
}

void XeScene::GenerateCommandBuffers() {
	if (commandBuffers.size() > 0) {
		//vkResetCommandPool(XeGetDevice(Engine), commandPool, VK_COMMAND_POOL_RESET_RELEASE_RESOURCES_BIT);
		vkQueueWaitIdle(Engine->deviceManager.graphicsQueue);
		vkFreeCommandBuffers(XeGetDevice(Engine), commandPool, static_cast<uint32_t>(commandBuffers.size()), &commandBuffers.data()[0]);
	}
	//TODO:: CHange to swapchain frame buffers
	commandBuffers.resize(Engine->drawSurface.swapChainImages.size());
	VkCommandBufferAllocateInfo allocInfo{};
	allocInfo.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO;
	allocInfo.commandPool = commandPool;
	allocInfo.level = VK_COMMAND_BUFFER_LEVEL_PRIMARY;
	allocInfo.commandBufferCount = (uint32_t)commandBuffers.size();

	std::array<VkClearValue, 2> clearValues{};
	


	if (vkAllocateCommandBuffers(XeGetDevice(Engine), &allocInfo, commandBuffers.data()) != VK_SUCCESS) {
		throw std::runtime_error("failed to allocate command buffers!");
	}
	for (size_t i = 0; i < commandBuffers.size(); i++) {
		VkCommandBufferBeginInfo beginInfo{};
		beginInfo.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO;
		beginInfo.flags = 0; // Optional
		beginInfo.pInheritanceInfo = nullptr; // Optional

		if (vkBeginCommandBuffer(commandBuffers[i], &beginInfo) != VK_SUCCESS) {
			throw std::runtime_error("failed to begin recording command buffer!");
		}

		/*OFFSCREEN PASS*/
		{
			clearValues[0].depthStencil = { 1.0f, 0 };
			VkRenderPassBeginInfo renderPassInfo{};
			renderPassInfo.sType = VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO;
			renderPassInfo.renderPass = this->Engine->renderer.OffScreenPass.OffscreenPass.renderPass;
			renderPassInfo.framebuffer = this->Engine->renderer.OffScreenPass.OffscreenPass.frameBuffer;
			renderPassInfo.renderArea.offset = { 0, 0 };
			renderPassInfo.renderArea.extent.width = this->Engine->renderer.OffScreenPass.OffscreenPass.width;
			renderPassInfo.renderArea.extent.height = this->Engine->renderer.OffScreenPass.OffscreenPass.height;

			renderPassInfo.clearValueCount = 1;
			renderPassInfo.pClearValues = clearValues.data();
			vkCmdBeginRenderPass(commandBuffers[i], &renderPassInfo, VK_SUBPASS_CONTENTS_INLINE);


			auto viewport = VkViewport{};
			viewport.width = this->Engine->renderer.OffScreenPass.OffscreenPass.width;
			viewport.height = this->Engine->renderer.OffScreenPass.OffscreenPass.height;
			viewport.maxDepth = 1.0f;
			viewport.minDepth = 0.0f;
			vkCmdSetViewport(commandBuffers[i], 0, 1, &viewport);

			auto scissor = VkRect2D{};
			scissor.extent.width = this->Engine->renderer.OffScreenPass.OffscreenPass.width;
			scissor.extent.height = this->Engine->renderer.OffScreenPass.OffscreenPass.height;
			scissor.offset.x = 0;
			scissor.offset.y = 0;
			vkCmdSetScissor(commandBuffers[i], 0, 1, &scissor);

			vkCmdSetDepthBias(commandBuffers[i], depthBiasConstant, 0.0f, depthBiasSlope);

			DrawScene(commandBuffers[i], i, true);

			vkCmdEndRenderPass(commandBuffers[i]);
		}
		


		/*MAIN RENDER PASS*/

		{
			clearValues[0].color = { 0.0f, 0.0f, 0.0f, 1.0f };
			clearValues[1].depthStencil = { 1.0f, 0 };
			VkRenderPassBeginInfo renderPassInfo{};
			renderPassInfo.sType = VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO;
			renderPassInfo.renderPass = Engine->renderer.RenderPass;
			renderPassInfo.framebuffer = Engine->swapChainFramebuffers[i];
			renderPassInfo.renderArea.offset = { 0, 0 };
			renderPassInfo.renderArea.extent = Engine->drawSurface.swapChainExtent;

			renderPassInfo.clearValueCount = static_cast<uint32_t>(clearValues.size());
			renderPassInfo.pClearValues = clearValues.data();
			vkCmdBeginRenderPass(commandBuffers[i], &renderPassInfo, VK_SUBPASS_CONTENTS_INLINE);
			DrawScene(commandBuffers[i], i);
			vkCmdEndRenderPass(commandBuffers[i]);
		}
		



		if (vkEndCommandBuffer(commandBuffers[i]) != VK_SUCCESS) {
			throw std::runtime_error("failed to record command buffer!");
		}
	}
}

std::vector<char> ReadFile(std::string& filename) {
	std::ifstream file(filename, std::ios::ate | std::ios::binary);
	size_t fileSize = (size_t)file.tellg();
	std::vector<char> buffer(fileSize);
	file.seekg(0);
	file.read(buffer.data(), fileSize);
	file.close();

	return buffer;
	if (!file.is_open()) {
		throw std::runtime_error("failed to open file!");
	}
}

void XeScene::SetSkybox(XeSkyboxCreateInfo skybox) {
	SkyboxEnabled = true;
	
	std::string* faces = new std::string[6];
	faces[0] = skybox.Front;
	faces[1] = skybox.Back;
	faces[2] = skybox.Up;
	faces[3] = skybox.Down;
	faces[4] = skybox.Right;
	faces[5] = skybox.Left;
	Skybox.Cubemap = XeCubemap(faces);
	Skybox.Cubemap.Initialize(Engine);
	Skybox.Cubemap.Init();
	
	// MEMMMMMMMMMMMMORYYYYY LEAK 
	// REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
	XeDefaultMaterial* mat = new XeDefaultMaterial();
	mat->Shaders.FragmentShader = XeCreateShader(Engine, VK_SHADER_STAGE_FRAGMENT_BIT, ReadFile(skybox.FragmentShader.ImportLocation));
	mat->Shaders.VertexShader = XeCreateShader(Engine, VK_SHADER_STAGE_VERTEX_BIT, ReadFile(skybox.VertexShader.ImportLocation));
	mat->Diffuse = Skybox.Cubemap;
	mat->Specular = Skybox.Cubemap;
	mat->Initialize(Engine);
	mat->InitializeMaterial();
	Skybox.Mesh = XeImportMeshFromObj(skybox.Isosphere.ImportLocation);
	Skybox.Initialize(Engine);
	Skybox.InitializeMeshBuffer();
	//Skybox.Material = mat;
}

void XeScene::Cleanup() {
	for (auto const& [key, val] : entityPairMap) {
		key->Cleanup();
		for (auto ent : val) {
			ent->Cleanup();
		}
	}
	vkFreeCommandBuffers(XeGetDevice(Engine), Engine->commandPool, static_cast<uint32_t>(commandBuffers.size()), commandBuffers.data());
}

void XeAttachSkyboxToScene(XeScene* scene, XeSkyboxCreateInfo info) {
	scene->SetSkybox(info);
}

void XeScene::Initialize(XenonEngine* engine) {
	XeScene::EngineAttached::Initialize(engine);
	CreateCommandPool();
}

XeDynamicMaterial* XeScene::InsertMaterial(XeDynamicMaterial mat) {
	materialRegistry.push_back(mat);
	return &materialRegistry[materialRegistry.size() - 1];
}

void XeScene::InsertLight(glm::vec3 dir, glm::vec3 pos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec, float radii, float oradii) {
	Lights.push_back(XeLight(dir, pos, ambient, diffuse, spec, radii, oradii));
}
void XeScene::InsertLight(glm::vec3 dorpos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec) {
	Lights.push_back(XeLight(dorpos, ambient, diffuse, spec, XeLightingType::XE_LIGHTING_TYPE_POINT));
}
void XeScene::SetDirectionalLight(glm::vec3 dorpos, glm::vec3 ambient, glm::vec3 diffuse, glm::vec3 spec) {
	DirectionalLight = XeLight(dorpos, ambient, diffuse, spec, XeLightingType::XE_LIGHTING_TYPE_DIRECTIONAL);
}


XeSceneData XeScene::GetSceneMaterialData() {
	XeSceneData data{};
	data.projection = glm::perspective(glm::radians(45.0f), 1.0f, 0.1f, 10.0f);
	data.view = glm::lookAt(Camera.Position, Camera.LookAt, Camera.UpVector);
	data.viewPos = Camera.Position;
	data.directional = XE_LIGHT_DIRECTIONAL{};
	if (DirectionalLight.LightType == XeLightingType::XE_LIGHTING_TYPE_NONE) {
		data.directional.enabled = false;
	}
	else {
		data.directional = DirectionalLight.GetDirectional();
	}

	std::vector<XeLight> points;
	std::vector<XeLight> spots;

	for (int i = 0; i < Lights.size(); i++) {
		if (Lights[i].LightType == XeLightingType::XE_LIGHTING_TYPE_POINT)
			points.push_back(Lights[i]);
		else if (Lights[i].LightType == XeLightingType::XE_LIGHTING_TYPE_SPOT)
			spots.push_back(Lights[i]);
		else
			throw std::runtime_error("Invalid light in array");
	}


	for (int i = 0; i < NUM_POINT_LIGHTS; i++) {
		if (i > points.size() - 1) {
			data.points[i] = XE_LIGHT_POINT{};
			data.points[i].enabled = false;
		}
		else {
			data.points[i] = points[i].GetPoint();
		}
	}
	for (int i = 0; i < NUM_SPOT_LIGHTS; i++) {
		if (i > spots.size() - 1) {
			data.spots[i] = XE_LIGHT_SPOT{};
			data.spots[i].enabled = false;
		}
		else {
			data.spots[i] = spots[i].GetSpot();
		}
	}


	return data;
}

VkCommandBuffer* XeScene::GetCommandBuffer(uint32_t i) {
	return &commandBuffers[i];
}