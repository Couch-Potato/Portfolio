#pragma once
#include "xenon.h"
#include <glm/gtc/quaternion.hpp> 
#include <glm/gtx/quaternion.hpp>

void XeEntity::Draw(VkCommandBuffer commandBuffer) {
	vkCmdPushConstants(commandBuffer, Material->Pipeline.Layout, VK_SHADER_STAGE_VERTEX_BIT, 0, sizeof(EntityPushConstant), &PushConstant);
	//DrawCmd(commandBuffer);
}

void XeMeshedEntity::DrawCmd2(VkCommandBuffer commandBuffer) {
	VkBuffer vertexBuffers[] = { VertexBuffer.Buffer };
	VkDeviceSize offsets[] = { 0 };
	vkCmdBindVertexBuffers(commandBuffer, 0, 1, vertexBuffers, offsets);
	vkCmdBindIndexBuffer(commandBuffer, IndexBuffer.Buffer, 0, VK_INDEX_TYPE_UINT32);
	vkCmdDrawIndexed(commandBuffer, static_cast<uint32_t>(Mesh.indices.size()), 1, 0, 0, 0);
}

void XeMeshedEntity::InitializeMeshBuffer() {
	VertexBuffer = XeCreateBufferBinding(Engine, sizeof(Mesh.Vertices[0]) * Mesh.Vertices.size(), Mesh.Vertices.data(), VK_BUFFER_USAGE_VERTEX_BUFFER_BIT);
	IndexBuffer = XeCreateBufferBinding(Engine, sizeof(Mesh.indices[0]) * Mesh.indices.size(), Mesh.indices.data(), VK_BUFFER_USAGE_INDEX_BUFFER_BIT);
}

void XeDrawEntity(VkCommandBuffer commandBuffer, XeMeshedEntity* entity) {
	entity->Draw(commandBuffer);
}
void XeDrawSkybox(VkCommandBuffer commandBuffer, XeSkybox* skybox) {
	skybox->Draw(commandBuffer);
}

void XeMeshedEntity::Update() {
	auto rotMatrix = glm::toMat4(Orientation);
	PushConstant.world = glm::scale(glm::translate(rotMatrix, Position), Scale);
	for (auto child : children) {
		child->Update();
		child->PushConstant.world = PushConstant.world * child->PushConstant.world;
	}
}

void XeMeshedEntity::Draw(VkCommandBuffer commandBuffer) {
	
	XeMeshedEntity::XeEntity::Draw(commandBuffer);
	DrawCmd2(commandBuffer);
}

void XeGenerateBufferAndDrawScene(XeScene* scene)
{
	scene->GenerateCommandBuffers();
}

void XeEntity::Cleanup() {
	throw new std::runtime_error("Why are you using a regular entity base. Make your own damn entity type...");
}
void XeMeshedEntity::Cleanup() {
	XeCleanupBuffer(Engine, VertexBuffer);
	XeCleanupBuffer(Engine, IndexBuffer);
}

void XeEntity::DrawCmd(VkCommandBuffer commandBuffer) {
	PLOGW << "Entity is using non-custom draw: " << this;
	throw std::runtime_error("Cannot draw a abstract entity. Use a derived one instead!");
}

void XeSkybox::Draw(VkCommandBuffer commandBuffer) {
	XeSkybox::XeEntity::Draw(commandBuffer);
	VkBuffer vertexBuffers[] = { VertexBuffer.Buffer };
	VkDeviceSize offsets[] = { 0 };
	vkCmdBindVertexBuffers(commandBuffer, 0, 1, vertexBuffers, offsets);
	vkCmdBindIndexBuffer(commandBuffer, IndexBuffer.Buffer, 0, VK_INDEX_TYPE_UINT32);
	vkCmdDrawIndexed(commandBuffer, static_cast<uint32_t>(Mesh.indices.size()), 1, 0, 0, 0);
}

void XeSkybox::InitializeMeshBuffer() {
	VertexBuffer = XeCreateBufferBinding(Engine, sizeof(Mesh.Vertices[0]) * Mesh.Vertices.size(), Mesh.Vertices.data(), VK_BUFFER_USAGE_VERTEX_BUFFER_BIT);
	IndexBuffer = XeCreateBufferBinding(Engine, sizeof(Mesh.indices[0]) * Mesh.indices.size(), Mesh.indices.data(), VK_BUFFER_USAGE_INDEX_BUFFER_BIT);
}

