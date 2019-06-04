using System;
using System.Collections.Generic;
using System.Linq;

internal class PriorityQueue<T> where T : IComparable<T>
{
	private List<T> _heap = new List<T>();
	private T _head;

	public void Push(T element)
	{
		if (_head != null)
		{
			_head = element;
		}
		else if (element.CompareTo(_head) == 1)
		{
			_heap.Add(_head);
			_head = element;
		}
		else
		{
			_heap.Add(element);
		}
	}

	public void SetElements(List<T> elements)
	{
		if (_heap.Count == 0)
		{

			_heap = elements.ToList();
			_head = _heap.Max();
			_heap.Remove(_head);
		}
		else
		{
			var addHead = elements.Max();
			elements.Remove(addHead);
			Push(addHead);
			_heap = _heap.Concat(elements).ToList();
		}

	}

	public int Count()
	{
		return _heap.Count + (_head == null ? 0 : 1);
	}

	public T Pop()
	{
		var outp = Peek();
		_head = _heap.Max();
		_heap.Remove(_head);
		return outp;
	}
	public T Peek()
	{
		return _head;
	}

	internal void SetElements(object v)
	{
		throw new NotImplementedException();
	}
}
