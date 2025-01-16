#pragma once

#include <stdint.h>
#include <deque>
#include "HuffmanTreeNode.h"
#include <algorithm>
#include <vector>
#include <list>

#ifdef HUFFMAN_COMPRESION
#define HUFFMAN_CPP __declspec(dllexport)
#else
#define HUFFMAN_CPP __declspec(dllimport)
#endif


struct ByteCounter {
    int byte;
    long counter;

    ByteCounter() : byte(0), counter(0) {}
    ByteCounter(int _byte, long _counter) : byte(_byte), counter(_counter) {}
};

bool operator<(ByteCounter b1, ByteCounter b2) {
    return b1.counter < b2.counter;
}

//countBytes
//Function witch count the occurrence of all bytes in the 1B integers table
//inputs:
//  byteTable[] - table of tytes to count
//  arraySize - long signed int of the length of the byteTable array
//outputs:
//  long signed int[] - table of counters for the bytes, where index i is counter for byte of value i           0 <= i < 256
//
extern "C" __declspec(dllexport) long* countBytes(uint8_t* byteTable, int arraySize) {
    long counterTable[256];                     //table of counters of bytes

    //fill the table with 0
    for (int i = 0; i < 256; ++i) {
        counterTable[i] = 0;
    }

    //count bytes in the file
    for (long i = 0; i < arraySize; ++i) {
        ++counterTable[byteTable[i]];                //increment a value of occurrence of the byte
    }

    return counterTable;
}

//makeSortedArray
//input:
//  array - array which should be sorted
//  sortedArrayLength - length of the array to sort
//output:
//  void
//
extern "C" __declspec(dllexport) void* makeSortedArray(long* array, int sortedArraylength) {
    std::deque<ByteCounter> byteTab = std::deque<ByteCounter>(256);

    //fill the table with bytes and counters
    for (int i = 0; i < 256; ++i) {
        byteTab[i] = ByteCounter(i, array[i]);
    }

    std::sort(byteTab.begin(), byteTab.end());

    while (byteTab[0].counter == 0)
        byteTab.pop_front();

    return (void*) &byteTab;
}

extern "C" __declspec(dllexport) void* makeHuffmanCodeTree(ByteCounter* sortedInputArray, int sizeOfInputArray) {
    std::list<HuffmanTreeNode> orphanNodes;
    std::vector<HuffmanTreeNode> codeArray;
    int usedNodes = 0;
    ByteCounter choosedBytes[2];
    std::vector<HuffmanTreeNode> choosedNodes;

    while (usedNodes < sizeOfInputArray) {
        choosedBytes[0] = sortedInputArray[usedNodes];
        choosedBytes[1] = sortedInputArray[usedNodes + 1];

        if (!orphanNodes.empty()) {
            for (HuffmanTreeNode item : orphanNodes) {
                if (item.getValue() < choosedBytes[0].counter and choosedNodes.size() < 2) {
                    choosedNodes.push_back(item);
                }
                else if (item.getValue() < choosedBytes[1].counter and choosedNodes.size() < 1) {
                    choosedNodes.push_back(item);
                }
                else {
                    if (choosedNodes[0].getValue() > item.getValue()) {
                        choosedNodes[0] = item;
                    }
                    else if (choosedNodes[1].getValue() > item.getValue()) {
                        choosedNodes[1] = item;
                    }
                }
            }
        }
        else {
            codeArray.push_back(HuffmanTreeNode(choosedBytes[0].byte, choosedBytes[0].counter, nullptr, nullptr));

            codeArray.push_back(HuffmanTreeNode(choosedBytes[1].byte, choosedBytes[1].counter, nullptr, nullptr));
        }

        //tworzenie drzewa
    }

    return (void*) & codeArray[0];
}

extern "C" __declspec(dllexport) void codeFile(char** huffmanCode, int fileArray, int codedArray, long fileLength){}