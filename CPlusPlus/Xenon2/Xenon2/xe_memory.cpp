#include "xenon.h"

void XeMemoryAllocator::LinkRegion(region_t parent, region_t child) {
	allocatedRegions[parent].linked_regions.push_back(child);
}

void XeMemoryAllocator::DeAllocate(region_t region) {
	auto rgx = allocatedRegions[region];

	PLOGV << "Deallocating memory region: " << std::hex << region << " of size: " << rgx.region_size << " @ 0x" << std::hex << rgx.ptr;

	for (auto const rg2 : rgx.linked_regions) {
		DeAllocate(rg2);
	}

	/*
	Handle any fancy destructors
	*/
	if (rgx.allocation_type == allocated_t::ALLOCATED_ENTITY) {
		GetInstanceFromHandle<XeMeshedEntity>(region)->Cleanup();
	}
	if (rgx.allocation_type == allocated_t::ALLOCATED_TEXTURE) {
		GetInstanceFromHandle<XeTexture>(region)->Cleanup();
	}
	if (rgx.allocation_type == allocated_t::ALLOCATED_MATERIAL) {
		GetInstanceFromHandle<XeMaterial>(region)->Cleanup();
	}

	free(allocatedRegions[region].ptr);
	allocatedRegions[region].isFreed = true;
}

void XeFree(XenonEngine* engine, region_t val) {
	return engine->memoryManager.DeAllocate(val);
}


bool XeDoesOwnInstance(XenonEngine* engine, region_t handle, owner_t owner) {
	return engine->memoryManager.allocatedRegions[handle].owner == owner;
}

owner_t GetMemoryOwner(XenonEngine* engine, region_t handle) {
	return engine->memoryManager.allocatedRegions[handle].owner;
}

void XeLinkMemory(XenonEngine* engine, region_t parent, region_t child) {
	engine->memoryManager.LinkRegion(parent, child);
}

