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

void HuffmanTreeNode::addValue(long val)
{
	this->value += val;
}

void HuffmanTreeNode::setLeft(HuffmanTreeNode* left)
{
	this->leftChild = left;
}

void HuffmanTreeNode::setRight(HuffmanTreeNode* right)
{
	this->rightChild = right;
}

HuffmanTreeNode* HuffmanTreeNode::getLeft() {
	return leftChild;
}

HuffmanTreeNode* HuffmanTreeNode::getRigth() {
	return rightChild;
}