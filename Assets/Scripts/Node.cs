using System;
using System.Collections.Generic;
using UnityEngine;

public class Node<T> : IEquatable<Node<T>> where T : IEquatable<T>
{
	public T Data { get; set; }

	//internal List<Node<T>> Children { get; set; } = new List<Node<T>>();

	public Dictionary<float, Node<T>> ConnectionAngles { get; set; } = new Dictionary<float, Node<T>>();

	public Node(T data)
	{
		Data = data;
	}

	public bool Equals(Node<T> other)
	{
		return other.Data.Equals(Data);
	}

}