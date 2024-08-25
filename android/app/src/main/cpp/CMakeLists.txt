project(Object_Detect)

cmake_minimum_required(VERSION 3.10)

set(OpenCV_DIR ${CMAKE_SOURCE_DIR}/opencv/sdk/native/jni)
find_package(OpenCV REQUIRED core imgproc highgui)

set(ncnn_DIR ${CMAKE_SOURCE_DIR}/ncnn/${ANDROID_ABI}/lib/cmake/ncnn)
find_package(ncnn REQUIRED)

add_library(Object_Detect SHARED yolov8ncnn.cpp yolo.cpp ndkcamera.cpp)

target_link_libraries(Object_Detect ncnn ${OpenCV_LIBS} camera2ndk mediandk)