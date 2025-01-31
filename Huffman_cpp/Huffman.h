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
    long byte;
    long long counter;

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
extern "C" __declspec(dllexport) HUFFMAN_CPP void countBytes(uint8_t* byteTable, int arraySize, long long* ptr) {
    
    //count bytes in the file
    for (long long i = 0; i < 12; ++i) {
        ++ptr[byteTable[i]];                //increment a value of occurrence of the byte
    }

}


//makeSortedArray
//input:
//  array - array which should be sorted
//  sortedArrayLength - length of the array to sort
//output:
//  void
//
extern "C" __declspec(dllexport) void makeSortedArray(long long* array, long sortedArraylength, ByteCounter* countersArray) {
    std::deque<ByteCounter> byteTab = std::deque<ByteCounter>(256);

    //fill the table with bytes and counters
    for (int i = 0; i < 256; ++i) {
        byteTab[i] = ByteCounter(i, array[i]);
    }

    std::sort(byteTab.begin(), byteTab.end());

    while (byteTab[0].counter == 0)
        byteTab.pop_front();

    for (long i = 0; i < sortedArraylength; ++i) {
        countersArray[i] = byteTab[i];
    }
}

extern "C" __declspec(dllexport) void makeHuffmanCodeTree(ByteCounter* sortedInputArray, long sizeOfInputArray, HuffmanTreeNode* treeArray) {
    std::list<HuffmanTreeNode> orphanNodes;
    int usedNodes = 0;
    int arrayIterator = 0;
    ByteCounter choosedBytes[2];
    std::vector<std::list<HuffmanTreeNode>::iterator> choosedNodes;

    while (usedNodes + 1 < sizeOfInputArray) {
        choosedBytes[0] = sortedInputArray[usedNodes];
        choosedBytes[1] = sortedInputArray[usedNodes + 1];

        if (!orphanNodes.empty()) {
            for (std::list<HuffmanTreeNode>::iterator i = orphanNodes.begin(); i != orphanNodes.end(); ++i) {
                if ((*i).getValue() <= choosedBytes[0].counter and choosedNodes.size() < 2) {
                    choosedNodes.push_back(i);
                }
                else if ((*i).getValue() <= choosedBytes[1].counter and choosedNodes.size() < 1) {
                    choosedNodes.push_back(i);
                }
                else if (choosedNodes.size() == 2) {
                    if ((*choosedNodes[0]).getValue() >= (*i).getValue()) {
                        choosedNodes[0] = i;
                    }
                    else if ((*choosedNodes[1]).getValue() >= (*i).getValue()) {
                        choosedNodes[1] = i;
                    }
                }
            }
        }
        orphanNodes.push_back(HuffmanTreeNode());

        if (choosedNodes.size() <= 1) {
            treeArray[arrayIterator] = HuffmanTreeNode(choosedBytes[0].byte, choosedBytes[0].counter, nullptr, nullptr);
            ++usedNodes;
        }
        else if (choosedNodes.size() == 2) {
            treeArray[arrayIterator] = *choosedNodes.back();
            orphanNodes.erase(choosedNodes.back());
            choosedNodes.pop_back();
        }

        orphanNodes.back().setLeft(&treeArray[arrayIterator]);
        orphanNodes.back().addValue(treeArray[arrayIterator].getValue());

        ++arrayIterator;

        if (choosedNodes.size() == 0) {
            treeArray[arrayIterator] = HuffmanTreeNode(choosedBytes[1].byte, choosedBytes[1].counter, nullptr, nullptr);
            ++usedNodes;
        }
        else if (choosedNodes.size() >= 1) {
            treeArray[arrayIterator] = *choosedNodes.back();
            orphanNodes.erase(choosedNodes.back());
            choosedNodes.pop_back();
        }

        orphanNodes.back().setRight(&treeArray[arrayIterator]);
        orphanNodes.back().addValue(treeArray[arrayIterator].getValue());

        ++arrayIterator;
    }

    while (orphanNodes.size() != 1) {

        if (choosedNodes.size() == 0) {
            choosedNodes.push_back(orphanNodes.begin());

            orphanNodes.push_back(HuffmanTreeNode(0, 0, nullptr, nullptr));

            for (std::list<HuffmanTreeNode>::iterator i = ++orphanNodes.begin(); i != --orphanNodes.end(); ++i) {
                if ((*i).getValue() < (*choosedNodes.back()).getValue())
                    choosedNodes.back() = i;
            }
        }
        else
            orphanNodes.push_back(HuffmanTreeNode(0, 0, nullptr, nullptr));

        orphanNodes.back().addValue((*choosedNodes.back()).getValue());
        treeArray[arrayIterator] = *choosedNodes.back();
        orphanNodes.back().setRight(&treeArray[arrayIterator]);
        orphanNodes.erase(choosedNodes.back());
        choosedNodes.pop_back();

        choosedNodes.push_back(orphanNodes.begin());

        for (std::list<HuffmanTreeNode>::iterator i = ++orphanNodes.begin(); i != --orphanNodes.end(); ++i) {
            if ((*i).getValue() < (*choosedNodes.back()).getValue())
                choosedNodes.back() = i;
        }

        if (usedNodes == sizeOfInputArray - 1 and (*choosedNodes.back()).getValue() >= sortedInputArray[usedNodes].counter) {

            orphanNodes.back().addValue(sortedInputArray[usedNodes].counter);

            treeArray[++arrayIterator] = HuffmanTreeNode(sortedInputArray[usedNodes].byte, sortedInputArray[usedNodes].counter, nullptr, nullptr);
            orphanNodes.back().setLeft(&treeArray[arrayIterator]);
            ++arrayIterator;
            ++usedNodes;
        }
        else {

            orphanNodes.back().addValue((*choosedNodes.back()).getValue());
            treeArray[++arrayIterator] = *choosedNodes.back();
            orphanNodes.back().setLeft(&treeArray[arrayIterator]);
            orphanNodes.erase(choosedNodes.back());
            choosedNodes.pop_back();
        }


    }

    treeArray[++arrayIterator] = orphanNodes.back();
}

extern "C" __declspec(dllexport) void codeFile(short** huffmanCode, uint8_t* fileArray, char* codedArray, long long fileLength) {
    int codeChar = -1;
    int codeArrayIterator = 0;
    int codedFileIterator = 0;
    int byteIterator = 0;
    uint8_t byteHolder = 0;

    for (long long i = 0; i < fileLength; ++i) {
        while (codeChar != 0) {
            codeChar = huffmanCode[fileArray[i]][codeArrayIterator];

            if (codeChar == 48) {
                byteHolder = byteHolder << 1;
                ++byteIterator;
            }
            else if (codeChar == 49) {
                byteHolder = byteHolder << 1;
                ++byteHolder;
                ++byteIterator;
            }
            ++codeArrayIterator;

            if (byteIterator == 8) {
                codedArray[codedFileIterator++] = byteHolder;
                byteIterator = 0;
                byteHolder = 0;
            }
        }

        codeChar = -1;
        codeArrayIterator = 0;
    }
    byteHolder = byteHolder << (8 - byteIterator);
    if (byteIterator != 0)
        codedArray[codedFileIterator] = byteHolder;
}

extern "C" __declspec(dllexport) void decodeFile(HuffmanTreeNode* treeRoot, uint8_t* fileArray, uint8_t* codedArray, unsigned long long fileLength) {
    HuffmanTreeNode* currentNode = treeRoot;
    uint8_t bitMask = 128;
    uint8_t bit = 0;
    long long codedArrayIterator = 0;
    long long fileArrayIterator = 0;

    while (fileArrayIterator < fileLength) {
        while (bitMask != 0) {
            bit = codedArray[codedArrayIterator] & bitMask;

            if (bit == 0)
                currentNode = currentNode->getLeft();
            else
                currentNode = currentNode->getRigth();

            if (currentNode->getLeft() == nullptr and currentNode->getRigth() == nullptr) {
                fileArray[fileArrayIterator++] = currentNode->getByte();
                currentNode = treeRoot;
            }

            bitMask = bitMask >> 1;
        }
        bitMask = 128;
        ++codedArrayIterator;
    }
}