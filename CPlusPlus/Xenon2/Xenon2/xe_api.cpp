#include "xe_api.h"

using namespace v8;

const char* ToCString(const String::Utf8Value& value) {
	return *value ? *value : "<string conversion failed>";
}

void CreateVector(Isolate* isolate, Local<ObjectTemplate> existing, float x, float y, float z) {
	existing->Set(isolate, "x", Number::New(isolate, x));
	existing->Set(isolate, "y", Number::New(isolate, y));
	existing->Set(isolate, "z", Number::New(isolate, z));

}

struct VECTOR_UNPACKED {
	double x;
	double y;
	double z;
};

VECTOR_UNPACKED UnpackVector(Local<Object> vector, Local<Context> ctx) {
	VECTOR_UNPACKED upk{};
	upk.x = vector->Get(ctx, String::NewFromUtf8Literal(ctx->GetIsolate(), "x")).ToLocalChecked()->ToNumber(ctx).ToLocalChecked()->Value();
	upk.y = vector->Get(ctx, String::NewFromUtf8Literal(ctx->GetIsolate(), "y")).ToLocalChecked()->ToNumber(ctx).ToLocalChecked()->Value();
	upk.z = vector->Get(ctx, String::NewFromUtf8Literal(ctx->GetIsolate(), "z")).ToLocalChecked()->ToNumber(ctx).ToLocalChecked()->Value();
	return upk;
}

namespace XE_API_COMPONENT {
	void export_f(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto comp = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext()).component;
		//comp->exports.Reset(args.GetIsolate(), args[0]);
	}

	void export_f_as(const v8::FunctionCallbackInfo<v8::Value>& args){
		//auto comp = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext())->component;
		//auto isolate = args.GetIsolate();
		//if (comp->exports.IsEmpty()) {
		//	Local<ObjectTemplate> object = ObjectTemplate::New(args.GetIsolate());
		//	object->Set(args.GetIsolate(), ToCString(String::Utf8Value(args.GetIsolate(), args[0])), args[1]);
		//	//comp->exports.Reset(args.GetIsolate(), object);
		//}
		//else {
		//	if (comp->exports.Get(isolate)->IsObject()) {
		//		auto lc = comp->exports.Get(isolate)->ToObject(args.GetIsolate()->GetCurrentContext()).ToLocalChecked();
		//		//Local<Value> vx = Local<Value>(args[1]);
		//		//lc->Set(args.GetIsolate()->GetCurrentContext(), args[0], args[1]);
		//		//comp->exports.Reset(args.GetIsolate(), lc);
		//	}
		//	else {
		//		throw std::runtime_error("Cannot 'export as' when the currently exported object is not an object.");
		//	}
		//}
	}

	void require_f(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto comp = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext()).component;
		auto name_hash = ToCString(String::Utf8Value(isolate, args[0]));
		
		if (XeDoesComponentExist(comp->linkedEntity, name_hash)) {
			PLOGV << "Resolving dependency " << name_hash << " for " <<comp->id << ".\nDependency Type: COMPONENT";
			//args.GetReturnValue().Set(XeGetComponent(comp->linkedEntity, name_hash)->Get(isolate));

			return;
		}
	
		if (XeDoesCoreLibExist(CURRENT, name_hash)) {
			PLOGV << "Resolving dependency " << name_hash << " for " << comp->id << ".\nDependency Type: CORE_LIB";
			//args.GetReturnValue().Set(XeGetCoreLib(CURRENT, name_hash)->Get(isolate));

			return;
		}

		throw std::runtime_error("Dependency " + std::string(name_hash) + " does not exist. Requested by: " + std::to_string(comp->id));
	}
};

namespace XE_API_BOOTSTRAPPABLE {
	void export_f(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto comp = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext()).bootstrappable;
		//comp->exports.Reset(args.GetIsolate(), args[0]);
	}

	void export_f_as(const v8::FunctionCallbackInfo<v8::Value>& args) {
		//auto comp = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext())->bootstrappable;
		//auto isolate = args.GetIsolate();
		//if (comp->exports.IsEmpty()) {
		//	Local<ObjectTemplate> object = ObjectTemplate::New(args.GetIsolate());
		//	object->Set(args.GetIsolate(), ToCString(String::Utf8Value(args.GetIsolate(), args[0])), args[1]);
		//	//comp->exports.Reset(args.GetIsolate(), object);
		//}
		//else {
		//	if (comp->exports.Get(isolate)->IsObject()) {
		//		auto lc = comp->exports.Get(isolate)->ToObject(args.GetIsolate()->GetCurrentContext()).ToLocalChecked();
		//		Local<Value> vx = Local<Value>(args[1]);
		//		lc->Set(args.GetIsolate()->GetCurrentContext(), args[0], args[1]);
		//		//comp->exports.Reset(args.GetIsolate(), lc);
		//	}
		//	else {
		//		throw std::runtime_error("Cannot 'export as' when the currently exported object is not an object.");
		//	}
		//}
	}

	void require_f(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto comp = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext()).bootstrappable;
		auto name_hash = ToCString(String::Utf8Value(isolate, args[0]));

		if (XeDoesCoreLibExist(CURRENT, name_hash)) {
			PLOGV << "Resolving dependency " << name_hash << " for " << comp->id << ".\nDependency Type: CORE_LIB";
			//args.GetReturnValue().Set(XeGetCoreLib(CURRENT, name_hash)->Get(isolate));

			return;
		}

		throw std::runtime_error("Dependency " + std::string(name_hash) + " does not exist. Requested by: " + std::to_string(comp->id));
	}

	void set_entity_position(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();

		auto entity = args[0]->IntegerValue(context).ToChecked();

		auto vector = UnpackVector(args[1], context);
		auto entity_i = XeGetInstance<XeMeshedEntity>(CURRENT, entity);
		entity_i->Position = glm::vec3(vector.x, vector.y, vector.z);
		entity_i->Update();
	}

	void set_entity_rotation(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();

		auto entity = args[0]->IntegerValue(context).ToChecked();

		auto vector = UnpackVector(args[1], context);
		auto entity_i = XeGetInstance<XeMeshedEntity>(CURRENT, entity);
		entity_i->Orientation = glm::quat(glm::vec3(vector.x, vector.y, vector.z));
		entity_i->Update();
	}

	void set_entity_scale(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();

		auto entity = args[0]->IntegerValue(context).ToChecked();

		auto vector = UnpackVector(args[1], context);
		auto entity_i = XeGetInstance<XeMeshedEntity>(CURRENT, entity);
		entity_i->Scale = glm::vec3(vector.x, vector.y, vector.z);
		entity_i->Update();
	}

	void get_entity_position(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();

		auto entity = args[0]->IntegerValue(context).ToChecked();

		auto obj_t = ObjectTemplate::New(isolate);
		auto entity_i = XeGetInstance<XeMeshedEntity>(CURRENT, entity);

		auto avx = entity_i->Position;

		CreateVector(isolate, obj_t, avx.x, avx.y, avx.z);

		args.GetReturnValue().Set(obj_t);
	}

	void get_entity_scale(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();

		auto entity = args[0]->IntegerValue(context).ToChecked();

		auto obj_t = ObjectTemplate::New(isolate);
		auto entity_i = XeGetInstance<XeMeshedEntity>(CURRENT, entity);

		auto avx = entity_i->Scale;

		CreateVector(isolate, obj_t, avx.x, avx.y, avx.z);

		args.GetReturnValue().Set(obj_t);
	}

	void get_entity_rotation(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();

		auto entity = args[0]->IntegerValue(context).ToChecked();

		auto obj_t = ObjectTemplate::New(isolate);
		auto entity_i = XeGetInstance<XeMeshedEntity>(CURRENT, entity);

		

		auto avx = glm::eulerAngles(entity_i->Orientation);

		CreateVector(isolate, obj_t, avx.x, avx.y, avx.z);

		args.GetReturnValue().Set(obj_t);
	}

	void set_camera_pos(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();


		auto vector = UnpackVector(args[0], context);
		CURRENT->scene.Camera.Position = glm::vec3(vector.x, vector.y, vector.z);
	}
	/*
	I lied. This sets the look-at vector. Muahahahahaha...
	*/
	void set_camera_rotation(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();


		auto vector = UnpackVector(args[0], context);
		CURRENT->scene.Camera.LookAt = glm::vec3(vector.x, vector.y, vector.z);
	}

	void get_camera_pos(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();



		auto obj_t = ObjectTemplate::New(isolate);

		auto avx = CURRENT->scene.Camera.Position;

		CreateVector(isolate, obj_t, avx.x, avx.y, avx.z);

		args.GetReturnValue().Set(obj_t);
	}

	void get_camera_rotation(const v8::FunctionCallbackInfo<v8::Value>& args) {
		auto isolate = args.GetIsolate();
		auto context = args.GetIsolate()->GetCurrentContext();



		auto obj_t = ObjectTemplate::New(isolate);

		auto avx = CURRENT->scene.Camera.LookAt;

		CreateVector(isolate, obj_t, avx.x, avx.y, avx.z);

		args.GetReturnValue().Set(obj_t);
	}

	void translate_camera(const v8::FunctionCallbackInfo<v8::Value>& args) {
		PLOGW << "'translate_camera' is not implemented and will not work. Sorry Charlie!";
	}

	void translate_entity(const v8::FunctionCallbackInfo<v8::Value>& args) {
		PLOGW << "'translate_entity' is not implemented and will not work. Sorry Charlie!";
	}



};