using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _heap = new List<T>();

    // O(logN)
    public void Push(T data)
    {
        _heap.Add(data);

        var now = _heap.Count - 1;
        while (now > 0)
        {
            var next = (now - 1) / 2;
            if (_heap[now].CompareTo(_heap[next]) < 0)
            {
                break;
            }

            (_heap[now], _heap[next]) = (_heap[next], _heap[now]);
            now = next;
        }
    }

    // O(logN)
    public T Pop()
    {
        var ret = _heap[0];

        var lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        lastIndex--;

        var now = 0;
        while (true)
        {
            var left = 2 * now + 1;
            var right = 2 * now + 2;

            var next = now;
            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
            {
                next = left;
            }

            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
            {
                next = right;
            }

            if (next == now)
            {
                break;
            }

            (_heap[now], _heap[next]) = (_heap[next], _heap[now]);
            now = next;
        }

        return ret;
    }

    public int Count => _heap.Count;
}