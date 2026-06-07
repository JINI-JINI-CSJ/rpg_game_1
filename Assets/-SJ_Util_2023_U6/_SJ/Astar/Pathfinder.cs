using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GridMaster;

//for more on A* visit
//https://en.wikipedia.org/wiki/A*_search_algorithm
namespace Pathfinding
{
    public class Pathfinder
    {
        GridBase gridBase;
        public Node_PFind startPosition;
        public Node_PFind endPosition;

        public volatile bool jobDone = false;
        PathfindMaster.PathfindingJobComplete completeCallback;
        List<Node_PFind> foundPath;

        //Constructor
        public Pathfinder(Node_PFind start, Node_PFind target, PathfindMaster.PathfindingJobComplete callback)
        {
            startPosition = start;
            endPosition = target;
            completeCallback = callback;
            gridBase = GridBase.GetInstance();
        }

        public void FindPath()
        {         
            foundPath = FindPathActual(startPosition, endPosition);

            jobDone = true;
        }

        public void NotifyComplete()
        {
            if(completeCallback != null)
            {
                completeCallback(foundPath);
            }
        }

        public List<Node_PFind> FindPathActual(Node_PFind start, Node_PFind target)
        {
            //Typical A* algorythm from here and on

            List<Node_PFind> foundPath = new List<Node_PFind>();

            //We need two lists, one for the nodes we need to check and one for the nodes we've already checked
            List<Node_PFind> openSet = new List<Node_PFind>();
            HashSet<Node_PFind> closedSet = new HashSet<Node_PFind>();

            //We start adding to the open set
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Node_PFind currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    //We check the costs for the current node
                    //You can have more opt. here but that's not important now
                    if (openSet[i].fCost < currentNode.fCost ||
                        (openSet[i].fCost == currentNode.fCost &&
                        openSet[i].hCost < currentNode.hCost))
                    {
                        //and then we assign a new current node
                        if (!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }

                //we remove the current node from the open set and add to the closed set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                //if the current node is the target node
                if (currentNode.Equals(target))
                {
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, currentNode);
                    break;
                }

                //if we haven't reached our target, then we need to start looking the neighbours
                foreach (Node_PFind neighbour in GetNeighbours(currentNode,true))
                {
                    if (!closedSet.Contains(neighbour))
                    {
                        //we create a new movement cost for our neighbours
                        float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        //and if it's lower than the neighbour's cost
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            //we calculate the new costs
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, target);
                            //Assign the parent node
                            neighbour.parentNode = currentNode;
                            //And add the neighbour node to the open set
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }
            
            //we return the path at the end
            return foundPath;
        }

        private List<Node_PFind> RetracePath(Node_PFind startNode, Node_PFind endNode)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<Node_PFind> path = new List<Node_PFind>();
            Node_PFind currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                //by taking the parentNodes we assigned
                currentNode = currentNode.parentNode;
            }

            //then we simply reverse the list
            path.Reverse();

            return path;
        }

        private List<Node_PFind> GetNeighbours(Node_PFind node, bool getVerticalneighbours = false)
        {
            //This is were we start taking our neighbours


            List<Node_PFind> retList = new List<Node_PFind>();
            // for (int x = -1; x <= 1; x++)
            // {
            //     for (int yIndex = -1; yIndex <= 1; yIndex++)
            //     {
            //         for (int z = -1; z <= 1; z++)
            //         {
            //             int y = yIndex;

            //             //If we don't want a 3d A*, then we don't search the y
            //             if (!getVerticalneighbours)
            //             {
            //                 y = 0;
            //             }

            //             if (x == 0 && y == 0 && z == 0)
            //             {
            //                 //000 is the current node
            //             }
            //             else
            //             {
            //                 Node searchPos = new Node();

            //                 //the nodes we want are what's forward/backwars,left/righ,up/down from us
            //                 searchPos.x = node.x + x;
            //                 searchPos.y = node.y + y;
            //                 searchPos.z = node.z + z;

            //                 Node newNode = GetNeighbourNode(searchPos, true, node);

            //                 if (newNode != null)
            //                 {
            //                     retList.Add(newNode);
            //                 }
            //             }
            //         }
            //     }
            // }

            // X , Y 만 계산
            // 4방향만 
            Node_PFind node_n = null;

            node_n = GetNode(node.x - 1, node.y,0);
            if( node_n != null && node_n.isWalkable ) retList.Add(node_n);

            node_n = GetNode(node.x + 1, node.y,0);
            if( node_n != null && node_n.isWalkable ) retList.Add(node_n);

            node_n = GetNode(node.x , node.y - 1 ,0);
            if( node_n != null && node_n.isWalkable ) retList.Add(node_n);

            node_n = GetNode(node.x , node.y + 1 ,0);
            if( node_n != null && node_n.isWalkable ) retList.Add(node_n);

            return retList;

        }

        private Node_PFind GetNeighbourNode(Node_PFind adjPos, bool searchTopDown, Node_PFind currentNodePos)
        {
            //this is where the meat of it is
            //We can add all the checks we need here to tweak the algorythm to our heart's content
            //but first let's start from the the usual stuff you'll see in A*

            Node_PFind retVal = null;
            
            //let's take the node from the adjacent positions we passed
            Node_PFind node = GetNode(adjPos.x, adjPos.y, adjPos.z);

            //if it's not null and we can walk on it
            if (node != null && node.isWalkable)
            {
                //we can use that node
                retVal = node;
            }//if not
            else if (searchTopDown)//and we want to have 3d A* 
            {
                //then look what the adjacent node have under him
                adjPos.y -= 1;
                Node_PFind bottomBlock = GetNode(adjPos.x, adjPos.y, adjPos.z);
                
                //if there is a bottom block and we can walk on it
                if (bottomBlock != null && bottomBlock.isWalkable)
                {
                    retVal = bottomBlock;// we can return that
                }
                else
                {
                    //otherwise, we look what it has on top of it
                    adjPos.y += 2;
                    Node_PFind topBlock = GetNode(adjPos.x, adjPos.y, adjPos.z);
                    if (topBlock != null && topBlock.isWalkable)
                    {
                        retVal = topBlock;
                    }
                }
            }

            //if the node is diagonal to the current node then check the neighbouring nodes
            //so to move diagonally, we need to have 4 nodes walkable
            int originalX = adjPos.x - currentNodePos.x;
            int originalZ = adjPos.z - currentNodePos.z;

            if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
            {
                // the first block is originalX, 0 and the second to check is 0, originalZ
                //They need to be pathfinding walkable
                Node_PFind neighbour1 = GetNode(currentNodePos.x + originalX, currentNodePos.y, currentNodePos.z);
                if (neighbour1 == null || !neighbour1.isWalkable)
                {
                    retVal = null;
                }

                Node_PFind neighbour2 = GetNode(currentNodePos.x, currentNodePos.y, currentNodePos.z + originalZ);
                if (neighbour2 == null || !neighbour2.isWalkable)
                {
                    retVal = null;
                }
            }

            //and here's where we can add even more additional checks
            if (retVal != null)
            {
                //Example, do not approach a node from the left
                /*if(node.x > currentNodePos.x) {
                    node = null;
                }*/
            }

            return retVal;
        }

        private Node_PFind GetNode(int x, int y, int z)
        {
            Node_PFind n = null;

            lock(gridBase)
            {
                n = gridBase.GetNode(x, y, z);
            }
            return n;
        }

        private int GetDistance(Node_PFind posA, Node_PFind posB)
        {
            //We find the distance between each node
            //not much to explain here

            int distX = Mathf.Abs(posA.x - posB.x);
            int distZ = Mathf.Abs(posA.z - posB.z);
            int distY = Mathf.Abs(posA.y - posB.y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }

    }
}
