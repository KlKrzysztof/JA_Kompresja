// Asembly Language - Project
// Krzyszkof Klecha
// section 11
// semester 5
// year 2024 / 25
// Huffman coding compresion
// version 1.1
//
// Dll library : c++ language
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
    long long byte;
    long long counter;

    ByteCounter() : byte(0), counter(0) {}
    ByteCounter(int _byte, long _counter) : byte(_byte), counter(_counter) {}
};

bool operator<(ByteCounter b1, ByteCounter b2) {
    return b1.counter < b2.counter;
}

//countBytes - gets byte array and count occurance of all bytes in the array.
//             Saves counters in destination array where index == byte 
//
//inputs:
//  byteTable[] - table of tytes to count
//  arraySize - long signed int of the length of the byteTable array
//  long signed int[] - table of counters for the bytes, where index i is counter for byte of value i           0 <= i < 256
//outputs:
//  Destinated array  filled with counters of each byte as a long long int
// 
// Algotithm increments counter at index readed as byte
//
extern "C" __declspec(dllexport) HUFFMAN_CPP void countBytes(uint8_t* byteTable, long arraySize, long long* ptr) {
    
    //count bytes in the file
    for (long long i = 0; i < arraySize; ++i) {
        ++ptr[byteTable[i]];                //increment a value of occurrence of the byte
    }

}


//makeSortedArray - creates a sorted array (ascending order) from an unordered byte counter array
//                  (where sourceArray[byte] = counter) into a new array of items(each item is a pair <byte, counter>)
//                  and excludes bytes with counter = 0.
//input:
//  long long *array            - pointer to source counting array (each long long represents counter for byte index)
//  long long sortedArrayLength - length of the source array
//  ByteCounter* countersArray  - pointer to destination (sorted) array
//output:
//  the destination array is filled with items (ByteCounter)
//   For equal counters, the original order is preserved(stable sort).
// 
//  struct ByteCounter {
//      long long byte;
//      long long counter;
//  };
//
extern "C" __declspec(dllexport) void makeSortedArray(long long* array, long long sortedArraylength, ByteCounter* countersArray) {
    std::deque<ByteCounter> byteTab = std::deque<ByteCounter>(256); //queue for easy deleting null values 

    //fill the table with bytes and counters
    for (int i = 0; i < 256; ++i) {
        byteTab[i] = ByteCounter(i, array[i]);
    }

    std::stable_sort(byteTab.begin(), byteTab.end());   //sort the array by stable algorythm

    while (byteTab[0].counter == 0)                     //pop null counters
        byteTab.pop_front();

    for (long i = 0; i < sortedArraylength; ++i) {      //write it to array
        countersArray[i] = byteTab[i];
    }
}

//  makeHuffmanCodeTree - create binary tree of Huffmans code. Gets sorted array of 
//                      pairs<byte, counter> and merge each pair creating treeNodes
// 
//  class HuffmanTreeNode
//  {
//      unsigned long long byte;
//      long long value;
//      HuffmanTreeNode* leftChild;
//      HuffmanTreeNode* rightChild;
//  }
// 
// input:
//  ByteCounter* sortedInputArray - sorted array of ByteCounters
//  long long sizeOfInputArray    - size of source array
//  HuffmanTreeNode* treeArray    - array to fill with tree nodes
// 
// output:
//   Array of treeNodes where at the begining stands the node with lesed weight.
//   Weights of nodes are growing and the last node in the array is the root.
//   Order of the bytes is preserved.
// 
// Algorythm read two pair of byteCounters, checks if bytes is in range 0..255. 
// If it's so then creates treeNodes in destination array and creates an orphan node
// in source array.Orphan node has counter just like countByte but has pointer to array of
// pointers at stack.This array holds pointers to previously created nodes in output array.
// Orphan node is placed at before first greater or equal index in the array.Rest items are
// shifted, so array stays sorted.In next iterations orphan nodes are recognized and loaded
// to registers with it's pointers.
//
// ################## WARNING - procedure destroyes input array ##################
//
extern "C" __declspec(dllexport) void makeHuffmanCodeTree(ByteCounter* sortedInputArray, long long sizeOfInputArray, HuffmanTreeNode* treeArray) {
    ByteCounter nodePointerWeight;      //orphan node item for sorted array
    ByteCounter insertValue;            //value which insert to sorted array when shifting elements
    ByteCounter receivedValue;          //value which get from sorted array when shifting elements
    HuffmanTreeNode(**treeNodePointerArray) = new HuffmanTreeNode * [sizeOfInputArray * 2]; //array to store pointers
    const unsigned long long maxByteValue = 255;    //max byte size
    long long* firstElemPtr;            //pointer of first orphan node
    long long* secondElemPtr;           //pointer of second orphan node
    HuffmanTreeNode firstNode;          //first created node
    HuffmanTreeNode secondNode;         //seconde created node
    long long treeArrayIterator = 0;    //iterator for tree array
    long long insertIterator = 0;       //index where insert orphan node

    for (long long i = 0; i < sizeOfInputArray - 1; ++i) {

        //get first value
        firstNode.setByte(sortedInputArray[i].byte);
        firstNode.setValue(sortedInputArray[i].counter);

        //get second value
        secondNode.setByte(sortedInputArray[i + 1].byte);
        secondNode.setValue(sortedInputArray[i + 1].counter);

        //if byte in first value is greater than 255 then it is node
        if (firstNode.getByte() > maxByteValue) {
            firstElemPtr = (long long*)firstNode.getByte();
            firstNode.setLeft((HuffmanTreeNode*)firstElemPtr[0]);
            firstNode.setRight((HuffmanTreeNode*)firstElemPtr[1]);
            firstNode.setByte(0);
        }

        //if byte in second value is greater than 255 then it is node
        if (secondNode.getByte() > maxByteValue) {
            secondElemPtr = (long long*)secondNode.getByte();
            secondNode.setLeft((HuffmanTreeNode*)secondElemPtr[0]);
            secondNode.setRight((HuffmanTreeNode*)secondElemPtr[1]);
            secondNode.setByte(0);
        }

        nodePointerWeight.byte = (long long)&treeNodePointerArray[treeArrayIterator];        //get pointer where we can save pointers of the node we create

        treeNodePointerArray[treeArrayIterator] = &treeArray[treeArrayIterator];            //get pointer to first child

        treeArray[treeArrayIterator++] = firstNode;                                         //save child in output array

        treeNodePointerArray[treeArrayIterator] = &treeArray[treeArrayIterator];            //get pointer to second child

        treeArray[treeArrayIterator++] = secondNode;                                        //save child in output array

        nodePointerWeight.counter = firstNode.getValue() + secondNode.getValue();           //calculate weight of the Node

        //find where insert the nodePointerWeight inside sortedIntutArray

        insertIterator = i + 1;
        while (nodePointerWeight.counter > sortedInputArray[insertIterator + 1].counter and insertIterator + 1 < sizeOfInputArray)    //find where we can insert the new node
            ++insertIterator;

        //change values

        receivedValue = sortedInputArray[insertIterator];
        sortedInputArray[insertIterator] = nodePointerWeight;

        insertValue = receivedValue;

        //shift rigth all the values

        for (long long i = insertIterator - 1; i > 0; --i) {
            receivedValue = sortedInputArray[i];
            sortedInputArray[i] = insertValue;
            insertValue = receivedValue;
        }

        firstNode.setRight(nullptr);
        firstNode.setLeft(nullptr);

        secondNode.setLeft(nullptr);
        secondNode.setRight(nullptr);
    }



    treeArray[treeArrayIterator] = HuffmanTreeNode(0, nodePointerWeight.counter, ((HuffmanTreeNode**)nodePointerWeight.byte)[0], ((HuffmanTreeNode**)nodePointerWeight.byte)[1]);

    if (treeNodePointerArray != nullptr) {
        delete[] treeNodePointerArray;
        treeNodePointerArray = nullptr;
    }

    
}

//codeFile - code input byte array by code writed in char** array, where at each index stays 
//            code in ASCII format('0', '1') for byte that index represents. char array has to be
//            null terminated.
// 
// input:
//  short** huffmanCode  - pointer to array of ASCII coded array (short becouse thats the size of char in C#)
//  uint8_t* fileArray   - source array of bytes
//  char* codedArray     - destination array of coded bytes
//  long long fileLength - length of the input array
// output:
//  The procedure reads each input byte, obtains its corresponding code string (from the code table),
//   then writes the bits(0s and 1s) consecutively into the output array.
// 
// Algorythm loads byte from source array reads code of that byte (in ASCII)
// and creates byte based on that code
//
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