#pragma once

#ifdef HUFFMAN_COMPRESION
#define HUFFMAN_CPP __declspec(dllexport)
#else
#define HUFFMAN_CPP __declspec(dllimport)
#endif

extern "C" HUFFMAN_CPP void doSth();