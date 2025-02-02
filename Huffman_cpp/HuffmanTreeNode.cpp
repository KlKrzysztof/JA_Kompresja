#include "pch.h"
#include "HuffmanTreeNode.h"

HuffmanTreeNode::HuffmanTreeNode(): 
	byte(0), value(0), leftChild(nullptr), rightChild(nullptr)
{}

HuffmanTreeNode::HuffmanTreeNode(long _byte, long _value, HuffmanTreeNode* _leftChild, HuffmanTreeNode* _rightChild): 
	byte(_byte), value(_value), leftChild(_leftChild), rightChild(_rightChild)
{}

unsigned long long HuffmanTreeNode::getByte()
{
	return this->byte;
}

void HuffmanTreeNode::setByte(unsigned long long b)
{
	this->byte = b;
}

long long HuffmanTreeNode::getValue()
{
	return value;
}

void HuffmanTreeNode::setValue(long long v)
{
	this->value = v;
}

void HuffmanTreeNode::addValue(long long val)
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