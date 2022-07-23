#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <vector>
#include <fstream>
#include <algorithm>
#include <string>
#include <vector>
#include <map>
#include <string> 

const size_t TYPE_ENTITY = 0x0;
const size_t TYPE_MATERIAL = 0x1;
const size_t TYPE_SHADER = 0x2;
const size_t TYPE_SCENE = 0x3;


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
};

struct IMP_ENTRY_PRECEEDER {
    size_t entry_length;
    size_t entry_type;
};

struct IMP_IMPORTER {
    size_t type;
    size_t source;
};

struct IMP_SHADER_BUFFER {
    size_t bufferType;
    size_t bufferName;
    IMP_IMPORTER bufferImporter;
};

struct IMP_CHILDREN {
    size_t name;
};

size_t HashString(std::string str) {
    return std::hash<std::string>{}(str);
}

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


class DYSTRO_ARCHIVE {
private:
    std::map<std::string, size_t> fileSpace;
    std::vector<ENTRY> entries;
    std::map<size_t, ENTRY_INFO> entries_read;
    std::map<size_t, FILE_ENTRY_INFO> file_entries;
    std::vector<UNMAMAGED_ENTRY> unmanaged_entries;
    std::vector<std::string> write_order;
    size_t currentSize = 0;
    size_t currentFile_Size = 0;
public:
    VERSION_HEADER VERSION;
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

        std::ifstream infile(location, std::ofstream::binary);
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
        std::fstream outfile(output, std::ios::binary | std::ios::out);

        // Create our version header;

        VERSION_HEADER header{};
        header.MAJOR = 1;
        header.MINOR = 0;
        header.TABLE_SIZE = GetIndexTableSize();

        // Write the version header

        auto headerBuffer = writeStructToBuffer<VERSION_HEADER>(header);
        outfile.write((char*)headerBuffer, sizeof(header));
        delete headerBuffer;

        // Write the entries

        for (auto entry : entries) {
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
            infile.open(key, std::ios::in | std::ios::binary);

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

        VERSION = *header;

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

            char* hashNameBuffer = (char*)malloc(sizeof(size_t));
            inStream.read(hashNameBuffer, sizeof(size_t));
            size_t* hx = reinterpret_cast<size_t*>(hashNameBuffer);
            auto actualSize = *hx;

            size_t stream_Start = inStream.tellg();

            inStream.seekg(stream_Start + actualSize, std::ios::beg);

            FILE_ENTRY_INFO ifx{};
            ifx.fileLen = actualSize;
            ifx.fileStart = stream_Start;
            ifx.archive = fileName;

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
    size_t GetFileEntrySize(size_t indexId) {
        return file_entries[indexId].fileLen;
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

        void** xar = new void* [indvSize];

        /*       std::vector<void*> sliceNDice;
               sliceNDice.resize(indvSize);*/
        for (int i = 0; i < fileEntry.fileLen; i += sizeof(T))
        {
            auto idx = i == 0 ? 0 : i / sizeof(T);
            auto nDice = malloc(sizeof(T));
            memcpy(nDice, ((char*)indexedData) + i, sizeof(T));
            xar[idx] = nDice;
        }

        std::vector<T> structured;
        for (int i = 0; i < indvSize; i++) {
            auto entry = xar[i];
            structured.push_back(*(reinterpret_cast<T*>(entry)));
            free(entry);
        }
        delete xar;
        return structured;
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
};

