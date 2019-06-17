using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraphActor : MonoBehaviour
{
	private Node<Polygon> _currentNode;
	internal delegate void NodeChangedHandle(Node<Polygon> poly);
	internal event NodeChangedHandle OnNodeChanged;
	internal Node<Polygon> CurrentNode
	{
		get => _currentNode;
		set
		{
			OnNodeChanged?.Invoke(value);
			_currentNode = value;
		}
	}

	void MoveNode(Node<Polygon> node)
	{

	}



}
