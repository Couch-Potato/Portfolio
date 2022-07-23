# CMakeList.txt : CMake project for NeuralVisualizer, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)


set(CMAKE_CXX_STANDARD 11)
add_subdirectory(rpi_ws281x)
add_subdirectory(libblepp)
# Add source to this project's executable.
add_executable (NeuralVisualizer "NeuralVisualizer.cpp" "NeuralVisualizer.h" "StripVisualizer.h" "StripVisualizer.cpp")
find_library(WIRINGPI_LIBRARIES NAMES wiringPi)
target_link_libraries(NeuralVisualizer ${WIRINGPI_LIBRARIES} ws2811 ble++)
target_include_directories(NeuralVisualizer PUBLIC "${PROJECT_BINARY_DIR}" "${PROJECT_SOURCE_DIR}/rpi_ws281x" "${PROJECT_SOURCE_DIR}/libblepp")


# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/gcc/libsfml-audio.so)

# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/gcc/libsfml-system.dylib)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/gcc/libsfml-graphics.dylib)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/gcc/libsfml-window.dylib)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/clang/libogg)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/clang/libopenal)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/clang/libflac)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/clang/libvorbisenc)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/clang/libvorbisfile)
# target_link_libraries(NeuralVisualizer ${CMAKE_SOURCE_DIR}/sfml/clang/libvorbis)
# TODO: Add tests and install targets if needed.
