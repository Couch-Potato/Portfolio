#pragma once
#include "xenon.h"
#include "xe_api.h"
#include <libplatform/libplatform.h>
//XenonEngine* CURRENT;
/*
Ok. I do not wanna hear it on this. wah wah wah wah wah blah blah blah.
Fuck object oriented programming when trying to implement this.
*/

void XE_S_PRINT(const v8::FunctionCallbackInfo<v8::Value>& args) {
	auto ctxData = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext());
	if (ctxData.resultFound) {
		if (ctxData.isComponent) {
			PLOGI << "SCRIPT_MSG: [@COMPONENT_" << ctxData.component->id << "] " << *v8::String::Utf8Value(args.GetIsolate(), args[0]);
		}
		if (!ctxData.isComponent) {
			PLOGI << "SCRIPT_MSG: [@BSTRAP_" << ctxData.bootstrappable->id << "] " << *v8::String::Utf8Value(args.GetIsolate(), args[0]);
		}
	}
	
}
void XE_S_WARN(const v8::FunctionCallbackInfo<v8::Value>& args) {
	auto ctxData = XeGetScriptFromContext(CURRENT, args.GetIsolate()->GetCurrentContext());
	if (ctxData.resultFound) {
		if (ctxData.isComponent) {
			PLOGW << "SCRIPT_WARN: [@COMPONENT_" << ctxData.component->id << "] " << *v8::String::Utf8Value(args.GetIsolate(), args[0]);
		}
		if (!ctxData.isComponent) {
			PLOGW << "SCRIPT_WARN: [@BSTRAP_" << ctxData.bootstrappable->id << "] " << *v8::String::Utf8Value(args.GetIsolate(), args[0]);
		}
	}
}

void XeComponent::Run() {
	CURRENT = Engine;
	auto isolate = XeGetIsolate(Engine);

	v8::Local<v8::ObjectTemplate> global = v8::ObjectTemplate::New(isolate);

	// Do API building here!!!

	if (isAttachedToEntity) {
		auto ix = v8::Integer::NewFromUnsigned(XeGetIsolate(CURRENT), linkedEntity->handle);
		global->Set(XeGetIsolate(CURRENT), "AttachedEntity", ix);
		//auto ix = v8::Integer::NewFromUnsigned(XeGetIsolate(Engine), linkedEntity->handle);
		//global->Set(XeGetIsolate(Engine), "AttachedEntity", ix);
	}

	global->Set(XeGetIsolate(Engine), "IsAttached", v8::Boolean::New(XeGetIsolate(Engine), isAttachedToEntity));

	global->Set(XeGetIsolate(Engine), "warn", v8::FunctionTemplate::New(XeGetIsolate(Engine), XE_S_WARN));
	global->Set(XeGetIsolate(Engine), "print", v8::FunctionTemplate::New(XeGetIsolate(Engine), XE_S_PRINT));

	APPLY_COMPONENT_API(Engine, global);

	v8::Local<v8::Context> i_context = v8::Context::New(isolate, nullptr, global);
	v8::Context::Scope context_scope(i_context);

	for (auto const& [key, val] : scripts) {
		v8::TryCatch trycatch(isolate);

		v8::Local<v8::String> source = v8::String::NewFromUtf8(isolate, val.c_str()).ToLocalChecked();

		v8::MaybeLocal<v8::Script> compiled = v8::Script::Compile(i_context, source);

		if (compiled.IsEmpty())
		{
			PLOGE << "Failed to compile script (" << id << "). ";
			v8::Local<v8::Value> exception = trycatch.Exception();
			v8::String::Utf8Value exception_str(isolate, exception);
			PLOGE << "Exception info: " << *exception_str;
			return;
		}

		v8::Local<v8::Script> script = compiled.ToLocalChecked();

		Engine->scriptEngine.contexts.push_back(i_context);
		context = i_context;
		Engine->scriptEngine.runningComponents.push_back(this);

		v8::MaybeLocal<v8::Value> result = script->Run(i_context);

		if (result.IsEmpty()) {
			v8::Local<v8::Value> exception = trycatch.Exception();
			v8::String::Utf8Value exception_str(isolate, exception);
			PLOGE << "Exception thrown in component " << id << " @ script " << key;
			PLOGE << *exception_str;
		}
	}
}

void XeComponent::Attach(XeMeshedEntity* entity) {
	isAttachedToEntity = true;
	linkedEntity = entity;
}

void XeComponent::Dispose() {
	
}

v8::Isolate* XeGetIsolate(XenonEngine* engine) {
	return engine->scriptEngine.isolate;
}

void XeBootstrappable::Run() {
	CURRENT = Engine;
	auto isolate = XeGetIsolate(Engine);

	v8::Local<v8::ObjectTemplate> global = v8::ObjectTemplate::New(isolate);

	// Do API building here!!!
	global->Set(XeGetIsolate(Engine), "warn", v8::FunctionTemplate::New(XeGetIsolate(Engine), XE_S_WARN));
	global->Set(XeGetIsolate(Engine), "print", v8::FunctionTemplate::New(XeGetIsolate(Engine), XE_S_PRINT));

	APPLY_BOOTSTRAP_API(Engine, global);
	v8::Local<v8::Context> i_context;

	try {
		i_context = v8::Context::New(isolate, nullptr, global);
	}
	catch (const std::exception& e) {
		PLOGF << "V8 Context Initialization Error. " << e.what();
		throw std::runtime_error("V8 Context Initialization Error. If you see this, it is because the developers of the V8 engine suck massive fucking donkey dick and didn't implement proper exception handling for this part. Go complain to them for being fucking stupid as shit.");

	}
	

	v8::Context::Scope context_scope(i_context);

	

	for (auto const& [key, val] : scripts) {
		v8::TryCatch trycatch(isolate);

		v8::Local<v8::String> source = v8::String::NewFromUtf8(isolate, val.c_str()).ToLocalChecked();

		v8::MaybeLocal<v8::Script> compiled = v8::Script::Compile(i_context, source);


		if (compiled.IsEmpty())
		{
			PLOGE << "Failed to compile script (" << id << "). ";
			v8::Local<v8::Value> exception = trycatch.Exception();
			v8::String::Utf8Value exception_str(isolate, exception);
			PLOGE << "Exception info: " << *exception_str;
			return;
		}

		v8::Local<v8::Script> script = compiled.ToLocalChecked();

		Engine->scriptEngine.contexts.push_back(i_context);
		context = i_context;

		v8::MaybeLocal<v8::Value> result = script->Run(i_context);

		if (result.IsEmpty()) {
			v8::Local<v8::Value> exception = trycatch.Exception();
			v8::String::Utf8Value exception_str(isolate, exception);
			PLOGE << "Exception thrown in bootstrappable " << id << " @ script " << key;
			PLOGE << *exception_str;
		}
	}
}

void XeBootstrappable::AddScript(std::size_t id, std::string content) {
	scripts.insert(std::pair<size_t, std::string>(id, content));
}

void XeComponent::AddScript(std::size_t id, std::string content) {
	scripts.insert(std::pair<size_t, std::string>(id, content));
}

v8::Global<v8::Value>* XeGetComponent(XeMeshedEntity* entity, std::string component_name) {
	return XeGetComponent(entity, XeHashString(component_name));
}
v8::Global<v8::Value>* XeGetComponent(XeMeshedEntity* entity, size_t hash) {
	return &entity->components[hash]->exports;
}
v8::Global<v8::Value>* XeGetCoreLib(XenonEngine* engine, std::string name) {
	return XeGetCoreLib(engine, XeHashString(name));
}
v8::Global<v8::Value>* XeGetCoreLib(XenonEngine* engine, size_t hash) {
	return &engine->scriptEngine.coreScripts[hash]->exports;
}

bool XeDoesCoreLibExist(XenonEngine* engine, size_t hash) {
	if (engine->scriptEngine.coreScripts.find(hash) == engine->scriptEngine.coreScripts.end()) {
		return false;
	}
	return true;
}

bool XeDoesCoreLibExist(XenonEngine* engine, std::string name) {
	return XeDoesCoreLibExist(engine, XeHashString(name));
}

bool XeDoesComponentExist(XeMeshedEntity* entity, std::string component_name) {
	return XeDoesComponentExist(entity, XeHashString(component_name));
}

bool XeDoesComponentExist(XeMeshedEntity* entity, size_t hash) {
	if (entity->components.find(hash) == entity->components.end()) {
		return false;
	}
	return true;
}

XeGetScriptFromContextResult  XeGetScriptFromContext(XenonEngine* engine, v8::Local<v8::Context> ctx) {
	XeGetScriptFromContextResult rx;
	
	rx.resultFound = false;
	for (auto const cmx : engine->scriptEngine.runningComponents) {
		if (cmx->context == ctx) {
			rx.component = cmx;
			rx.isComponent = true;
			rx.resultFound = true;
			break;
		}
	}
	for (auto const& [key, cmx] : engine->scriptEngine.coreScripts) {
		if (cmx->context == ctx) {
			rx.bootstrappable = cmx;
			rx.isComponent = false;
			rx.resultFound = true;
			break;
		}
	}


	return rx;
}
void XeScriptEngine::InitIsolate(const char* su) {
	PLOGI << "Initilizing script engine";
	v8::V8::InitializeICUDefaultLocation(su);
	v8::V8::InitializeExternalStartupData(su);
	
	std::unique_ptr<v8::Platform> platform = v8::platform::NewDefaultPlatform();
	v8::V8::InitializePlatform(platform.get());
	v8::V8::Initialize();

	v8::Isolate::CreateParams params;
	params.array_buffer_allocator = v8::ArrayBuffer::Allocator::NewDefaultAllocator();
	isolate = v8::Isolate::New(params);

}