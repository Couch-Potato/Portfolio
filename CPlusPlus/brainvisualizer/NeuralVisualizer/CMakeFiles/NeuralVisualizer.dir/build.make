# CMAKE generated file: DO NOT EDIT!
# Generated by "Unix Makefiles" Generator, CMake Version 3.16

# Delete rule output on recipe failure.
.DELETE_ON_ERROR:


#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:


# Remove some rules from gmake that .SUFFIXES does not remove.
SUFFIXES =

.SUFFIXES: .hpux_make_needs_suffix_list


# Suppress display of executed commands.
$(VERBOSE).SILENT:


# A target that is always out of date.
cmake_force:

.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

# The shell in which to execute make rules.
SHELL = /bin/sh

# The CMake executable.
CMAKE_COMMAND = /usr/bin/cmake

# The command to remove a file.
RM = /usr/bin/cmake -E remove -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = /home/pi/Documents/brainvisualizer

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = /home/pi/Documents/brainvisualizer

# Include any dependencies generated for this target.
include NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/depend.make

# Include the progress variables for this target.
include NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/progress.make

# Include the compile flags for this target's objects.
include NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/flags.make

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.o: NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/flags.make
NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.o: NeuralVisualizer/NeuralVisualizer.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/home/pi/Documents/brainvisualizer/CMakeFiles --progress-num=$(CMAKE_PROGRESS_1) "Building CXX object NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.o"
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && /usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.o -c /home/pi/Documents/brainvisualizer/NeuralVisualizer/NeuralVisualizer.cpp

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.i"
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && /usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /home/pi/Documents/brainvisualizer/NeuralVisualizer/NeuralVisualizer.cpp > CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.i

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.s"
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && /usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /home/pi/Documents/brainvisualizer/NeuralVisualizer/NeuralVisualizer.cpp -o CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.s

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.o: NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/flags.make
NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.o: NeuralVisualizer/StripVisualizer.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/home/pi/Documents/brainvisualizer/CMakeFiles --progress-num=$(CMAKE_PROGRESS_2) "Building CXX object NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.o"
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && /usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.o -c /home/pi/Documents/brainvisualizer/NeuralVisualizer/StripVisualizer.cpp

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.i"
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && /usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /home/pi/Documents/brainvisualizer/NeuralVisualizer/StripVisualizer.cpp > CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.i

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.s"
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && /usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /home/pi/Documents/brainvisualizer/NeuralVisualizer/StripVisualizer.cpp -o CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.s

# Object files for target NeuralVisualizer
NeuralVisualizer_OBJECTS = \
"CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.o" \
"CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.o"

# External object files for target NeuralVisualizer
NeuralVisualizer_EXTERNAL_OBJECTS =

NeuralVisualizer/NeuralVisualizer: NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/NeuralVisualizer.cpp.o
NeuralVisualizer/NeuralVisualizer: NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/StripVisualizer.cpp.o
NeuralVisualizer/NeuralVisualizer: NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/build.make
NeuralVisualizer/NeuralVisualizer: sfml/clang/libsfml-audio.dylib
NeuralVisualizer/NeuralVisualizer: sfml/clang/libsfml-system.dylib
NeuralVisualizer/NeuralVisualizer: sfml/clang/libsfml-graphics.dylib
NeuralVisualizer/NeuralVisualizer: sfml/clang/libsfml-window.dylib
NeuralVisualizer/NeuralVisualizer: NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/link.txt
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --bold --progress-dir=/home/pi/Documents/brainvisualizer/CMakeFiles --progress-num=$(CMAKE_PROGRESS_3) "Linking CXX executable NeuralVisualizer"
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && $(CMAKE_COMMAND) -E cmake_link_script CMakeFiles/NeuralVisualizer.dir/link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/build: NeuralVisualizer/NeuralVisualizer

.PHONY : NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/build

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/clean:
	cd /home/pi/Documents/brainvisualizer/NeuralVisualizer && $(CMAKE_COMMAND) -P CMakeFiles/NeuralVisualizer.dir/cmake_clean.cmake
.PHONY : NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/clean

NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/depend:
	cd /home/pi/Documents/brainvisualizer && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" /home/pi/Documents/brainvisualizer /home/pi/Documents/brainvisualizer/NeuralVisualizer /home/pi/Documents/brainvisualizer /home/pi/Documents/brainvisualizer/NeuralVisualizer /home/pi/Documents/brainvisualizer/NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/DependInfo.cmake --color=$(COLOR)
.PHONY : NeuralVisualizer/CMakeFiles/NeuralVisualizer.dir/depend

