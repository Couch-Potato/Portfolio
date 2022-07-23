#include "xenon.h"
#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>
#define TINYOBJLOADER_IMPLEMENTATION
#include "tiny_obj_loader.h"
#include <assimp/Importer.hpp>
#include <assimp/postprocess.h>  
#include <assimp/scene.h>
#include <assimp/mesh.h>
#include "xe_file.h"

void __WARN_NO_TEXCORD() {
	PLOGW << "Imported model mesh entity does not have a texcoord!";
}
void __WARN_NO_NORMAL() {
	PLOGW << "Imported model mesh entity does not have a normal!";
}

XeMesh XeImportMeshFromObj(std::string file) {
	XeMesh iMesh{};
	tinyobj::attrib_t attrib;
	std::vector<tinyobj::shape_t> shapes;
	std::vector<tinyobj::material_t> materials;
	std::string warn, err;
	if (!tinyobj::LoadObj(&attrib, &shapes, &materials, &warn, &err, file.c_str())) {
		throw std::runtime_error(warn + err);
	}
	PLOGV << "Mounted mesh from " << file;
	if (attrib.texcoords.size() == 0)
		__WARN_NO_TEXCORD();
	if (attrib.normals.size() == 0)
		__WARN_NO_NORMAL();
	std::unordered_map<Vertex, uint32_t> uniqueVertices{};
	for (const auto& shape : shapes) {
		for (const auto& index : shape.mesh.indices) {
			Vertex vertex{};
			vertex.pos = {
				attrib.vertices[3 * index.vertex_index + 0],
				attrib.vertices[3 * index.vertex_index + 1],
				attrib.vertices[3 * index.vertex_index + 2]
			};
			if (attrib.texcoords.size() > 0) {
				vertex.texCoord = {
				attrib.texcoords[2 * index.texcoord_index + 0],
				1.0f - attrib.texcoords[2 * index.texcoord_index + 1]
				};
			}
			
			vertex.color = { 1.0f, 1.0f, 1.0f };
			if (attrib.texcoords.size() > 0) {
				vertex.normal = {
				attrib.normals[3 * index.normal_index + 0],
				attrib.normals[3 * index.normal_index + 1],
				attrib.normals[3 * index.normal_index + 2],
				};
			}
			
			if (uniqueVertices.count(vertex) == 0) {
				uniqueVertices[vertex] = static_cast<uint32_t>(iMesh.Vertices.size());
				iMesh.Vertices.push_back(vertex);
			}
			iMesh.indices.push_back(uniqueVertices[vertex]);
		}
	}

	return iMesh;

}
XeMesh XeImportMeshFromObjBuffer(void* buffer, size_t len) {
	XeMesh iMesh{};
	tinyobj::attrib_t attrib;
	std::vector<tinyobj::shape_t> shapes;
	std::vector<tinyobj::material_t> materials;
	std::string warn, err;




	if (!tinyobj::LoadObj(&attrib, &shapes, &materials, &warn, &err, "1")) {
		throw std::runtime_error(warn + err);
	}
	PLOGV << "Mounted mesh from " << "1";
	if (attrib.texcoords.size() == 0)
		__WARN_NO_TEXCORD();
	if (attrib.normals.size() == 0)
		__WARN_NO_NORMAL();
	std::unordered_map<Vertex, uint32_t> uniqueVertices{};
	for (const auto& shape : shapes) {
		for (const auto& index : shape.mesh.indices) {
			Vertex vertex{};
			vertex.pos = {
				attrib.vertices[3 * index.vertex_index + 0],
				attrib.vertices[3 * index.vertex_index + 1],
				attrib.vertices[3 * index.vertex_index + 2]
			};
			if (attrib.texcoords.size() > 0) {
				vertex.texCoord = {
				attrib.texcoords[2 * index.texcoord_index + 0],
				1.0f - attrib.texcoords[2 * index.texcoord_index + 1]
				};
			}

			vertex.color = { 1.0f, 1.0f, 1.0f };
			if (attrib.texcoords.size() > 0) {
				vertex.normal = {
				attrib.normals[3 * index.normal_index + 0],
				attrib.normals[3 * index.normal_index + 1],
				attrib.normals[3 * index.normal_index + 2],
				};
			}

			if (uniqueVertices.count(vertex) == 0) {
				uniqueVertices[vertex] = static_cast<uint32_t>(iMesh.Vertices.size());
				iMesh.Vertices.push_back(vertex);
			}
			iMesh.indices.push_back(uniqueVertices[vertex]);
		}
	}

	return iMesh;

}
XeMesh XeImportMeshFromFBX(const std::string& file) {
	XeMesh iMesh{};
	//Assimp::Importer importer;
	//const aiScene* scene = importer.ReadFile(file, aiProcess_CalcTangentSpace |
	//	aiProcess_Triangulate |
	//	aiProcess_JoinIdenticalVertices |
	//	aiProcess_SortByPType);
	//bool HAS_NORMALS = scene->mMeshes[0]->HasNormals();
	//bool HAS_BONES = scene->mMeshes[0]->HasBones();
	//uint32_t size = scene->mMeshes[0]->mNumFaces;
	//auto x_mesh = scene->mMeshes[0];
	//bool HAS_TEXCOORDS = sizeof(x_mesh->mTextureCoords) > 0;
	//std::unordered_map<Vertex, uint32_t> uniqueVertices{};
	//for (uint32_t i = 0; i < size; i++) {
	//	auto face = scene->mMeshes[0]->mFaces[i];
	//	uint16_t k_size = face.mNumIndices;
	//	for (uint16_t k = 0; k < k_size; k++) {
	//		auto index = face.mIndices[k];
	//		Vertex vertex{};
	//		vertex.pos = {
	//			x_mesh->mVertices[index].x,
	//			x_mesh->mVertices[index].y,
	//			x_mesh->mVertices[index].z,
	//		};
	//		vertex.color = { 1.0f, 1.0f, 1.0f };
	//		if (HAS_NORMALS) {
	//			vertex.normal = {
	//				x_mesh->mNormals[index].x,
	//				x_mesh->mNormals[index].y,
	//				x_mesh->mNormals[index].z,
	//			};
	//		}
	//		if (HAS_TEXCOORDS) {
	//			vertex.texCoord = {
	//				x_mesh->mTextureCoords[0][index].x,
	//				1.0f - x_mesh->mTextureCoords[0][index].y,
	//			};
	//		}
	//		if (uniqueVertices.count(vertex) == 0) {
	//			uniqueVertices[vertex] = static_cast<uint32_t>(iMesh.Vertices.size());
	//			iMesh.Vertices.push_back(vertex);
	//		}
	//		/*if (HAS_BONES) {
	//			uint16_t j_size = x_mesh->mNumBones;
	//			for (uint16_t j = 0; j < j_size; j++) {
	//				auto bone = x_mesh->mBones[j];
	//				bone.
	//			}
	//		}*/
	//		iMesh.indices.push_back(uniqueVertices[vertex]);
	//	}
	//}
	return iMesh;
}


std::vector<char> XeReadFile(std::string& filename) {
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

XeMeshedEntity* XE_FACTORY_MESHED_ENTITY(XenonEngine* engine, XeEntityCreateInfo info) {
	XeMeshedEntity* c_entity = new XeMeshedEntity();
	c_entity->Initialize(engine);
	if (info.Mesh.MeshType == XeMeshType::XE_MESH_OBJ) {
		c_entity->Mesh = XeImportMeshFromObj(info.Mesh.ImportLocation);
	}
	else if (info.Mesh.MeshType == XeMeshType::XE_MESH_FBX) {
		c_entity->Mesh = XeImportMeshFromFBX(info.Mesh.ImportLocation);
	}
	else
	{
		throw std::runtime_error("Invalid mesh type");
	}
	c_entity->InitializeMeshBuffer();
	c_entity->Material = &engine->scene.materialRegistry[info.MaterialId];
	c_entity->Position = info.Position;
	c_entity->Orientation = info.Orientation;
	c_entity->Scale = info.Scale;
	c_entity->Update();
	return c_entity;
}
XeDefaultMaterial XE_FACTORY_DEFAULT_MATERIAL(XenonEngine* engine, XeMaterialCreateInfo info) {
	XeDefaultMaterial mat;


	// Ensure the shaders are properly setup
	if (info.FragmentShader.Method != XeShaderImportMethod::XE_PRE_COMPILED)
		throw std::runtime_error("There is no implemented compliation function for non-compiled shaders, or you did not specify an import method. (Fragment Shader)");
	if (info.VertexShader.Method != XeShaderImportMethod::XE_PRE_COMPILED)
		throw std::runtime_error("There is no implemented compliation function for non-compiled shaders, or you did not specify an import method. (Vertex Shader)");

	mat.Shaders.FragmentShader = XeCreateShader(engine, VK_SHADER_STAGE_FRAGMENT_BIT, XeReadFile(info.FragmentShader.ImportLocation));
	mat.Shaders.VertexShader = XeCreateShader(engine, VK_SHADER_STAGE_VERTEX_BIT, XeReadFile(info.VertexShader.ImportLocation));
	std::string shaderloc = "shadow/vert.spv";
	mat.Shaders.DepthVertexShader = XeCreateShader(engine, VK_SHADER_STAGE_VERTEX_BIT, XeReadFile(shaderloc));
	mat.Diffuse = info.Diffuse;
	mat.Specular = info.Specular;
	mat.Initialize(engine);
	mat.InitializeMaterial();
	return mat;
}

void XeBootstrapScene(XenonEngine* engine, XeSceneCreateInfo* scene) {
	XeScene c_scene;
	c_scene.Initialize(engine);
	for (int i = 0; i < scene->MaterialAmount; i++) {
		//PLOGV << scene->Materials[i].MaterialType;
		if (scene->Materials[i].MaterialType == XeMaterialType::XE_MATERIAL_DEFAULT) {
			//c_scene.InsertMaterial(XE_FACTORY_DEFAULT_MATERIAL(engine, scene->Materials[i]));
		}
		else {
			throw std::runtime_error("You specified either a NULL or invalid material type that cannot be instantiated.");
		}
	}
	for (int i = 0; i < scene->EntityAmount; i++) {
		auto entity = scene->Entities[i];
		if (entity.EntityType == XeEntityType::XE_ENTITY_MESHED_ENTITY) {
			XeMeshedEntity* c_entity = XE_FACTORY_MESHED_ENTITY(engine, entity);
			c_entity->Material = &c_scene.materialRegistry[entity.MaterialId];
			c_scene.InsertEntity(c_entity);
		}
		else {
			throw std::runtime_error("You specified either a NULL or invalid entity type that cannot be instantiated.");
		}
	}
	engine->scene = c_scene;
}

XeTexture XeImportTexture(XenonEngine* engine, XeTextureImportInfo importInfo) {
	XeTexture texture = XeTexture();

	texture.Initialize(engine);

	int texWidth, texHeight, texChannels;
	stbi_uc* pixels = stbi_load(importInfo.ImportLocation.c_str(), &texWidth, &texHeight, &texChannels, STBI_rgb_alpha);
	
	if (texWidth < 0) {
		throw std::runtime_error("failed to load texture image!");
	}

	texture.SetImageBuffer(pixels, texWidth, texHeight, false);

	return texture;
}
XeTexture XeImportTextureFromBuffer(XenonEngine* engine, void* buffer, size_t size) {
	XeTexture texture = XeTexture();

	texture.Initialize(engine);

	int texWidth, texHeight, texChannels;
	stbi_uc* pixels = stbi_load_from_memory((unsigned char*)buffer, size, &texWidth, &texHeight, &texChannels, STBI_rgb_alpha);

	if (texWidth <= 0) {
		throw std::runtime_error("failed to load texture image!");
	}

	texture.SetImageBuffer(pixels, texWidth, texHeight, false);

	return texture;
}

DYSTRO_ARCHIVE CURRENT_ARCHIVE;

void XeImportArchive(std::string name) {
	CURRENT_ARCHIVE.ReadFromFile(name);
	const char major = XE_F_VERSION_MAJOR;
	const char minor = XE_F_VERSION_MINOR;
	if (CURRENT_ARCHIVE.VERSION.MAJOR != major || CURRENT_ARCHIVE.VERSION.MINOR != minor) {
		PLOGF << "Invalid dystro version. \nThis file (" + name + ") was built towards an outdated version of the engine and it is not currently compatible. Please rebuild the archive.\nArchive Version: v" << CURRENT_ARCHIVE.VERSION.MAJOR << "." << CURRENT_ARCHIVE.VERSION.MINOR<< "\nCurrent Version: v" << major << "." << minor;
		throw std::runtime_error("Invalid dystro version. This file (" + name + ") was built towards an outdated version of the engine and it is not currently compatible. Please rebuild the archive.");
	}
	
}

void XeImportBootstrappables(XenonEngine* engine) {
	PLOGI << "Importing bootstrappables";
	IMP_BOOTSTRAP_REGISTRY* reg = CURRENT_ARCHIVE.GetEntryOfType<IMP_BOOTSTRAP_REGISTRY>(TYPE_BOOTSTRAP_REGISTRY);
	std::vector<IMP_BOOTSTRAPPABLE> bstraps = CURRENT_ARCHIVE.GetVector<IMP_BOOTSTRAPPABLE>(reg->bs_vec_location);
	for (auto const bStrap : bstraps) {
		PLOGV << "Importing boostrap: " << bStrap.hash;
		XeBootstrappable* bs = new XeBootstrappable;
		bs->Initialize(engine);
		bs->id = bStrap.hash;
		std::vector<IMP_SCRIPT> scripts = CURRENT_ARCHIVE.GetVector<IMP_SCRIPT>(bStrap.script_vec_location);
		for (auto const scr : scripts) {
			PLOGV << "Importing script " << scr.hash << " for " << bs->id;
			auto file = (char*)CURRENT_ARCHIVE.GetFile(scr.location);
			bs->AddScript(scr.hash, std::string(file, CURRENT_ARCHIVE.GetFileEntrySize(scr.location)));
		}

		const size_t xs = bs->id;
		
		// I have no idea why this fucking throws an error but for some god fucking awful reason it does.... Fuck my life, fuck microsoft.
		auto pair = std::pair<const size_t, XeBootstrappable*>(xs,bs);
		engine->scriptEngine.coreScripts.insert(pair);
	}
}

XeComponent* XeImportComponentFromArchive(XenonEngine* engine, size_t compName, XeMeshedEntity* linked) {
	PLOGV << "Importing component " << compName;
	IMP_COMPONENT_DEF* def = CURRENT_ARCHIVE.GetEntryFromHash<IMP_COMPONENT_DEF>(compName);
	//auto reg = XeAlloc<XeComponent>(engine);

	// Do not worry about this witchcraft. It is to skuurt past the deconstructor.
	auto cRegion = malloc(sizeof(XeComponent));
	XeComponent default_c;
	memcpy(cRegion, &default_c, sizeof(XeComponent));
	XeComponent* bs = reinterpret_cast<XeComponent*>(cRegion);


	bs->Initialize(engine);
	bs->id = compName;
	std::vector<IMP_SCRIPT> scripts = CURRENT_ARCHIVE.GetVector<IMP_SCRIPT>(def->script_vec_location);
	for (auto const scr : scripts) {
		PLOGV << "Importing script " << scr.hash << " for " << bs->id;
		auto file = (char*)CURRENT_ARCHIVE.GetFile(scr.location);
		bs->AddScript(scr.hash, std::string(file, CURRENT_ARCHIVE.GetFileEntrySize(scr.location)));
	}

	if (linked != nullptr) {
		bs->Attach(linked);
	}


	return bs;
}


XeTexture ImportTextureFromArchive(XenonEngine* engine, size_t name) {

	PLOGI << "Importing texture: " << name << ", writing to external buffer...";

	auto entry = CURRENT_ARCHIVE.GetFile(name);

	/*auto fn = std::tmpnam(NULL);

	std::FILE* tmpf = std::fopen(fn, "wb+");
	auto fsize = CURRENT_ARCHIVE.GetFileEntrySize(name);
	std::fwrite((char*)entry, sizeof(char), fsize, tmpf);
	std::fclose(tmpf);

	PLOGI << "Built texture: " << name << ".";

	XeTextureImportInfo info{};
	info.Channels = XeTextureImportChannels::XE_RGB_ALPHA;
	info.ImportLocation = fn;*/

	XeTexture txt = XeImportTextureFromBuffer(engine, entry, CURRENT_ARCHIVE.GetFileEntrySize(name));

	PLOGV << txt.mipLevels;

	//std::remove(fn);

	return txt;
}

XeScene XeImportSceneFromArchive(XenonEngine* engine, size_t name) {
	IMP_SCENE* scene = CURRENT_ARCHIVE.GetEntryFromHash<IMP_SCENE>(name);
	XeScene c_scene;
	c_scene.Initialize(engine);
	std::vector<IMP_ENTITY_CHILD> children = CURRENT_ARCHIVE.GetVector<IMP_ENTITY_CHILD>(scene->children);

	for (auto child : children) {
		auto entityx = ImportEntityFromArchive(engine, child.name, &c_scene);
		entityx.Position = glm::vec3(child.position.X, child.position.Y, child.position.Z);
		entityx.Scale = glm::vec3(child.scale.X, child.scale.Y, child.scale.Z);
		entityx.Orientation = glm::quat(glm::vec3(child.rotation.X, child.rotation.Y, child.rotation.Z));
		entityx.Update();


		//auto rgx = XeAlloc<XeMeshedEntity>(engine);
		//entityx.handle = rgx;
		XeMemSet(engine, entityx.handle, entityx);
		
		XeMeshedEntity* inst = XeGetInstance<XeMeshedEntity>(engine, entityx.handle);

		c_scene.InsertEntity(inst);
	}
	return c_scene;
}
XeMeshedEntity ImportEntityFromArchive(XenonEngine* engine, size_t name, XeScene* parentScene) {
	PLOGI << "Importing entity: " << name << "...";
	XeMeshedEntity c_entity;
	auto rgx = XeAlloc<XeMeshedEntity>(engine);
	c_entity.handle = rgx;
	c_entity.Initialize(engine);
	IMP_ENTITY* entity = CURRENT_ARCHIVE.GetEntryFromHash<IMP_ENTITY>(name);
	std::vector<IMP_ENTITY_CHILD> children = CURRENT_ARCHIVE.GetVector<IMP_ENTITY_CHILD>(entity->children_set);

	std::vector<size_t> compDefs = CURRENT_ARCHIVE.GetVector<size_t>(entity->component_list);

	if (entity->mesh_type != 0) {
		throw std::runtime_error("Invalid mesh type. Currently the engine only allows the mesh type of 'OBJ'.");
	}

	PLOGI << "Importing mesh for: " << name << ", writing to external buffer...";

	auto fn = std::tmpnam(NULL);
	std::FILE* tmpf = std::fopen(fn, "wb+");
	auto intx = CURRENT_ARCHIVE.GetFile(entity->mesh_index);
	auto fsize = CURRENT_ARCHIVE.GetFileEntrySize(entity->mesh_index);
	std::fwrite((char *)intx, sizeof(char), fsize, tmpf);
	//std::fputs((char *)(intx), tmpf);
	std::fclose(tmpf);

	c_entity.Mesh = XeImportMeshFromObj(fn);

	std::remove(fn);

	for (auto child : children) {
		auto entityx = ImportEntityFromArchive(engine, child.name, parentScene);
		entityx.Position = glm::vec3(child.position.X, child.position.Y, child.position.Z);
		entityx.Scale = glm::vec3(child.scale.X, child.scale.Y, child.scale.Z);
		entityx.Orientation = glm::quat(glm::vec3(child.rotation.X, child.rotation.Y, child.rotation.Z));
		entityx.Update();
		c_entity.children.push_back(XeGetInstance<XeMeshedEntity>(engine, entityx.handle));
		XeLinkMemory(engine, rgx, entityx.handle);
	}

	for (auto cx : compDefs) {
		auto linkedPtr = XeGetInstance<XeMeshedEntity>(engine, rgx);
		auto compx = XeImportComponentFromArchive(engine, cx, linkedPtr);

		//auto compHandle = XeAlloc<XeComponent>(engine, compx);

		c_entity.components.insert(std::pair<size_t, XeComponent*>(cx, compx));
		c_entity.cList.push_back(compx);
		//XeLinkMemory(engine, rgx, compHandle);
	}
	
	c_entity.InitializeMeshBuffer();

	bool PairedMaterial = false;
	for (auto material : parentScene->materialRegistry) {
		if (material.MaterialId == entity->material_id) {
			c_entity.Material = &material;
			PairedMaterial = true;
			break;
		}
	}
	if (!PairedMaterial) {
		XeDynamicMaterial newMat = ImportMaterialFromArchive(engine, entity->material_id);
		auto matx = parentScene->InsertMaterial(newMat);
		c_entity.Material = matx;
	}
	XeMemSet(engine, rgx, c_entity);
	return c_entity;
}

region_t recentlyLinked;

XeDynamicMaterial ImportMaterialFromArchive(XenonEngine* engine, size_t name) {
	PLOGI << "Importing material: " << name << "...";

	
	IMP_MATERIAL* mat = CURRENT_ARCHIVE.GetEntryFromHash<IMP_MATERIAL>(name);
	IMP_SHADER* shader = CURRENT_ARCHIVE.GetEntryFromHash<IMP_SHADER>(mat->shader_id);
	PLOGV << "Importing shader entries and buffers";
	std::vector<IMP_MATERIAL_PROPERTY> mat_props = CURRENT_ARCHIVE.GetVector<IMP_MATERIAL_PROPERTY>(mat->shader_prop_set);
	std::vector<IMP_SHADER_BUFFER> applied_buffers = CURRENT_ARCHIVE.GetVector<IMP_SHADER_BUFFER>(shader->buffers);

	auto mat_handle = XeAlloc<XeDynamicMaterial>(engine);

	XeDynamicMaterial dynmat;
	PLOGV << "Importing shader modules";

	PLOGV << "FRAG_S: " << shader->fragment_shader_index;
	PLOGV << "VERTEX_S: " << shader->vertex_shader_index;

	auto x = XeCreateShaderModule(engine, (char*)CURRENT_ARCHIVE.GetFile(shader->fragment_shader_index), CURRENT_ARCHIVE.GetFileEntrySize(shader->fragment_shader_index));
	auto y = XeCreateShaderModule(engine, (char*)CURRENT_ARCHIVE.GetFile(shader->vertex_shader_index), CURRENT_ARCHIVE.GetFileEntrySize(shader->vertex_shader_index));

	PLOGV << "Initializing shader modules";
	dynmat.Shaders.FragmentShader = XeCreateShader(engine, VK_SHADER_STAGE_FRAGMENT_BIT, x);
	PLOGV << "Imported fragment shader: " << CURRENT_ARCHIVE.GetFileEntrySize(shader->fragment_shader_index) << " bytes";
	dynmat.Shaders.VertexShader = XeCreateShader(engine, VK_SHADER_STAGE_VERTEX_BIT, y);
	PLOGV << "Imported vertex shader: " << CURRENT_ARCHIVE.GetFileEntrySize(shader->vertex_shader_index) << " bytes";
	std::string shaderloc = "shadow/vert.spv";
	dynmat.Shaders.DepthVertexShader = XeCreateShader(engine, VK_SHADER_STAGE_VERTEX_BIT, XeReadFile(shaderloc));
	dynmat.MaterialId = name;
	
	for (auto apb : applied_buffers)
	{
		// UNIFORM
		if (apb.bufferType == 0) {
			// Imported from material
			if (apb.bufferImporter.type == 0) {
				for (auto prop : mat_props) {
					// Finds the property with the correct name and checks if it is of a UBO type
					if (prop.name == apb.bufferImporter.source && prop.type == 0) {

						XeBufferItem item{};
						item.type = XeBufferType::UNIFORM;
						item.internalBuffer = XeMultiBuffer(sizeof(prop.value), &prop.value);
						auto bufptr = dynmat.InsertBuffer(item);
						auto bufhand = XeAlloc(engine, bufptr);
						XeLinkMemory(engine, mat_handle, bufhand);
					}
				}
			}
			// Set from shader file statically...
			else if(apb.bufferImporter.type == 1) {
				XeBufferItem item{};
				item.type = XeBufferType::UNIFORM;
				item.internalBuffer = XeMultiBuffer(sizeof(apb.bufferImporter.source), &apb.bufferImporter.source);
				auto bufptr = dynmat.InsertBuffer(item);
				auto bufhand = XeAlloc(engine, bufptr);
				XeLinkMemory(engine, mat_handle, bufhand);
			}
		}
		// TEXTURE
		if (apb.bufferType == 1) {
			// Imported from material
			if (apb.bufferImporter.type == 0) {
				for (auto prop : mat_props) {
					// Finds the property with the correct name and checks if it is of a UBO type
					if (prop.name == apb.bufferImporter.source && prop.type == 1) {

						XeBufferItem item{};
						XeTexture linked = ImportTextureFromArchive(engine, prop.value);
						item.type = XeBufferType::TEXTURE;
						item.imageSampler = linked.Sampler;
						item.imageView = linked.ImageView;
						auto tb = dynmat.LinkTexture(linked);
						auto toLink = XeAlloc(engine, tb);
						XeLinkMemory(engine, mat_handle, toLink);
						dynmat.InsertBuffer(item);
					}
				}
			}
			// Set from shader file statically...
			else if (apb.bufferImporter.type == 2) {
				XeBufferItem item{};
				XeTexture linked = ImportTextureFromArchive(engine, apb.bufferImporter.source);
				item.type = XeBufferType::TEXTURE;
				item.imageSampler = linked.Sampler;
				item.imageView = linked.ImageView;
				auto tb = dynmat.LinkTexture(linked);
				auto toLink = XeAlloc(engine, tb);
				XeLinkMemory(engine, mat_handle, toLink);
				dynmat.InsertBuffer(item);
			}
			// Pre-existant buffer
			else if (apb.bufferImporter.type == 3) {
				if (apb.bufferImporter.source == XeHashString("SCENE_DEPTH_MAP")) {
					XeBufferItem item{};
					item.type = XeBufferType::TEXTURE;
					item.imageSampler = engine->renderer.OffScreenPass.OffscreenPass.depthSampler;
					item.imageView = engine->renderer.OffScreenPass.OffscreenPass.depth.view;
					dynmat.InsertBuffer(item);
				}
				
			}
		}
	}

	// mat.Shaders.DepthVertexShader = XeCreateShader(engine, VK_SHADER_STAGE_VERTEX_BIT, XeReadFile(shaderloc));


	PLOGV << "Initializing material...";
	dynmat.Initialize(engine);
	dynmat.InitializeMaterial();
	return dynmat;
}