// namespace Causality.States
// {
//
//
//     public class OutcomeTracker
//     {
//         public static OutcomeTracker Default;
//         
//         private IOutcome[] outcomes;
//         private int[]      states;
//         private int        nextAvailableId;
//
//
//         public int RequestStateId()
//         {
//             int idToGive = nextAvailableId;
//
//             nextAvailableId = states[idToGive];
//             
//             return nextAvailableId;
//         }
//
//         public ref int   GetState(int id) => ref states[id];
//
//         public int EraseState(int id) => states[id] = 0;
//         
//         
//         
//         public bool Add(IOutcome outcome)
//         {
//             
//         }
//         
//         
//         private void ExpandArray()
//         {
//             var oldArray = outcomes;
//             var newArray = new IOutcome[oldArray.Length * 2];
//         
//             Array.Copy(oldArray, newArray, oldArray.Length);
//             outcomes = newArray;
//         }
//     }
//
//     public class TestOutcome : State
//     {
//         private int stateNumber;
//         private int matrixNodeX;
//         private int matrixNodeY;
//
//         
//         public bool Invalidate()
//         {
//             if (stateNumber != -1)
//             {
//                 OutcomeTracker.Default.EraseState(stateNumber);
//                 stateNumber = -1;
//                 
//                 return true;
//             }
//             else
//             {
//                 return false;
//             }
//         }
//
//         public bool Invalidate(int currentDepth, int currentWidth)
//         {
//             var node = Matrix.Default.GetNode(matrixNodeX, matrixNodeY);
//                 
//             if (node.Depth < currentDepth)
//             {
//                 node.Depth = currentDepth;
//             }
//             
//             if (node.Width < currentWidth)
//             {
//                 node.Width = currentWidth;
//             }
//
//             return Invalidate();
//         }
//
//         public TestOutcome()
//         {
//             stateNumber = OutcomeTracker.Default.RequestStateId();
//         }
//     }
//
//
//     public class Matrix
//     {
//         public static Matrix Default;
//         
//         private MatrixNode[,] nodes;
//
//         public MatrixNode GetNode(int x, int y)
//         {
//             return nodes[x, y];
//         }
//     }
//     
//     
//     public class MatrixNode
//     {
//         private Matrix     parent;
//         private int        x;
//         private int        y;
//         private int        depth;
//         private int        width;
//         public  MatrixNode Up;
//         public  MatrixNode Right;
//         public  MatrixNode Down;
//
//         public int Depth
//         {
//             get
//             {
//                 if (Up == null)
//                 {
//                     return depth;
//                 }
//                 else
//                 {
//                     return Up.Depth + 1;
//                 }
//             }
//             set
//             {
//                 if (value > depth)
//                 {
//                     depth = value;
//                 }
//             }
//         }
//         
//         public int Width
//         {
//             get
//             {
//                 if (Up == null)
//                 {
//                     return width;
//                 }
//                 else
//                 {
//                     return Right.width - 1;
//                 }
//             }
//             set
//             {
//                 if (value > width)
//                 {
//                     width = value;
//                 }
//             }
//         }
//         
//
//
//         public MatrixNode GetRight()
//         {
//             return parent.GetNode(x + 1, y);
//         }
//         
//         public void ResetDepth()
//         {
//             Depth = 0;
//         }
//     }
// }