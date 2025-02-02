#pragma once
#include <stdint.h>
class HuffmanTreeNode
{
	unsigned long long byte;
	long long value;
	HuffmanTreeNode* leftChild;
	HuffmanTreeNode* rightChild;

public:
	HuffmanTreeNode();

	HuffmanTreeNode(long _byte , long _value, HuffmanTreeNode* _leftChild, HuffmanTreeNode* _rightChild );

	unsigned long long getByte();

	void setByte(unsigned long long b);

	long long getValue();

	void setValue(long long v);

	void addValue(long long val);

	void setLeft(HuffmanTreeNode* left);

	void setRight(HuffmanTreeNode* right);

	HuffmanTreeNode* getLeft();

	HuffmanTreeNode* getRigth();
};

