#pragma once
#include <plog/Log.h>
#include <plog/Appenders/ColorConsoleAppender.h>
namespace vkt_log {
	void init(int logLevel) {
		static plog::ColorConsoleAppender<plog::TxtFormatter> consoleAdapter;
		plog::init((plog::Severity)logLevel, "log.txt").addAppender(&consoleAdapter); // Initialize the logger with the both appenders.....
	}
}