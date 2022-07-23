#pragma once
#include "xenon.h"

inline XenonEngine* CURRENT;

namespace XE_API_COMPONENT {
	void export_f(const v8::FunctionCallbackInfo<v8::Value>& args);
	void export_f_as(const v8::FunctionCallbackInfo<v8::Value>& args);

	void require_f(const v8::FunctionCallbackInfo<v8::Value>& args);
};

namespace XE_API_BOOTSTRAPPABLE {
	void set_entity_position(const v8::FunctionCallbackInfo<v8::Value>& args);
	void set_entity_rotation(const v8::FunctionCallbackInfo<v8::Value>& args);
	void set_entity_scale(const v8::FunctionCallbackInfo<v8::Value>& args);

	void get_entity_position(const v8::FunctionCallbackInfo<v8::Value>& args);
	void get_entity_rotation(const v8::FunctionCallbackInfo<v8::Value>& args);
	void get_entity_scale(const v8::FunctionCallbackInfo<v8::Value>& args);

	void set_camera_pos(const v8::FunctionCallbackInfo<v8::Value>& args);
	void set_camera_rotation(const v8::FunctionCallbackInfo<v8::Value>& args);
	
	void get_camera_pos(const v8::FunctionCallbackInfo<v8::Value>& args);
	void get_camera_rotation(const v8::FunctionCallbackInfo<v8::Value>& args);

	void translate_camera(const v8::FunctionCallbackInfo<v8::Value>& args);
	void translate_entity(const v8::FunctionCallbackInfo<v8::Value>& args);

	void export_f(const v8::FunctionCallbackInfo<v8::Value>& args);
	void export_f_as(const v8::FunctionCallbackInfo<v8::Value>& args);

	void require_f(const v8::FunctionCallbackInfo<v8::Value>& args);
};

inline void APPLY_COMPONENT_API(XenonEngine* engine, v8::Local<v8::ObjectTemplate> host) {
	auto isolate = XeGetIsolate(engine);
	host->Set(isolate, "export_module", v8::FunctionTemplate::New(isolate, XE_API_COMPONENT::export_f));
	host->Set(isolate, "export_as", v8::FunctionTemplate::New(isolate, XE_API_COMPONENT::export_f_as));
	host->Set(isolate, "require", v8::FunctionTemplate::New(isolate, XE_API_COMPONENT::require_f));
}

inline void APPLY_BOOTSTRAP_API(XenonEngine* engine, v8::Local<v8::ObjectTemplate> host) {
	auto isolate = XeGetIsolate(engine);
	host->Set(isolate, "export_module", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::export_f));
	host->Set(isolate, "export_as", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::export_f_as));
	host->Set(isolate, "require", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::require_f));

	host->Set(isolate, "set_entity_position", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::set_entity_position));
	host->Set(isolate, "set_entity_rotation", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::set_entity_rotation));
	host->Set(isolate, "set_entity_scale", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::set_entity_scale));

	host->Set(isolate, "get_entity_position", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::get_entity_position));
	host->Set(isolate, "get_entity_rotation", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::get_entity_rotation));
	host->Set(isolate, "get_entity_scale", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::get_entity_scale));

	host->Set(isolate, "set_camera_position", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::set_camera_pos));
	host->Set(isolate, "set_camera_rotation", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::set_camera_rotation));

	host->Set(isolate, "get_camera_position", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::get_camera_pos));
	host->Set(isolate, "get_camera_rotation", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::get_camera_rotation));

	host->Set(isolate, "translate_camera", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::translate_camera));
	host->Set(isolate, "translate_entity", v8::FunctionTemplate::New(isolate, XE_API_BOOTSTRAPPABLE::translate_entity));
}