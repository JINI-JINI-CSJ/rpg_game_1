// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// //using System.Numerics;
// using Clrain.Collections;
// using UnityEngine;

// namespace SJ_AStarGrid
// {
//     public class Node
//     {
//         // Change this depending on what the desired size is for each element in the grid
//         //public static int NODE_SIZE = 32;
//         public Node Parent;
//         public Vector2 Position;
//         public float DistanceToTarget;
//         public float Cost;
//         public float Weight;
//         public float F
//         {
//             get
//             {
//                 if (DistanceToTarget != -1 && Cost != -1)
//                     return DistanceToTarget + Cost;
//                 else
//                     return -1;
//             }
//         }
//         public bool Walkable;

//         public Node(Vector2 pos, bool walkable, float weight = 1)
//         {
//             Parent = null;
//             Position = pos;
//             DistanceToTarget = -1;
//             Cost = 1;
//             Weight = weight;
//             Walkable = walkable;
//         }

//         public void     Log()
//         {
//             Debug.Log( "Node===" );
//             Debug.Log( "DistanceToTarget : " + DistanceToTarget );
//             Debug.Log( "Walkable : " + Walkable );
//             Debug.Log( "F : " + F );
//         }
//     }

//     public class SJ_Astar
//     {
//         //List<List<Node>> Grid;

//         Node[,] Grid;

//         public int GridRows
//         {
//             get
//             {
//                return Grid.GetLength(0);
//             }
//         }
//         public  int GridCols
//         {
//             get
//             {
//                 return Grid.GetLength(1);
//             }
//         }

//         public  void    Alloc_Size( int size )
//         {
//             Grid = new Node[size,size];
//             for( int y = 0 ; y < size ; y++ )
//             {
//                 for( int x = 0 ; x < size ; x++ )
//                 {
//                     Grid[y,x] = new Node( new Vector2( x , y ) , true );
//                 }
//             }
//         }

//         public  void    AllNode_MoveAble()
//         {
//             for( int y = 0 ; y < GridRows ; y++ )
//             {
//                 for( int x = 0 ; x < GridCols ; x++ )
//                 {
//                     Grid[y,x].Walkable = true;
//                 }
//             }
//         }

//         public  void    Set_MoveAble( int x , int y , bool b)
//         {
//             GetNode(x,y).Walkable = b;
//         }

//         public  Node    GetNode( int x , int y )
//         {
//             return Grid[y,x];
//         }

//         public Stack<Node> FindPath( int sx,int sy , int ex , int ey )
//         {
//             return FindPath(new Vector2(sx, sy) , new Vector2(ex, ey) );
//         }



//         public Stack<Node> FindPath(Vector2 Start, Vector2 End)
//         {
            
//             // Node start = new Node(new Vector2((int)(Start.X/Node.NODE_SIZE), (int) (Start.Y/Node.NODE_SIZE)), true);
//             // Node end = new Node(new Vector2((int)(End.X / Node.NODE_SIZE), (int)(End.Y / Node.NODE_SIZE)), true);
//             Node start = new Node(new Vector2((int)(Start.x), (int) (Start.y)), true);
//             Node end = new Node(new Vector2((int)(End.x), (int)(End.y )), true);

//             Stack<Node> Path = new Stack<Node>();

//             //SortedDictionary<float , Node> OpenList = new SortedDictionary<float, Node>();
//             PriorityQueue<float , Node> OpenList = new PriorityQueue<float, Node>();
//             HashSet<Node>               OpenList_hs = new HashSet<Node>();

//             HashSet<Node> ClosedList = new HashSet<Node>();
//             List<Node> adjacencies;
//             Node current = start;
           
//             // add start node to Open List
//             OpenList.Enqueue( start.F  , start );
//             OpenList_hs.Add(start);

//             while(OpenList.Count != 0 && !ClosedList.Contains(end))
//             {
//                 OpenList.Dequeue( out current );
//                 OpenList_hs.Remove( current );

//                 ClosedList.Add(current);
//                 adjacencies = GetAdjacentNodes(current);

//                 foreach(Node n in adjacencies)
//                 {
//                     if (!ClosedList.Contains(n) && n.Walkable)
//                     {
//                         bool isFound = false;

//                         if( OpenList_hs.Contains( n ) )
//                         {
//                             isFound = true;
//                         }

//                         if (!isFound)
//                         {
//                             n.Parent = current;
//                             n.DistanceToTarget = Vector2.Distance( n.Position , end.Position );
//                             n.Cost = n.Weight + n.Parent.Cost;
//                             n.Log();
//                             OpenList.Enqueue( n.F , n );
//                             OpenList_hs.Add(n);
//                         }
//                     }
//                 }
//             }
            
//             // construct path, if end was not closed return null
//             if(!ClosedList.Contains(end))
//             {
//                 return null;
//             }

//             // if all good, return path
//             // Node temp = ClosedList[ClosedList.IndexOf(current)];
//             // if (temp == null) return null;
            
//             Node temp = current;
//             do
//             {
//                 Path.Push(temp);
//                 temp = temp.Parent;
//             } while (temp != start && temp != null) ;

//             return Path;
//         }
		
//         private List<Node> GetAdjacentNodes(Node n)
//         {
//             List<Node> temp = new List<Node>();

//             int row = (int)n.Position.y;
//             int col = (int)n.Position.x;

//             if(row + 1 < GridRows)
//             {

//                 //temp.Add(Grid[col,row + 1]);
//                 temp.Add( GetNode( col , row + 1 ) );
//             }
//             if(row - 1 >= 0)
//             {
//                 //temp.Add(Grid[col , row - 1]);
//                 temp.Add( GetNode( col , row - 1 ) );
//             }
//             if(col - 1 >= 0)
//             {
//                 //temp.Add(Grid[col - 1 , row]);
//                 temp.Add( GetNode( col - 1 , row ) );
//             }
//             if(col + 1 < GridCols)
//             {
//                 //temp.Add(Grid[col + 1 , row]);
//                 temp.Add( GetNode( col + 1 , row ) );
//             }

//             return temp;
//         }
//     }
// }