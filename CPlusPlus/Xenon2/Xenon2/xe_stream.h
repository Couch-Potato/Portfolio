#pragma once
#include "xenon.h"
/*
Class for easily making and writing streams
*/

typedef uint64_t XeStreamSize;

class XeStream:public XeDisposable
{
public:
	XeStream(XeStreamSize size);
	XeStreamSize Size;
	void Cleanup();
	void Write(void* data, XeStreamSize length,  XeStreamSize position = -1);
	void* Read();
private:
	void* stream_data;
	XeStreamSize i_position;
};

