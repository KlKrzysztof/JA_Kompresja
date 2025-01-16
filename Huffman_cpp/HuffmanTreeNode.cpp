#include "pch.h"
#include "HuffmanTreeNode.h"

HuffmanTreeNode::HuffmanTreeNode(): 
	byte(0), value(0), leftChild(nullptr), rightChild(nullptr)
{}

HuffmanTreeNode::HuffmanTreeNode(long _byte, long _value, HuffmanTreeNode* _leftChild, HuffmanTreeNode* _rightChild): 
	byte(_byte), value(_value), leftChild(_leftChild), rightChild(_rightChild)
{}

long HuffmanTreeNode::getByte()
{
	return this->byte;
}

long HuffmanTreeNode::getValue()
{
	return value;
}
