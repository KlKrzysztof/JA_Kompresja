#include "pch.h"
#include "Huffman.h"
#include <algorithm>
#include <vector>
#include <deque>

//struct TreeNode
//{
//public:
//    long NodeByte;
//    long NodeValue;
//    TreeNode* LessNode;
//    TreeNode* GreaterNode;
//
//    TreeNode()
//    {
//        NodeByte = 0;
//        NodeValue = 0;
//        LessNode = nullptr;
//        GreaterNode = nullptr;
//    }
//};


/*

//countBytes
//Function witch count the occurrence of all bytes in the 1B integers table
//inputs:
//  byteTable[] - table of tytes to count
//  arraySize - long signed int of the length of the byteTable array
//outputs:
//  long signed int[] - table of counters for the bytes, where index i is counter for byte of value i           0 <= i < 256
////
//extern "C" __declspec(dllexport) long* countBytes(uint8_t* byteTable, long arraySize) {
//    long counterTable[256];                     //table of counters of bytes
//
//    //fill the table with 0
//    for (int i = 0; i < 256; ++i) { 
//        counterTable[i] = 0;
//    }
//
//    //count bytes in the file
//    for (long i = 0; i < arraySize; ++i) {
//        ++counterTable[byteTable[i]];                //increment a value of occurrence of the byte
//    }
//
//    return counterTable;
//}


//makeSortedArray
//input:
//  array - array which should be sorted
//  sortedArrayLength - length of the array to sort
//output:
//  void
//
std::deque<ByteCounter> makeSortedArray(long* array, int sortedArraylength) {
    std::deque<ByteCounter> byteTab = std::deque<ByteCounter>(256);

    //fill the table with bytes and counters
    for (int i = 0; i < 256; ++i) {
        byteTab[i] = ByteCounter(i, array[i]);
    }

    std::sort(byteTab.begin(), byteTab.end());

    while (byteTab[0].counter == 0)
        byteTab.pop_front();

    return byteTab;
}


HuffmanTreeNode* makeHuffmanCodeTree(ByteCounter* sortedInputArray, int sizeOfInputArray) {
    std::vector<HuffmanTreeNode> orthanNodes;
    std::vector<HuffmanTreeNode> codeArray;

    //for(int )

    return &codeArray[0];
}


void codeFile(char** huffmanCode, int fileArray, int codedArray, long fileLength) {

}*/