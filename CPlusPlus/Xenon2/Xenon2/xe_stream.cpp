#include "xe_stream.h"

XeStream::XeStream(XeStreamSize size) {
	Size = size;
	stream_data = malloc(size);
}
void XeStream::Cleanup() {
	free(stream_data);
}
void XeStream::Write(void* data, XeStreamSize length, XeStreamSize position) {
	bool MOVE_POSITION = position > -1;
	XeStreamSize POSITION_OFFSET = 0;
	if (MOVE_POSITION) POSITION_OFFSET = position; else MOVE_POSITION = i_position;
	if (MOVE_POSITION + length > Size) throw new std::runtime_error("Write failed... trying to write to a place in memory that is beyond the allocated region.");
	memmove((char*)stream_data + MOVE_POSITION, data, length);

}
void* XeStream::Read() {
	return stream_data;
}