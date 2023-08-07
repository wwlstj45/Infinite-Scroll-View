using UnityEngine;

public class Node<T>
{
    public T value;
    public Node<T> next;
    public Node<T> prev;
}

public class DoublyLinkedList<T>
{
    public Node<T> head;
    public Node<T> tail;
    public Node<T> root;
    int count;

    public DoublyLinkedList()
    {
        head = null;
    }

    public void Add(T item)
    {
        var newNode = new Node<T>();
        newNode.value = item;
        InsertItem(newNode);
    }

    public void InsertItem(Node<T> newNode)
    {
        if (count == 0)
        {
            head = newNode;
            root = newNode;
            tail = newNode;
            count++;
        }
        else
        {
            // set NewNode's neighbors
            newNode.prev = tail;
            newNode.next = root;
            // set Head Next
            tail.next = newNode;
            // Change Head
            tail = newNode;
            // set Root's neighbor
            root.prev = newNode;
            count++;
        }
    }

    public int Count()
    {
        return count;
    }

    public T Next()
    {
        var item = head.next.value;
        head = head.next;
        return item;
    }
    public T GetItem()
    {
        return head.value;
    }
    public T Prev()
    {
        var item = head.prev.value;
        head = head.prev;
        return item;
    }

    public T GetObjectFromRoot(int idx=0)
    {
        if (idx > count) Debug.LogError($"{idx} is Exceeds the LinkedList's size");
        var item = root;
        for (int i = 0; i < idx; i++)
        {
            item = item.next;
        }
        return item.value;
    }
    public T GetObjectFromTail(int idx = 0)
    {
        if (idx > count) Debug.LogError($"{idx} is Exceeds the LinkedList's size");
        var item = tail;
        for (int i = 0; i < idx; i++)
        {
            item = item.prev;
        }
        return item.value;
    }
}
