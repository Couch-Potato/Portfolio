#include "xenon.h"
#include <libplatform/libplatform.h>

#define MATH_PI 3.14159

uint16_t WIDTH = 800;
uint16_t HEIGHT = 800;

class MyEngine : public XenonEngine {
	XeSceneCreateInfo* Ascene;
public:
	MyEngine(){}
	std::vector<std::string> sourceFiles;
protected:
	XeSceneCreateInfo* ConstructInitialScene() {
		
		Ascene = new XeSceneCreateInfo();
		// Basic material
		Ascene->MaterialAmount = 0;
		Ascene->EntityAmount = 0;
		return Ascene;
	}
	void AfterInit() {
		/*auto mat = XeMaterialCreateInfo();
		auto txture = XeTextureImportInfo();
		txture.Channels = XeTextureImportChannels::XE_RGB_ALPHA;
		txture.ImportLocation = "viking_room.png";
		mat.Diffuse = XeImportTexture(this, txture);

		auto spec = XeTextureImportInfo();
		spec.Channels = XeTextureImportChannels::XE_RGB_ALPHA;
		spec.ImportLocation = "spec.png";
		mat.Specular = XeImportTexture(this, spec);


		XeShaderImportInfo vertex = XeShaderImportInfo();
		vertex.ImportLocation = "vert.spv";
		vertex.Method = XeShaderImportMethod::XE_PRE_COMPILED;
		XeShaderImportInfo frag = XeShaderImportInfo();
		frag.ImportLocation = "frag.spv";
		frag.Method = XeShaderImportMethod::XE_PRE_COMPILED;

		mat.FragmentShader = frag;
		mat.VertexShader = vertex;

		scene.InsertMaterial(mat);

		auto ent = XeEntityCreateInfo();
		ent.EntityType = XeEntityType::XE_ENTITY_MESHED_ENTITY;
		ent.MaterialId = 0;
		
		auto mesh = XeMeshCreateInfo();
		mesh.ImportLocation = "mesh.obj";
		mesh.MeshType = XeMeshType::XE_MESH_OBJ;
		ent.Mesh = mesh;
		ent.Orientation = glm::quat(glm::vec3(MATH_PI, 0, 0));
		scene.InsertEntity(ent);



		auto ent2 = XeEntityCreateInfo();
		ent2.EntityType = XeEntityType::XE_ENTITY_MESHED_ENTITY;
		ent2.MaterialId = 0;

		auto mesh2 = XeMeshCreateInfo();
		mesh2.ImportLocation = "mesh.obj";
		mesh2.MeshType = XeMeshType::XE_MESH_OBJ;
		ent2.Mesh = mesh;
		ent2.Orientation = glm::quat(glm::vec3(MATH_PI, 0, 0));
		ent2.Position = glm::vec3(1, 0, .1);
		ent2.Scale = glm::vec3(.3, .3, .3);
		scene.InsertEntity(ent2);*/

		
		for (auto entry : sourceFiles) {
			XeImportArchive(entry);
		}
		if (sourceFiles.size() == 0)
			XeImportArchive("bin.dystro");
		scene = XeImportSceneFromArchive(scene.Engine, XeHashString("SCENE_DEFAULT"));


		scene.SetDirectionalLight(
			glm::vec3(-0.2f, -1.0f, -0.3f),
			glm::vec3(0.05f, 0.05f, 0.05f),
			glm::vec3(0.4f, 0.4f, 0.4f),
			glm::vec3(0.5f, 0.5f, 0.5f)
			);


		//Point Lights
		scene.InsertLight(
			glm::vec3(0.7f, 0.2f, 2.0f),
			glm::vec3(0.05f, 0.05f, 0.05f),
			glm::vec3(0.8f, 0.8f, 0.8f),
			glm::vec3(1.0f, 1.0f, 1.0f)
		);
		scene.InsertLight(
			glm::vec3(2.3f, -3.3f, -4.0f),
			glm::vec3(0.05f, 0.05f, 0.05f),
			glm::vec3(0.8f, 0.8f, 0.8f),
			glm::vec3(1.0f, 1.0f, 1.0f)
		);
		scene.InsertLight(
			glm::vec3(-4.0f, 2.0f, -12.0f),
			glm::vec3(0.05f, 0.05f, 0.05f),
			glm::vec3(0.8f, 0.8f, 0.8f),
			glm::vec3(1.0f, 1.0f, 1.0f)
		);
		scene.InsertLight(
			glm::vec3(0.0f, 0.0f, -3.0f),
			glm::vec3(0.05f, 0.05f, 0.05f),
			glm::vec3(0.8f, 0.8f, 0.8f),
			glm::vec3(1.0f, 1.0f, 1.0f)
		);


		//Spot lights
		scene.InsertLight(scene.Camera.LookAt,scene.Camera.Position, glm::vec3(0.0f), glm::vec3(1.0f), glm::vec3(1.0f), 12.5f, 15.0f);


		// Skybox

		XeSkyboxCreateInfo sky;
		sky.Front = "skybox/front.png";
		sky.Down = "skybox/down.png";
		sky.Up = "skybox/up.png";
		sky.Back = "skybox/back.png";
		sky.Right = "skybox/right.png";
		sky.Left = "skybox/left.png";

		auto smesh = XeMeshCreateInfo();
		smesh.ImportLocation = "skybox/isosphere.obj";
		smesh.MeshType = XeMeshType::XE_MESH_OBJ;

		sky.Isosphere = smesh;

		XeShaderImportInfo vertexs = XeShaderImportInfo();
		vertexs.ImportLocation = "skybox/vert.spv";
		vertexs.Method = XeShaderImportMethod::XE_PRE_COMPILED;
		XeShaderImportInfo frags = XeShaderImportInfo();
		frags.ImportLocation = "skybox/frag.spv";
		frags.Method = XeShaderImportMethod::XE_PRE_COMPILED;

		sky.FragmentShader = frags;
		sky.VertexShader = vertexs;

		//XeAttachSkyboxToScene(&scene, sky);


	}
	void AfterCleanup() {
		glfwDestroyWindow(Window);

		glfwTerminate();
	}
public:
	void Run(const char* module_n) {
		glfwInit();
		glfwWindowHint(GLFW_CLIENT_API, GLFW_NO_API);
		auto window = glfwCreateWindow(WIDTH, HEIGHT, "Xenon 2", nullptr, nullptr);
		glfwSetWindowUserPointer(window, this);
		XenonEngine::BeginEngine(window, XE_LOG_VERBOSE, module_n);
	}
};

int main(int argc, char* argv[]) {
	MyEngine engine;
	for (int i = 1; i < argc; i++) {
		engine.sourceFiles.push_back(std::string(argv[i]));
	}
	v8::V8::InitializeExternalStartupData(argv[0]);
	std::unique_ptr<v8::Platform> platform = v8::platform::NewDefaultPlatform();
	v8::V8::InitializePlatform(platform.get());
	v8::V8::Initialize();
	v8::Isolate::CreateParams create_params;
	create_params.array_buffer_allocator =
		v8::ArrayBuffer::Allocator::NewDefaultAllocator();
	v8::Isolate* isolate = v8::Isolate::New(create_params);
	engine.scriptEngine.isolate = isolate;

	v8::HandleScope handle_scope(isolate);
	v8::Isolate::Scope isolate_scope(isolate);

	engine.Run(argv[0]);
}