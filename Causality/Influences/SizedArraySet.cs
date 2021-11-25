// using Core.Tools;
//
// namespace Causality.States
// {
//     public class SizedArraySet<T>
//     {
//         private T[][] arrays;
//         private int   nextAvailableIndex;
//
//         public T[] GetArrayOfSize(int size)
//         {
//             if (size >= nextAvailableIndex)
//             {
//                 arrays = CreateArrayOfSize(size);
//             }
//             else
//             {
//                 return arrays[size];
//             }
//         }
//
//         protected T[] CreateArrayOfSize(int size)
//         {
//             var currentArrays = arrays;
//             
//             if (size > currentArrays.Length)
//             {
//                 arrays = Collections.ExpandArrayToAtLeast(currentArrays, size);
//             }
//
//             ref var appropriateSlot = ref arrays[size]; 
//             
//             if (appropriateSlot != null)
//             {
//                 return appropriateSlot;
//             }
//             else
//             {
//                 appropriateSlot = new T[size];
//             }
//
//             return arrays[size];
//         }
//
//     }
// }