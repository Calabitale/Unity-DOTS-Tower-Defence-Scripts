using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

public partial class DOTSKDTree : SystemBase
{
    public EntityQuery enemyQuery;
    

    public NativeList<KdNode> KDnative;

    KdNode _last;
    int _root;

    protected override void OnCreate()
    {

        enemyQuery = GetEntityQuery(
            ComponentType.ReadOnly<EnemyDudeTag>(),
            ComponentType.ReadOnly<Translation>());

    }



    protected override void OnUpdate()
    {

        if (enemyQuery.CalculateEntityCount() <= 0)
            return;      

        KDnative = new NativeList<KdNode>(Allocator.Temp);
        var enemytrans = enemyQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        //var timmy = _getSplitValue(KDnative[0].level, KDnative[0].current);

        //Debug.Log("The value is " + timmy);

        for(int i = 0; i < KDnative.Length; i++)
        {
            //_root = i;
            KdNode newnode = new KdNode();
            KDnative.Add(newnode);
            AddNodes(KDnative[i], enemytrans[i].Value, i, _last, _root, KDnative);

        }

        Entities.WithAll<EnemyDudeTag>().
            ForEach((int entityInQueryIndex, in Translation enemytrans) =>
            {
                //var currentNat = KDnative[entityInQueryIndex];
                //currentNat.current = enemytrans.Value;
                //KDnative[entityInQueryIndex] = currentNat;
                //var _root = KDnative[0];
               //var timmy = _getSplitValue(KDnative[entityInQueryIndex].level, KDnative[entityInQueryIndex].current);

                //Debug.Log(timmy);

            }).Run();

        //Debug.Log("The eleventh value is " + KDnative[11].current);

       
        
        Entities.
            //WithStructuralChanges().
            //WithReadOnly(BestDirectionCells).
            //WithReadOnly(TranslutionEnts). 
            //WithDisposeOnCompletion(TranslutionEnts).
            WithAll<EnemyDudeTag>().
            ForEach((Entity medude, ref DamageTaken enemdamage, ref CurrHealth curryheath, ref IsAlive alive) =>
            {
                
                


            }).Schedule();


    }

    //static int partitions(int[] arr, int low, int high)
    //{
    //    int pivot = arr[high], pivotloc = low, temp;
    //    for (int i = low; i <= high; i++)
    //    {
    //        // inserting elements of less value 
    //        // to the left of the pivot location
    //        if (arr[i] < pivot)
    //        {
    //            temp = arr[i];
    //            arr[i] = arr[pivotloc];
    //            arr[pivotloc] = temp;
    //            pivotloc++;
    //        }
    //    }

    //    // swapping pivot to the readonly pivot location
    //    temp = arr[high];
    //    arr[high] = arr[pivotloc];
    //    arr[pivotloc] = temp;

    //    return pivotloc;
    //}
    #region Original KDCode maybe go back to it later
    public static void AddNodes(KdNode newNode, float3 position, int nodesarrayval, KdNode _last, int _root, NativeArray<KdNode> KDnative)
    {
        //_count++;
        newNode.left = 0;
        newNode.right = 0;
        newNode.level = 0;
        var parent = _findParent(position, newNode, KDnative);

        //set last
        if (!_last.isInitialised)
            _last.next = nodesarrayval;
        _last = newNode;

        //set root
        if (parent.isInitialised)
        {
            _root = nodesarrayval;
            return;
        }

        var splitParent = _getSplitValue(parent);
        var splitNew = _getSplitValue(parent.level, position);

        newNode.level = parent.level + 1;

        if (splitNew < splitParent)
            parent.left = nodesarrayval; //go left
        else
            parent.right = nodesarrayval; //go right
    }

    public static float _getSplitValue(int level, float3 position)
    {
        
        
            return (level % 3 == 0) ? position.x : (level % 3 == 1) ? position.y : position.z;
    }

    private static float _getSplitValue(KdNode node)
    {
        return _getSplitValue(node.level, node.currPos);
    }

    public static KdNode _findParent(float3 position, KdNode _root, NativeArray<KdNode> KDnative)
    {
        //travers from root to bottom and check every node
        var current = _root;
        var parent = _root;
        while (current.current != 0)
        {
            var splitCurrent = _getSplitValue(current);
            var splitSearch = _getSplitValue(current.level, position);

            parent = current;
            if (splitSearch < splitCurrent)
                current = KDnative[current.left]; //go left
            else
                current = KDnative[current.right]; //go right

        }
        return parent;
    }

    public struct KdNode //: IComponentData//This is the index method of KDTRee I just assign the index of the nodes to each value
    {
        public bool isInitialised;
        public float3 currPos;
        public int current;
        public int level;//NOTE Right is true of course
        public int left;
        public int right;
        public int next;
        public int oldref;

    }
    #endregion
}

//private float _getSplitValue(KdNode node)
//{
//    return _getSplitValue(node.level, node.component.transform.position);
//}

//private KdNode _findParent(Vector3 position)
//{
//    //travers from root to bottom and check every node
//    var current = _root;
//    var parent = _root;
//    while (current != null)
//    {
//        var splitCurrent = _getSplitValue(current);
//        var splitSearch = _getSplitValue(current.level, position);

//        parent = current;
//        if (splitSearch < splitCurrent)
//            current = current.left; //go left
//        else
//            current = current.right; //go right

//    }
//    return parent;
//}



//}
/// <summary>
/// K is an Enum or LayerMask alternative that allows your key value pairs to be defined in setting files.
/// It provides a way to convert human readable strings into values, even within burst jobs.
/// </summary>
/// <typeparam name="T"> The type of config. </typeparam>
//public static class K<T>
//{
//    private static readonly SharedStatic<KMap> Map = SharedStatic<KMap>.GetOrCreate<KMap, T>();

//    /// <summary> Given a name, returns the user defined value. </summary>
//    /// <param name="name"> The name. </param>
//    /// <returns> The value. </returns>
//    [BurstCompatible]
//    public static byte NameToKey(FixedString32Bytes name)
//    {
//        if (!Map.Data.TryGetValue(name, out var key))
//        {
//#if ENABLE_UNITY_COLLECTIONS_CHECKS
//            Debug.LogError($"{name} does not exist");
//#endif
//        }

//        return key;
//    }


public class KdTree<T> : IEnumerable<T>, IEnumerable where T : Component
{
    protected KdNode _root;
    protected KdNode _last;
    protected int _count;
    protected bool _just2D;
    protected float _LastUpdate = -1f;
    protected KdNode[] _open;

    public int Count { get { return _count; } }
    public bool IsReadOnly { get { return false; } }
    public float AverageSearchLength { protected set; get; }
    public float AverageSearchDeep { protected set; get; }

    /// <summary>
    /// create a tree
    /// </summary>
    /// <param name="just2D">just use x/z</param>
    public KdTree(bool just2D = false)
    {
        _just2D = just2D;
    }

    public T this[int key]
    {
        get
        {
            if (key >= _count)
                throw new ArgumentOutOfRangeException();
            var current = _root;
            for (var i = 0; i < key; i++)
                current = current.next;
            return current.component;
        }
    }

    /// <summary>
    /// add item
    /// </summary>
    /// <param name="item">item</param>
    public void Add(T item)
    {
        _add(new KdNode() { component = item });
    }

    /// <summary>
    /// batch add items
    /// </summary>
    /// <param name="items">items</param>
    public void AddAll(List<T> items)
    {
        foreach (var item in items)
            Add(item);
    }

    /// <summary>
    /// find all objects that matches the given predicate
    /// </summary>
    /// <param name="match">lamda expression</param>
    public KdTree<T> FindAll(Predicate<T> match)
    {
        var list = new KdTree<T>(_just2D);
        foreach (var node in this)
            if (match(node))
                list.Add(node);
        return list;
    }

    /// <summary>
    /// find first object that matches the given predicate
    /// </summary>
    /// <param name="match">lamda expression</param>
    public T Find(Predicate<T> match)
    {
        var current = _root;
        while (current != null)
        {
            if (match(current.component))
                return current.component;
            current = current.next;
        }
        return null;
    }

    /// <summary>
    /// Remove at position i (position in list or loop)
    /// </summary>
    public void RemoveAt(int i)
    {
        var list = new List<KdNode>(_getNodes());
        list.RemoveAt(i);
        Clear();
        foreach (var node in list)
        {
            node._oldRef = null;
            node.next = null;
        }
        foreach (var node in list)
            _add(node);
    }

    /// <summary>
    /// remove all objects that matches the given predicate
    /// </summary>
    /// <param name="match">lamda expression</param>
    public void RemoveAll(Predicate<T> match)
    {
        var list = new List<KdNode>(_getNodes());
        list.RemoveAll(n => match(n.component));
        Clear();
        foreach (var node in list)
        {
            node._oldRef = null;
            node.next = null;
        }
        foreach (var node in list)
            _add(node);
    }

    /// <summary>
    /// count all objects that matches the given predicate
    /// </summary>
    /// <param name="match">lamda expression</param>
    /// <returns>matching object count</returns>
    public int CountAll(Predicate<T> match)
    {
        int count = 0;
        foreach (var node in this)
            if (match(node))
                count++;
        return count;
    }

    /// <summary>
    /// clear tree
    /// </summary>
    public void Clear()
    {


        //rest for the garbage collection
        _root = null;
        _last = null;
        _count = 0;
    }

    /// <summary>
    /// Update positions (if objects moved)
    /// </summary>
    /// <param name="rate">Updates per second</param>
    public void UpdatePositions(float rate)
    {
        if (Time.timeSinceLevelLoad - _LastUpdate < 1f / rate)
            return;

        _LastUpdate = Time.timeSinceLevelLoad;

        UpdatePositions();
    }

    /// <summary>
    /// Update positions (if objects moved)
    /// </summary>
    public void UpdatePositions()
    {
        //save old traverse
        var current = _root;
        while (current != null)
        {
            current._oldRef = current.next;
            current = current.next;
        }

        //save root
        current = _root;

        //reset values
        Clear();

        //readd
        while (current != null)
        {
            _add(current);
            current = current._oldRef;
        }
    }

    /// <summary>
    /// Method to enable foreach-loops
    /// </summary>
    /// <returns>Enumberator</returns>
    public IEnumerator<T> GetEnumerator()
    {
        var current = _root;
        while (current != null)
        {
            yield return current.component;
            current = current.next;
        }
    }

    /// <summary>
    /// Convert to list
    /// </summary>
    /// <returns>list</returns>
    public List<T> ToList()
    {
        var list = new List<T>();
        foreach (var node in this)
            list.Add(node);
        return list;
    }

    /// <summary>
    /// Method to enable foreach-loops
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected float _distance(Vector3 a, Vector3 b)
    {
        if (_just2D)
            return (a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z);
        else
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
    }
    protected float _getSplitValue(int level, Vector3 position)
    {
        if (_just2D)
            return (level % 2 == 0) ? position.x : position.z;
        else
            return (level % 3 == 0) ? position.x : (level % 3 == 1) ? position.y : position.z;
    }

    private void _add(KdNode newNode)
    {
        _count++;
        newNode.left = null;
        newNode.right = null;
        newNode.level = 0;
        var parent = _findParent(newNode.component.transform.position);

        //set last
        if (_last != null)
            _last.next = newNode;
        _last = newNode;

        //set root
        if (parent == null)
        {
            _root = newNode;
            return;
        }

        var splitParent = _getSplitValue(parent);
        var splitNew = _getSplitValue(parent.level, newNode.component.transform.position);

        newNode.level = parent.level + 1;

        if (splitNew < splitParent)
            parent.left = newNode; //go left
        else
            parent.right = newNode; //go right
    }

    private KdNode _findParent(Vector3 position)
    {
        //travers from root to bottom and check every node
        var current = _root;
        var parent = _root;
        while (current != null)
        {
            var splitCurrent = _getSplitValue(current);
            var splitSearch = _getSplitValue(current.level, position);

            parent = current;
            if (splitSearch < splitCurrent)
                current = current.left; //go left
            else
                current = current.right; //go right

        }
        return parent;
    }

    /// <summary>
    /// Find closest object to given position
    /// </summary>
    /// <param name="position">position</param>
    /// <returns>closest object</returns>
    public T FindClosest(Vector3 position)
    {
        return _findClosest(position);
    }

    /// <summary>
    /// Find close objects to given position
    /// </summary>
    /// <param name="position">position</param>
    /// <returns>close object</returns>
    public IEnumerable<T> FindClose(Vector3 position)
    {
        var output = new List<T>();
        _findClosest(position, output);
        return output;
    }

    protected T _findClosest(Vector3 position, List<T> traversed = null)
    {
        if (_root == null)
            return null;

        var nearestDist = float.MaxValue;
        KdNode nearest = null;

        if (_open == null || _open.Length < Count)
            _open = new KdNode[Count];
        for (int i = 0; i < _open.Length; i++)
            _open[i] = null;

        var openAdd = 0;
        var openCur = 0;

        if (_root != null)
            _open[openAdd++] = _root;

        while (openCur < _open.Length && _open[openCur] != null)
        {
            var current = _open[openCur++];
            if (traversed != null)
                traversed.Add(current.component);

            var nodeDist = _distance(position, current.component.transform.position);
            if (nodeDist < nearestDist)
            {
                nearestDist = nodeDist;
                nearest = current;
            }

            var splitCurrent = _getSplitValue(current);
            var splitSearch = _getSplitValue(current.level, position);

            if (splitSearch < splitCurrent)
            {
                if (current.left != null)
                    _open[openAdd++] = current.left; //go left
                if (Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist && current.right != null)
                    _open[openAdd++] = current.right; //go right
            }
            else
            {
                if (current.right != null)
                    _open[openAdd++] = current.right; //go right
                if (Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist && current.left != null)
                    _open[openAdd++] = current.left; //go left
            }
        }

        AverageSearchLength = (99f * AverageSearchLength + openCur) / 100f;
        AverageSearchDeep = (99f * AverageSearchDeep + nearest.level) / 100f;

        return nearest.component;
    }

    private float _getSplitValue(KdNode node)
    {
        return _getSplitValue(node.level, node.component.transform.position);
    }

    private IEnumerable<KdNode> _getNodes()
    {
        var current = _root;
        while (current != null)
        {
            yield return current;
            current = current.next;
        }
    }

    protected class KdNode
    {
        internal T component;
        internal int level;
        internal KdNode left;
        internal KdNode right;
        internal KdNode next;
        internal KdNode _oldRef;
    }
}