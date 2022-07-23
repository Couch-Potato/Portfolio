#pragma once
#include <string>

#define XE_F_VERSION_MAJOR 1;
#define XE_F_VERSION_MINOR 1;

const size_t TYPE_ENTITY = 0x0;
const size_t TYPE_MATERIAL = 0x1;
const size_t TYPE_SHADER = 0x2;
const size_t TYPE_SCENE = 0x3;
const size_t TYPE_COMPONENT = 0x4;
const size_t TYPE_BOOTSTRAPPABLE = 0x5;
const size_t TYPE_BOOTSTRAP_REGISTRY = 0x6;

struct STRUCT_DEFINITION_ITEM {
    std::string typeName;
    std::string name;
};

struct STRUCT_DEFINITIONS {
    std::string name;
    std::vector<STRUCT_DEFINITION_ITEM> definition;
};

struct BUFFER_STRUCT_PROPERTY_ENTRY {
    size_t name;
    size_t offset;
    size_t len;
    size_t type;
};

struct VERSION_HEADER {
    char MAJOR;
    char MINOR;
    size_t TABLE_SIZE;
};

struct IMP_HASHABLE {
    size_t hash;
};

struct IMP_SHADER : IMP_HASHABLE {
    size_t vertex_shader_index;
    size_t fragment_shader_index;
    size_t buffers;
};

struct IMP_MATERIAL : IMP_HASHABLE {
    size_t shader_id;
    size_t shader_prop_set;
};

struct IMP_DEV_BINARY {
    size_t hash;
    size_t fileId;
};

struct IMP_SHADER_LIB : IMP_HASHABLE {
    size_t includes;
    size_t type;
    size_t version;
};

struct IMP_MATERIAL_PROPERTY {
    size_t type;
    size_t name;
    size_t value;
};

struct IMP_MESH {
    size_t type;
    size_t index;
};

struct IMP_ENTITY : IMP_HASHABLE {
    size_t material_id;
    size_t mesh_type;
    size_t mesh_index;
    size_t children_set;
    size_t component_list;
};

struct IMP_BOOTSTRAPPABLE : IMP_HASHABLE {
    size_t script_vec_location;
};

struct IMP_BOOTSTRAP_REGISTRY : IMP_HASHABLE {
    size_t bs_vec_location;
};

struct IMP_COMPONENT_DEF : IMP_HASHABLE {
    size_t script_vec_location; 

};

struct IMP_SCRIPT : IMP_HASHABLE {
    size_t location;
};

struct IMP_ENTRY_PRECEEDER {
    size_t entry_length;
    size_t entry_type;
};

struct IMP_IMPORTER {
    size_t type;
    size_t source;
    size_t param1;
};

struct IMP_SHADER_BUFFER {
    size_t bufferType;
    size_t bufferName;
    IMP_IMPORTER bufferImporter;
};

struct IMP_CHILDREN {
    size_t name;
};



struct IMP_VECTOR3 {
    size_t X;
    size_t Y;
    size_t Z;
};

struct IMP_ENTITY_CHILD {
    size_t name;
    IMP_VECTOR3 position;
    IMP_VECTOR3 scale;
    IMP_VECTOR3 rotation;
};

struct IMP_SCENE : IMP_HASHABLE {
    size_t children;
};

struct ENTRY {
    void* entryData;
    size_t entryLength;
};

struct ENTRY_INFO {
    char* entryBytes;
    size_t entryType;
};
struct FILE_ENTRY_INFO {
    size_t fileStart;
    size_t fileLen;
    std::string archive;
};

#pragma region VERSION 1_0

struct VERSION_1_0 {
    struct IMP_ENTITY : IMP_HASHABLE {
        size_t material_id;
        size_t mesh_type;
        size_t mesh_index;
        size_t children_set;
    };
};

#pragma endregion