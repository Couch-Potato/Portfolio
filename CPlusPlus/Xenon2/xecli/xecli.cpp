// xecli.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <vector>
#include <fstream>
#include <algorithm>
#include <string>
#include <vector>
#include <map>
#include <string> 
#include <shaderc/shaderc.hpp>
#include "rapidxml.hpp"
#include <filesystem>
#include <xe_types.h>

size_t HashString(std::string str) {
    return std::hash<std::string>{}(str);
}

IMP_VECTOR3* VectorizeNode(rapidxml::xml_node<>* node) {
    auto v = new IMP_VECTOR3;
    
    v->X = std::stoi(node->first_attribute("x")->value());
    v->Y = std::stoi(node->first_attribute("y")->value());
    v->Z = std::stoi(node->first_attribute("z")->value());

    return v;
}

/// <summary>
/// Allocates an entry buffer for an item and writes to it.
/// </summary>
/// <param name="entry">The entry</param>
/// <returns>The entry buffer</returns>
/// <remarks>Make sure to unallocate memory after disposing of object.</remarks>
template <class T>
void* writeEntry(T entry, size_t type) {
    void* toWriteTo = malloc(sizeof(entry) + sizeof(IMP_ENTRY_PRECEEDER));
    auto precede = IMP_ENTRY_PRECEEDER{};
    precede.entry_length = sizeof(entry);
    precede.entry_type = type;
    if (toWriteTo != 0) {
        memcpy(toWriteTo, &precede, sizeof(IMP_ENTRY_PRECEEDER));
    }
    else {
        throw std::runtime_error("Failed to create virtual entry buffer....");
    }
    void* offset = (void*)((char*)toWriteTo + sizeof(IMP_ENTRY_PRECEEDER));
    memcpy(offset, &entry, sizeof(entry));
    return toWriteTo;
}

template <class T>
void* writeStructToBuffer(T type) {
    void* writeTo = malloc(sizeof(type));
    memcpy(writeTo, &type, sizeof(type));
    return writeTo;
}

struct UNMAMAGED_ENTRY {
    void* data;
    size_t size;
};

struct ORGANIZED_ENTRY {
    std::string data;
    std::string entry_name;
    size_t size;
};

struct file_read_return {
    char* data;
    size_t size;
};

file_read_return* READ_FILE(std::string file) {
    FILE* f = fopen(file.c_str(), "rb");
    fseek(f, 0, SEEK_END);
    size_t fsize = ftell(f);
    fseek(f, 0, SEEK_SET);

    char* fsx = (char*)malloc(fsize + 1);
    fread_s(fsx, fsize, 1, fsize, f);

    fclose(f);

    file_read_return *ret = new file_read_return{};
    ret->data = fsx;
    ret->size = fsize;

    return ret;
}
file_read_return* READ_TEXT(std::string file) {
    FILE* f = fopen(file.c_str(), "rb");
    fseek(f, 0, SEEK_END);
    size_t fsize = ftell(f);
    fseek(f, 0, SEEK_SET);

    char* fsx = (char*)malloc(fsize + 1);
    fread_s(fsx, fsize, 1, fsize, f);

    fclose(f);

    file_read_return* ret = new file_read_return{};
    ret->data = fsx;
    ret->size = fsize;

    return ret;
}

bool exists(const std::string& name) {
    std::ifstream f(name.c_str());
    return f.good();
}


class BYTE_BUFFER {
private:
    struct BUFFER {
        void* item;
        size_t size;
        bool isMultiScalar;
    };
    std::vector<BUFFER> buffers;
public:
    void Write(void* item, size_t size, bool isMulti = false) {
        buffers.push_back({item, size, isMulti});
    }

    char* GetBytes() {
        size_t bufferSize = 0;
        size_t incrementer = 0;
        bool hasMultiScalar = false;

        for (const auto entry : buffers) {
            if (entry.isMultiScalar)
                hasMultiScalar = true;
        }

        for (const auto entry : buffers) {
            // We have to align everything as 4x the size the float
            if (hasMultiScalar) {
                bufferSize += sizeof(float) * 4;
                incrementer = sizeof(float) * 4;
            }
            else {
                bufferSize += entry.size;
                if (bufferSize > incrementer) {
                    incrementer = bufferSize;
                }
            }
        }

        // allocate the new buffer now that we have defined its size.
        void* bufferToWriteTo = malloc(bufferSize);
        size_t offset = 0;

        for (const auto entry : buffers) {
            memcpy((char*)bufferToWriteTo + offset, entry.item, entry.size);
            offset += incrementer;
        }

        return (char*)bufferToWriteTo;
    }
    size_t size() {
        size_t bufferSize = 0;
        size_t incrementer = 0;
        bool hasMultiScalar = false;

        for (const auto entry : buffers) {
            if (entry.isMultiScalar)
                hasMultiScalar = true;
        }

        for (const auto entry : buffers) {
            // We have to align everything as 4x the size the float
            if (hasMultiScalar) {
                bufferSize += sizeof(float) * 4;
                incrementer = sizeof(float) * 4;
            }
            else {
                bufferSize += entry.size;
                if (bufferSize > incrementer) {
                    incrementer = bufferSize;
                }
            }
        }

        return bufferSize;
    }
    void Dispose() {
        for (const auto buffer : buffers) {
            free(buffer.item);
        }
    }
    ~BYTE_BUFFER() {
        Dispose();
    }
};


class DYSTRO_ARCHIVE;

DYSTRO_ARCHIVE* TOOLS_ARCHIVE;

class DYSTRO_INCLUDER : public shaderc::CompileOptions::IncluderInterface {
public:
    explicit DYSTRO_INCLUDER() {}

    ~DYSTRO_INCLUDER() override {
        
    };

    shaderc_include_result* GetInclude(const char* requested_source,
        shaderc_include_type type,
        const char* requesting_source,
        size_t include_depth) override;

    void ReleaseInclude(shaderc_include_result* include_result) override {
        delete include_result;
    };
};


const std::string STANDARD_DEFINITIONS = "struct XE_LIGHT_DIRECTIONAL { bool enabled; vec3 direction; vec3 ambient; vec3 diffuse; vec3 specular;\n mat4 lightSpaceMatrix; }; struct XE_LIGHT_POINT { bool enabled; vec3 position; vec3 ambient; vec3 diffuse; vec3 specular; float constant; float linear; float quadratic; }; struct XE_LIGHT_SPOT { bool enabled; vec3 position; vec3 direction; vec3 ambient; vec3 diffuse; vec3 specular; float cutOff; float outerCutOff; float constant; float linear; float quadratic; }; layout(set = 0, binding = 0) uniform UniformBufferObject { mat4 view; mat4 projection; vec3 viewPos; XE_LIGHT_DIRECTIONAL directional; XE_LIGHT_SPOT spots[4]; XE_LIGHT_POINT points[4]; } SCENE; layout( push_constant ) uniform constants { mat4 world; } ENTITY;";

struct x_buffer_t {
    std::string bufferName;
    uint16_t type;
    int structBufferId;
};

std::vector<uint32_t> COMPILE_SHADER(std::string shaderName, std::string shaderItem, shaderc_shader_kind kind, DYSTRO_ARCHIVE* tools, shaderc::CompileOptions options, std::map<std::string, std::string> c_opts, std::string input_name, std::vector<x_buffer_t> buffers, std::vector<STRUCT_DEFINITIONS> defs) {
    std::cout << "Building shader: " << shaderName << "\n";
    std::string importDirectives = "";
    std::string regProc = "";
    options.AddMacroDefinition("DYS_COMP_VERS", "1");

    TOOLS_ARCHIVE = tools;

    shaderc::Compiler comp;
    //options.SetForcedVersionProfile(450, shaderc_profile::shaderc_profile_core);


    
    std::string bufferData = "";
    int i = 1;
    for (auto item : buffers) {
        // uniform
        if (item.type == 0) {
            // I NEED TO REDO THIS!!!
            
            bufferData += "layout(set = 0, binding = " + std::to_string(i) + ") uniform " + item.bufferName + "_t { uint64_t vx; } " + item.bufferName + "_v;" + item.bufferName + " = " + item.bufferName + "_v.vx;\n";
        }
        //texture
        else if (item.type == 1) {
            bufferData += "layout(binding = " + std::to_string(i) + ") uniform sampler2D " + item.bufferName + ";\n";
        }
        else if (item.type == 2) {
            STRUCT_DEFINITIONS stdef = defs[item.structBufferId];
            std::string itemSet = "";
            for (const auto xtem : stdef.definition) {
                itemSet += (xtem.typeName + " " + xtem.name + ";\n");
            }
            bufferData += ("layout(set = 0, binding = " + std::to_string(i) + ") uniform " + item.bufferName + "_t { " + itemSet + " } " + item.bufferName + ";\n");
        }
        i++;
    }

    std::istringstream strStm(shaderItem);
    
    for (std::string line; std::getline(strStm, line);) {
        if (line.find("#") == 0) {
            importDirectives += line + "\n";
        }
        else {
            //std::cout << line + "\n";
            regProc += line + "\r\n";
        }
    }

    std::string finishedShader = "#version 450\n#extension GL_ARB_separate_shader_objects : enable\n" + importDirectives + STANDARD_DEFINITIONS + bufferData + regProc;

    //std::cout << finishedShader;
    
    for (auto [key, val] : c_opts) {
        options.AddMacroDefinition(key, val);
    }

    options.SetIncluder(std::make_unique<DYSTRO_INCLUDER>());


    shaderc::PreprocessedSourceCompilationResult prepox = comp.PreprocessGlsl(finishedShader, kind, input_name.c_str(), options);

    if (prepox.GetCompilationStatus() != shaderc_compilation_status_success) {
        std::cout << "#############################################################################\n";
        std::cout << "                                SHADER ERROR\n";
        std::cout << "Source: @" + shaderName + "/" + input_name + "\n";
        std::cout << "Error: " << prepox.GetErrorMessage() + "\n";
        std::cout << "Step: Preprocessing\n";
        std::cout << "#############################################################################\n";
        return std::vector<uint32_t>();
    }
    else {
        std::cout << "Compiling @" + shaderName + "/" + input_name + "\n";
        std::cout << prepox.GetNumWarnings() << " warning(s)\n";
    }

    shaderc::SpvCompilationResult module = comp.CompileGlslToSpv(finishedShader, kind, input_name.c_str(), options);

    if (module.GetCompilationStatus() != shaderc_compilation_status_success) {
        std::cout << "#############################################################################\n";
        std::cout << "                                SHADER ERROR\n";
        std::cout << "Source: @" + shaderName + "/" + input_name + "\n";
        std::cout << "Error: " << module.GetErrorMessage() + "\n";
        std::cout << "Step: Compilation\n";
        std::cout << "#############################################################################\n";
        return std::vector<uint32_t>();
    }

    std::cout << "Compiled @" + shaderName + "/" + input_name + "\n";

    return { module.cbegin(), module.cend() };
}

class DYSTRO_ARCHIVE {
private:
    std::map<std::string, size_t> fileSpace;
    std::vector<ENTRY> entries;
    std::map<size_t, ENTRY_INFO> entries_read;
    std::map<size_t, FILE_ENTRY_INFO> file_entries;
    std::vector<UNMAMAGED_ENTRY> unmanaged_entries;
    std::vector<std::string> write_order;
    std::vector<IMP_BOOTSTRAPPABLE> bootstraps;
    size_t currentSize = 0;
    size_t currentFile_Size = 0;
public:
    template <class T>
    void WriteEntry(T entry, size_t Type) {
        auto data = writeEntry<T>(entry, Type);
        ENTRY ent{};
        ent.entryData = data;
        ent.entryLength = GetEntrySize<T>();
        entries.push_back(ent);
        currentSize += GetEntrySize<T>();
    }
    template <class T>
    size_t GetEntrySize() {
        return sizeof(T) + sizeof(IMP_ENTRY_PRECEEDER);
    }
    size_t AllocateFileSpace(std::string location) {

        std::ifstream infile(location , std::ofstream::binary);
        infile.seekg(0, std::ios::end);
        size_t length = infile.tellg();
        infile.seekg(0, std::ios::beg);
        infile.close();

        // DO SPACE SIZE!
        fileSpace.insert(std::pair<std::string, size_t>(location, currentFile_Size));
        write_order.push_back(location);
        auto tempLoc = currentFile_Size;
        currentFile_Size += length;
        return tempLoc;
    }
    size_t AllocateUnmanaged(void* item, size_t size) {
        UNMAMAGED_ENTRY entry{};

        // translate the buffer to our own one such that it does not get deleted at the exit of the stack frame.
        void* newData = malloc(size);
        memcpy(newData, item, size);

        entry.data = newData;
        entry.size = size;

        unmanaged_entries.push_back(entry);
        auto tempLoc = currentFile_Size;


        fileSpace.insert(std::pair<std::string, size_t>("BASE::" + std::to_string((unmanaged_entries.size() - 1)), currentFile_Size));
        write_order.push_back("BASE::" + std::to_string((unmanaged_entries.size() - 1)));
        currentFile_Size += size;
        return tempLoc;
    }
    template <class T>
    size_t WriteVector(std::vector<T> dg) {
        size_t stackSize = sizeof(T) * dg.size();
        if (dg.size() == 0) return -1;
        return AllocateUnmanaged(dg.data(), stackSize);
    }
    size_t GetIndexTableSize() {
        return currentSize;
    }
    void WriteToFile(std::string output) {
     
        // Handle all of the bootstrappable nonsense 
        // TODO: Invent a flagging system.


        IMP_BOOTSTRAP_REGISTRY reg{};
        auto vecLoc = WriteVector(bootstraps);
        reg.hash = HashString("BOOTSTRAP_" + (rand() % 99999999));
        reg.bs_vec_location = vecLoc;
        WriteEntry(reg, TYPE_BOOTSTRAP_REGISTRY);
        
        std::fstream outfile(output, std::ios::binary | std::ios::out);

        // Create our version header;

        VERSION_HEADER header{};
        header.MAJOR = XE_F_VERSION_MAJOR;
        header.MINOR = XE_F_VERSION_MINOR;
        header.TABLE_SIZE = GetIndexTableSize();

        // Write the version header

        auto headerBuffer = writeStructToBuffer<VERSION_HEADER>(header);
        outfile.write((char*)headerBuffer, sizeof(header));
        delete headerBuffer;

        // Write the entries

        for(auto entry : entries) {
            outfile.write((char*)entry.entryData, entry.entryLength);
        }

        // Open, read, and write all of the files to be appended at the end.

        for (auto key : write_order) {
            auto val = fileSpace[key];

            // Write unmanaged allocations
            if (key.substr(0, 6) == "BASE::") {
                auto imp = key.substr(6, key.length() - 6);
                auto ign = std::stoi(imp);
                outfile.write((char*)writeStructToBuffer<size_t>(unmanaged_entries[ign].size), sizeof(size_t));
                outfile.write((char*)unmanaged_entries[ign].data, unmanaged_entries[ign].size);
                continue;
            }

            std::ifstream infile;
            if (infile.is_open()) infile.close();
            infile.open(key, std::ios::binary);

            infile.seekg(0, std::ios::end);
            size_t length = infile.tellg();
            infile.seekg(0, std::ios::beg);

            char* buffer = (char*)malloc(length);
            infile.read(buffer, length);

            outfile.write((char*)writeStructToBuffer<size_t>(length), sizeof(size_t));

            outfile.write(buffer, length);
            outfile.tellp();
            infile.seekg(0, std::ios::end);
            infile.close();

            free(buffer);
        }

        outfile.close();
    }
    void ReadFromFile(std::string fileName) {
        std::ifstream inStream;
        inStream.open(fileName, std::ios::binary);

        inStream.seekg(0, std::ios::end);
        size_t fileLen = inStream.tellg();

        inStream.seekg(0, std::ios::beg);

        //Read the first 16 bytes and use that to construct the version header

        char* headerBuffer = (char*)malloc(sizeof(VERSION_HEADER));

        auto toAdvance = sizeof(VERSION_HEADER);

        inStream.read(headerBuffer, toAdvance);

        auto x2 = sizeof(VERSION_HEADER);

       
        auto curPosxy = inStream.tellg();
        VERSION_HEADER* header = reinterpret_cast<VERSION_HEADER*>(headerBuffer);
        
        size_t tableSize = header->TABLE_SIZE;
        size_t currentRead = 0;

        // Start reading all of the entries in the file and begin parsing / loading in the data

        while (currentRead < tableSize) {
            // Read the first 16 bytes of the entry to get its preceeder
            auto curPosx = inStream.tellg();

            char* precedeBuffer = (char*)malloc(sizeof(IMP_ENTRY_PRECEEDER));
            inStream.read(precedeBuffer, sizeof(IMP_ENTRY_PRECEEDER));
            IMP_ENTRY_PRECEEDER* preceeder = reinterpret_cast<IMP_ENTRY_PRECEEDER*>(precedeBuffer);
            
            auto curPos = inStream.tellg();

            //Read the next 8 bytes to see the hashname.

            char* hashNameBuffer = (char*)malloc(sizeof(size_t));
            inStream.read(hashNameBuffer, sizeof(size_t));
            size_t* hx = reinterpret_cast<size_t*>(hashNameBuffer);

            ENTRY_INFO info{};
            info.entryType = preceeder->entry_type;
            
            // Head back to the start of the struct
            inStream.seekg(curPos, std::ios::beg);


            // Read the entire struct
            char* entryBuffer = (char*)malloc(preceeder->entry_length);
            inStream.read(entryBuffer, preceeder->entry_length);

            info.entryBytes = entryBuffer;

            currentRead += preceeder->entry_length + sizeof(IMP_ENTRY_PRECEEDER);

            entries_read.insert(std::pair<size_t, ENTRY_INFO>(*hx, info));

            free(precedeBuffer);
            free(hashNameBuffer);
        }
        
        size_t currentFileId = 0;

        while (currentRead + 16 < fileLen) {
            // We can now begin reading the file entries

            // Read the first 8 bytes to get the size of the file

            char* hashNameBuffer = (char*)malloc(sizeof(IMP_HASHABLE));
            inStream.read(hashNameBuffer, sizeof(IMP_HASHABLE));
            IMP_HASHABLE* hx = reinterpret_cast<IMP_HASHABLE*>(hashNameBuffer);
            auto actualSize = hx->hash;

            size_t stream_Start = inStream.tellg();

            inStream.seekg(stream_Start + actualSize, std::ios::beg);

            FILE_ENTRY_INFO ifx{};
            ifx.fileLen = actualSize;
            ifx.fileStart = stream_Start;
            ifx.archive = std::filesystem::absolute(fileName).string();

            file_entries.insert(std::pair<size_t, FILE_ENTRY_INFO>(currentFileId, ifx));

            currentFileId += actualSize;

            currentRead += 8 + actualSize;

           

            free(hashNameBuffer);
        }

        inStream.close();
    }
    template <class T>
    T* GetEntry(std::string entryName) {
        char* gx = entries_read[HashString(entryName)].entryBytes;
        return reinterpret_cast<T*>(gx);
    }
    size_t GetFileSize(size_t file) {
        return file_entries[file].fileLen;
    }
    template <class T>
    T* GetEntryFromHash(size_t hash) {
        char* gx = entries_read[hash].entryBytes;
        return reinterpret_cast<T*>(gx);
    }
    void* GetFile(size_t indexId) {
        auto fileEntry = file_entries[indexId];

        std::ifstream inStream;
        inStream.open(fileEntry.archive, std::ios::in | std::ios::binary);
        inStream.seekg(fileEntry.fileStart, std::ios::beg);

        char* fileBuffer = (char*)malloc(fileEntry.fileLen);
        inStream.read(fileBuffer, fileEntry.fileLen);

        inStream.close();
        return fileBuffer;
    }
    template <class T>
    std::vector<T> GetVector(size_t index) {
        /*
            This is a huge slice and dice operation where we have to take the memory region, divide it by the sizeof the type (T), and iterate through each region
            allocating and creating a new buffer. We would then translate these buffered regions into the vector array. 
        */

        auto indexedData = GetFile(index);
        auto fileEntry = file_entries[index];

        auto indvSize = fileEntry.fileLen / sizeof(T);
        std::vector<void*> sliceNDice;
        sliceNDice.resize(indvSize);
        for (int i = 0; i < fileEntry.fileLen; i += sizeof(T))
        {
            auto idx = i == 0 ? 0 : i / sizeof(T);
            auto nDice = malloc(sizeof(T));
            memcpy(nDice, ((char*)indexedData) + i, sizeof(T));
            sliceNDice[idx] = nDice;
        }

        std::vector<T> structured;
        for (auto entry : sliceNDice) {
            structured.push_back(*(reinterpret_cast<T*>(entry)));
            free(entry);
        }
        return structured;
    }
    bool DoesEntryExist(size_t item) {
        return entries_read.count(item);
    }
    void Dispose() {
        for (auto entry : entries) {
           free(entry.entryData);
        }
        for (auto const& [key, val] : entries_read) {
            free(val.entryBytes);
        }
        for (auto entry : unmanaged_entries) {
            free(entry.data);
        }
    }
    void ImportShaderXml(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);
        root_node = doc.first_node("Shader");
        
        IMP_SHADER shader{};

        
        std::vector<STRUCT_DEFINITIONS> definitions;

       
        shader.hash = HashString(root_node->first_attribute("name")->value());

        std::vector<IMP_SHADER_BUFFER> buffers;
        std::vector<x_buffer_t> sbx;

        for (rapidxml::xml_node<>* bufferNode = root_node->first_node("Buffers")->first_node("UniformBuffer"); bufferNode; bufferNode = bufferNode->next_sibling("UniformBuffer")) {
            IMP_SHADER_BUFFER bfx{};
            bfx.bufferType = 0;
            bool isStructBuffer = false;
            int defid = 0;
            int structBufferId;
            bfx.bufferName = HashString(bufferNode->first_attribute("name")->value());
            if (bufferNode->first_node("ImportProperty") != 0) {
                bfx.bufferImporter.type = 0;
                bfx.bufferImporter.source = HashString(bufferNode->first_node("ImportProperty")->first_attribute("src")->value());
            }
            else if (bufferNode->first_node("Struct") != 0) {
                bfx.bufferImporter.type = 4;
                isStructBuffer = true;
                STRUCT_DEFINITIONS defx{};
                defx.name = bufferNode->first_node("Struct")->first_attribute("name")->value();

                BYTE_BUFFER buffer;
                
                for (rapidxml::xml_node<>* sNode = bufferNode->first_node(); sNode; sNode = sNode->next_sibling()) {
                    if (sNode->name() == "Float") {
                        float* val;
                        auto vx = std::strtof(sNode->value(), NULL);

                        val = &vx;

                        defx.definition.push_back({ "float", sNode->first_attribute("name")->value() });

                        buffer.Write(val, sizeof(float));
                    }
                    if (sNode->name() == "Vector3") {
                        struct VEC3 {
                            float x;
                            float y;
                            float z;
                        };

                        VEC3* vec = new VEC3();

                        float xx = std::strtof(sNode->first_node("X")->value(), NULL);
                        float yx = std::strtof(sNode->first_node("Y")->value(), NULL);
                        float zx = std::strtof(sNode->first_node("Z")->value(), NULL);

                        vec->x = xx;
                        vec->y = yx;
                        vec->z = zx;
                       
                        defx.definition.push_back({ "vec3", sNode->first_attribute("name")->value() });

                        buffer.Write(vec, 16, true);
                    }
                    if (sNode->name() == "Vector4") {
                        struct VEC4 {
                            float x;
                            float y;
                            float z;
                            float a;
                        };

                        VEC4* vec = new VEC4();

                        float xx = std::strtof(sNode->first_node("X")->value(), NULL);
                        float yx = std::strtof(sNode->first_node("Y")->value(), NULL);
                        float zx = std::strtof(sNode->first_node("Z")->value(), NULL);
                        float ax = std::strtof(sNode->first_node("A")->value(), NULL);

                        vec->x = xx;
                        vec->y = yx;
                        vec->z = zx;
                        vec->a = ax;

                        defx.definition.push_back({ "vec4", sNode->first_attribute("name")->value() });

                        buffer.Write(vec, 16, true);
                    }
                    if (sNode->name() == "Vector2") {
                        struct VEC2 {
                            float x;
                            float y;
                        };

                        VEC2* vec = new VEC2();

                        float xx = std::strtof(sNode->first_node("X")->value(), NULL);
                        float yx = std::strtof(sNode->first_node("Y")->value(), NULL);

                        vec->x = xx;
                        vec->y = yx;

                        defx.definition.push_back({ "vec2", sNode->first_attribute("name")->value() });

                        buffer.Write(vec, 8);
                    }
                    if (sNode->name() == "ImportProperty") {
                        //if (sNode->first_attribute("type")->value() == "float")
                    }
                }

                bfx.bufferImporter.source = AllocateUnmanaged(buffer.GetBytes(), buffer.size());
                bfx.bufferImporter.param1 = 0;
                defid = definitions.size();
                definitions.push_back(defx);
                buffer.Dispose();
            }
            else {
                bfx.bufferImporter.type = 1;
                bfx.bufferImporter.source = std::stoi(bufferNode->value());
            }
            x_buffer_t itm{};
            itm.bufferName = bufferNode->first_attribute("name")->value();
            itm.type = isStructBuffer ? 2 : 0;
            itm.structBufferId = defid;
            sbx.push_back(itm);
            buffers.push_back(bfx);
        }
        for (rapidxml::xml_node<>* bufferNode = root_node->first_node("Buffers")->first_node("TextureSampler"); bufferNode; bufferNode = bufferNode->next_sibling("TextureSampler")) {
            IMP_SHADER_BUFFER bfx{};
            bfx.bufferType = 1;
            bfx.bufferName = HashString(bufferNode->first_attribute("name")->value());
            if (bufferNode->first_node("ImportProperty") != 0) {
                bfx.bufferImporter.type = 0;
                bfx.bufferImporter.source = HashString(bufferNode->first_node("ImportProperty")->first_attribute("src")->value());
            }
            if (bufferNode->first_node("ImportTexture") != 0) {
                bfx.bufferImporter.type = 2;
                bfx.bufferImporter.source = AllocateFileSpace(std::filesystem::absolute(bufferNode->first_node("ImportTexture")->first_attribute("src")->value()).string());

            }
            if (bufferNode->first_node("BaseTexture") != 0) {
                bfx.bufferImporter.type = 3;
                bfx.bufferImporter.source = HashString(bufferNode->first_node("BaseTexture")->first_attribute("src")->value());

            }
            x_buffer_t itm{};
            itm.bufferName = bufferNode->first_attribute("name")->value();
            itm.type = 1;
            sbx.push_back(itm);
            buffers.push_back(bfx);
        }

        std::map<std::string, std::string> compileOptions;

        if (root_node->first_node("CompileOptions") != 0) {
            for (rapidxml::xml_node<>* bufferNode = root_node->first_node("CompileOptions")->first_node(); bufferNode; bufferNode = bufferNode->next_sibling()) {
                compileOptions.insert(std::pair(bufferNode->name(), bufferNode->value()));
            }
        }

        

        // Do pre processing for some shaders :)))

        auto fragSrc = root_node->first_node("FragmentShader")->first_attribute("src")->value();
        auto vertSrc = root_node->first_node("VertexShader")->first_attribute("src")->value();

        auto sx = READ_TEXT(fragSrc);
        auto vsx = READ_TEXT(vertSrc);

        auto frag = COMPILE_SHADER(root_node->first_attribute("name")->value(), sx->data, shaderc_shader_kind::shaderc_fragment_shader, this, shaderc::CompileOptions(), compileOptions, root_node->first_node("FragmentShader")->first_attribute("src")->value(), sbx, definitions);
        auto vert = COMPILE_SHADER(root_node->first_attribute("name")->value(), vsx->data, shaderc_shader_kind::shaderc_vertex_shader, this, shaderc::CompileOptions(), compileOptions, root_node->first_node("VertexShader")->first_attribute("src")->value(), sbx, definitions);

        delete sx->data;
        delete sx;
        delete vsx->data;
        delete vsx;

        shader.fragment_shader_index = AllocateUnmanaged(frag.data(), frag.size() * sizeof(uint32_t));
        shader.vertex_shader_index = AllocateUnmanaged(vert.data(), vert.size() * sizeof(uint32_t));

        shader.buffers = WriteVector(buffers);

        WriteEntry(shader, TYPE_SHADER);
    }
    void ImportMaterialXml(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);
        root_node = doc.first_node("Material");

        IMP_MATERIAL materialSet{};
        std::vector<IMP_MATERIAL_PROPERTY> props;

        auto shaderNode = root_node->first_node("Shader");
        auto shaderId = HashString(shaderNode->first_attribute("name")->value());
        auto materialId = HashString(root_node->first_attribute("name")->value());
        materialSet.hash = materialId;
        materialSet.shader_id = shaderId;

        for (rapidxml::xml_node<>* bufferNode = shaderNode->first_node(); bufferNode; bufferNode = bufferNode->next_sibling()) {
            IMP_MATERIAL_PROPERTY ppx{};
            if (bufferNode->first_node("ImportTexture") != 0) {
                ppx.type = 1;
                ppx.value = AllocateFileSpace(std::filesystem::absolute(bufferNode->first_node("ImportTexture")->first_attribute("src")->value()).string());
                ppx.name = HashString(bufferNode->name());
            }
            else {
                ppx.type = 0;
                ppx.value = std::stoi(bufferNode->value());
                ppx.name = HashString(bufferNode->name());
            }
            props.push_back(ppx);
        }

        materialSet.shader_prop_set = WriteVector(props);

        WriteEntry(materialSet, TYPE_MATERIAL);

    }
    void ImportEntityXml(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);
        root_node = doc.first_node("Entity");

        IMP_ENTITY entity{};

        entity.hash = HashString(root_node->first_attribute("name")->value());
        entity.material_id = HashString(root_node->first_node("Material")->first_attribute("name")->value());
        
        auto meshImportNode = root_node->first_node("Mesh")->first_node("ImportMesh");

        if (meshImportNode->first_attribute("type")->value() == "OBJ" || meshImportNode->first_attribute("type")->value() == "obj") {
            entity.mesh_type = 0;
        }
        if (meshImportNode->first_attribute("type")->value() == "FBX" || meshImportNode->first_attribute("type")->value() == "fbx") {
            entity.mesh_type = 1;
        }

        entity.mesh_index = AllocateFileSpace(std::filesystem::absolute(meshImportNode->first_attribute("src")->value()).string());

        auto childNode = root_node->first_node("Children");

        auto compNode = root_node->first_node("Components");

        std::vector<IMP_ENTITY_CHILD> chld;
        std::vector<size_t> comp;

        for (rapidxml::xml_node<>* bufferNode = childNode->first_node(0); bufferNode; bufferNode = bufferNode->next_sibling()) {
            IMP_ENTITY_CHILD ppx{};

            ppx.name = HashString(bufferNode->name());

            auto apx = VectorizeNode(bufferNode->first_node("Position"));
            auto asc = VectorizeNode(bufferNode->first_node("Scale"));
            auto arot = VectorizeNode(bufferNode->first_node("Rotation"));
            ppx.position = *apx;
            ppx.scale = *asc;
            ppx.rotation = *arot;

            delete apx;
            delete asc;
            delete arot;

            chld.push_back(ppx);
        }

        for (rapidxml::xml_node<>* bufferNode = compNode->first_node(0); bufferNode; bufferNode = bufferNode->next_sibling()) {
            
            comp.push_back(HashString(bufferNode->name()));

        }
        entity.children_set = WriteVector(chld);
        entity.component_list = WriteVector(comp);

        WriteEntry(entity, TYPE_ENTITY);
    }
    bool IsEmpty() {
        return entries_read.size() == 0;
    }
    bool IsWriteEmpty() {
        return entries.size() == 0;
    }
    void ImportSceneXml(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);
        root_node = doc.first_node("Scene");

        std::vector<IMP_ENTITY_CHILD> chld;
        IMP_SCENE scene{};
        scene.hash = HashString(root_node->first_attribute("name")->value());
        
        for (rapidxml::xml_node<>* bufferNode = root_node->first_node(0); bufferNode; bufferNode = bufferNode->next_sibling()) {
            IMP_ENTITY_CHILD ppx{};

            ppx.name = HashString(bufferNode->name());

            auto apx = VectorizeNode(bufferNode->first_node("Position"));
            auto asc = VectorizeNode(bufferNode->first_node("Scale"));
            auto arot = VectorizeNode(bufferNode->first_node("Rotation"));
            ppx.position = *apx;
            ppx.scale = *asc;
            ppx.rotation = *arot;

            delete apx;
            delete asc;
            delete arot;

            chld.push_back(ppx);
        }

        scene.children = WriteVector(chld);

        WriteEntry(scene, TYPE_SCENE);
    }
    void ImportComponent(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);
        root_node = doc.first_node("Component");

        IMP_COMPONENT_DEF comp{};
        comp.hash = HashString(root_node->first_attribute("name")->value());
        
        std::vector<IMP_SCRIPT> scripts;
        for (rapidxml::xml_node<>* bufferNode = root_node->first_node("Script"); bufferNode; bufferNode = bufferNode->next_sibling("Script")) {
            IMP_SCRIPT scr{};
            scr.location = AllocateFileSpace(std::filesystem::absolute(bufferNode->first_attribute("src")->value()).string());
            scripts.push_back(scr);
        }

        comp.script_vec_location = WriteVector(scripts);

        WriteEntry(comp, TYPE_COMPONENT);

    }
    void ImportBootstrappable(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);
        root_node = doc.first_node("Bootstrap");

        IMP_BOOTSTRAPPABLE comp{};
        comp.hash = HashString(root_node->first_attribute("name")->value());

        std::vector<IMP_SCRIPT> scripts;
        for (rapidxml::xml_node<>* bufferNode = root_node->first_node("Script"); bufferNode; bufferNode = bufferNode->next_sibling("Script")) {
            IMP_SCRIPT scr{};
            scr.location = AllocateFileSpace(std::filesystem::absolute(bufferNode->first_attribute("src")->value()).string());
            scripts.push_back(scr);
        }

        comp.script_vec_location = WriteVector(scripts);

        bootstraps.push_back(comp);
    }
    void ImportShaderLib(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);
        root_node = doc.first_node("ShaderLib");

        std::vector<IMP_DEV_BINARY> devbin;
        IMP_SHADER_LIB slib{};

        for (rapidxml::xml_node<>* bufferNode = root_node->first_node("Include"); bufferNode; bufferNode = bufferNode->next_sibling("Include")) {
            IMP_DEV_BINARY bin{};
            bin.hash = HashString(bufferNode->first_attribute("name")->value());
            bin.fileId = AllocateFileSpace(std::filesystem::absolute(bufferNode->first_attribute("src")->value()).string());
            
            devbin.push_back(bin);
        }
        slib.includes = WriteVector(devbin);
        slib.hash = HashString(root_node->first_attribute("name")->value());
        slib.type = 0;

        WriteEntry(slib, 0x10);

    }
    void ImportXml(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);

        if (doc.first_node("Scene") != 0) {
            theFile.close();
            ImportSceneXml(file);
        }
        if (doc.first_node("Material") != 0) {
            theFile.close();
            ImportMaterialXml(file);
        }
        if (doc.first_node("Entity") != 0) {
            theFile.close();
            ImportEntityXml(file);
        }
        if (doc.first_node("Shader") != 0) {
            theFile.close();
            ImportShaderXml(file);
        }
        if (doc.first_node("Component") != 0) {
            theFile.close();
            ImportComponent(file);
        }
        if (doc.first_node("Bootstrap") != 0) {
            theFile.close();
            ImportBootstrappable(file);
        }
    }
    void ImportXmlOnlyIfShaderLib(std::string file) {
        std::ifstream theFile(file);
        std::vector<char> buffer((std::istreambuf_iterator<char>(theFile)), std::istreambuf_iterator<char>());
        buffer.push_back('\0');
        rapidxml::xml_document<> doc;
        rapidxml::xml_node<>* root_node;
        doc.parse<0>(&buffer[0]);

        if (doc.first_node("ShaderLib") != 0) {
            theFile.close();
            ImportShaderLib(file);
        }
    }
};


void ImportFromDirectory(DYSTRO_ARCHIVE* archive, std::string directory) {
    std::filesystem::current_path(directory);
    for (const auto& entry : std::filesystem::directory_iterator(std::filesystem::current_path())) {
        if (entry.is_directory()) {
            auto x = entry.path().string();
            ImportFromDirectory(archive, x);
            std::filesystem::current_path("../");
        }
        if (entry.is_regular_file()) {
            if (entry.path().extension().string() == ".xml") {

                archive->ImportXml(entry.path().string());
            }
        }
    }
}

void ImportLibFromDirectory(DYSTRO_ARCHIVE* archive, std::string directory) {
    std::filesystem::current_path(directory);
    for (const auto& entry : std::filesystem::directory_iterator(std::filesystem::current_path())) {
        if (entry.is_directory()) {
            auto x = entry.path().string();
            ImportLibFromDirectory(archive, x);
            std::filesystem::current_path("../");
        }
        if (entry.is_regular_file()) {
            if (entry.path().extension().string() == ".xml") {
 
                archive->ImportXmlOnlyIfShaderLib(entry.path().string());
            }
        }
    }
}

shaderc_include_result* DYSTRO_INCLUDER::GetInclude(const char* requested_source,
    shaderc_include_type type,
    const char* requesting_source,
    size_t include_depth) {
    auto x = new shaderc_include_result{};
    x->content = "Could not include source!";
    x->content_length = sizeof("Could not include source!");
    x->source_name = "";
    x->source_name_length = 0;
    x->user_data = NULL;

    if (type == shaderc_include_type::shaderc_include_type_relative) {
        // #include "item" 
        // This is used for importing of external librarys from development packages.
        if (exists(requested_source)) {
            auto data = READ_FILE(requested_source);
            x->content = data->data;
            x->content_length = data->size;
            x->source_name = requested_source;
            x->source_name_length = std::strlen(requested_source);

            delete data;
        }
        else {
            x->content = "Could not find file.";
            x->content_length = sizeof("Could not find file.");
        }
    }
    else if (type == shaderc_include_type::shaderc_include_type_standard) {
        // #include <item>
        // This is used for importing of external librarys from development packages.

        std::string caller(requested_source);
        if (caller.find("/") == -1) {
            x->content = "Include is not formatted correctly.";
            x->content_length = sizeof("Include is not formatted correctly.");
            return x;
        }

        std::string shader_lib = caller.substr(0, caller.find("/"));

        if (TOOLS_ARCHIVE->DoesEntryExist(HashString(shader_lib))) {
            IMP_SHADER_LIB* shaderItem = TOOLS_ARCHIVE->GetEntry<IMP_SHADER_LIB>(shader_lib);
            auto sidx = shaderItem->includes;
            std::vector<IMP_DEV_BINARY> binaryList = TOOLS_ARCHIVE->GetVector<IMP_DEV_BINARY>(sidx);
            auto xz = caller.substr(caller.find("/") + 1, caller.length() - shader_lib.length());
            size_t nameHash = HashString(xz);
            for (auto bin : binaryList) {
                if (bin.hash == nameHash) {
                    x->source_name = requested_source;
                    x->source_name_length = std::strlen(requested_source);
                    x->content = (char*)TOOLS_ARCHIVE->GetFile(bin.fileId);
                    x->content_length = TOOLS_ARCHIVE->GetFileSize(bin.fileId);
                    return x;
                }
            }
        }
        else {
            x->content = "There is no definition for this shading library.";
            x->content_length = sizeof("There is no definition for this shading library.");
        }
    }

    return x;
}


int main(int argc, char* argv[])
{

    auto major = XE_F_VERSION_MAJOR;
    auto minor = XE_F_VERSION_MINOR

    auto curFulPath = std::filesystem::current_path();
    std::cout << "XeCLI v" << std::to_string(major) <<"."<< std::to_string(minor) <<"\n";

    DYSTRO_ARCHIVE archive;

    std::string output = "game.dystro";
    std::string inputDir = "Game/";
    std::vector<std::string> libs;

    for (int i = 0; i < argc; i++) {
        
        if (std::string(argv[i]) == "-d") {
            inputDir = argv[i + 1];
            i++;
        }
        if (std::string(argv[i]) == "-l") {
            libs.push_back(argv[i + 1]);
            i++;
        }
        if (std::string(argv[i])== "-o") {
            output = argv[i + 1];
            i++;
        }
    }


    for (auto lib : libs) {
        archive.ReadFromFile(lib);
    }

    DYSTRO_ARCHIVE tmpArchive;

    std::cout << "Building libraries: '" << inputDir << "'\n";

    ImportLibFromDirectory(&tmpArchive, inputDir);

    std::filesystem::current_path(curFulPath);

    

    if (!tmpArchive.IsWriteEmpty()) {
        std::cout << "Built libraries\n";
        tmpArchive.WriteToFile(output + ".dxlib");
        archive.ReadFromFile(output + ".dxlib");
    }

    tmpArchive.Dispose();

    std::cout << "Importing from: " << inputDir << "\n";

    ImportFromDirectory(&archive, inputDir);

    std::filesystem::current_path(curFulPath);

    archive.WriteToFile(output);

    std::cout << "Built " << inputDir;

    archive.Dispose();

    return 0;

}
