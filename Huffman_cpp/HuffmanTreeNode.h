#pragma once
#include <stdint.h>
class HuffmanTreeNode
{
	long long byte;
	long long value;
	HuffmanTreeNode* leftChild;
	HuffmanTreeNode* rightChild;

public:
	HuffmanTreeNode();

	HuffmanTreeNode(long _byte , long _value, HuffmanTreeNode* _leftChild, HuffmanTreeNode* _rightChild );

	long getByte();

	long getValue();

	void addValue(long val);

	void setLeft(HuffmanTreeNode* left);

	void setRight(HuffmanTreeNode* right);

	HuffmanTreeNode* getLeft();

	HuffmanTreeNode* getRigth();
};

