#pragma once
#include <vector>
#include <map>

enum XFS_FILE_TYPE {
	IMAGE = 0x001,
	MODEL_MESH_OBJ = 0x002,
	MODEL_MESH_FBX = 0x003,
	SHADER = 0x004,
	SOURCE_CODE_FILE = 0x005,
	ENTITY_DEF = 0x006,
	SCENE_DEF = 0x007,
	MACRO_DEF = 0x008,
	MATERIAL = 0x009,
	END
};


template <class T>
class xfs_item_t {
public:
	T item;
	XFS_FILE_TYPE type;
	uint32_t hash;
	xfs_item_t(T instance) {
		item = instance;
	}
};

struct xfs_archive_idx_item_t {
	uint32_t hash;
	XFS_FILE_TYPE type;
	size_t size;
	size_t location;
};

struct xfs_archive_idx
{
	std::map<XFS_FILE_TYPE, std::vector<xfs_archive_idx_item_t>> items;
};

void XfInitArchiveIndex(xfs_archive_idx* index) {
	for (int fooInt = XFS_FILE_TYPE::IMAGE; fooInt != XFS_FILE_TYPE::END; fooInt++)
	{
		XFS_FILE_TYPE foo = static_cast<XFS_FILE_TYPE>(fooInt);
		index->items.insert(std::pair<XFS_FILE_TYPE, std::vector<xfs_archive_idx_item_t>>(foo, std::vector<xfs_archive_idx_item_t>()));
	}
	
}

void XfIndexFile(uint32_t hash, XFS_FILE_TYPE type, size_t size, size_t location, xfs_archive_idx *index) {
	index->items[type].push_back(xfs_archive_idx_item_t{
		hash,
		type,
		size,
		location
		});
}
