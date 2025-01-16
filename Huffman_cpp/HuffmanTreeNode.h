#pragma once
#include <stdint.h>
class HuffmanTreeNode
{
	long byte;
	long value;
	HuffmanTreeNode* leftChild;
	HuffmanTreeNode* rightChild;

public:
	HuffmanTreeNode();

	HuffmanTreeNode(long _byte , long _value, HuffmanTreeNode* _leftChild, HuffmanTreeNode* _rightChild );

	long getByte();

	long getValue();
};

