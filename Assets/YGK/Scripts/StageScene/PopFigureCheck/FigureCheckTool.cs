using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   static class FigureCheckTool
   {
      public static Block GetBlock(Block src, HexaDirection to)
      {
         var map = BlockMap.Instance;
         int row = src.Row;
         int col = src.Col;

         var delta = HexaDirections.GetDelta(col);
         var nr = row + delta[(int)to].row;
         var nc = col + delta[(int)to].col;

         if (map.IsIndexOutOfRange(nr, nc)) return null;
         if (map.IsIndexNotEnabled(nr, nc)) return null;
         var block = map.GetBlock(nr, nc);
         return block;
      }

      public static Block GetBlock(Block src, int colDelta)
      {
         var map = BlockMap.Instance;
         int row = src.Row;
         int col = src.Col + colDelta;

         if (map.IsIndexOutOfRange(row, col)) return null;
         if (map.IsIndexNotEnabled(row, col)) return null;
         var block = map.GetBlock(col, col);
         return block;
      }

      public static void CollectTo(Block src, HexaDirection to, List<Block> collected)
      {
         var map = BlockMap.Instance;
         int currRow = src.Row;
         int currCol = src.Col;
         int nextRow;
         int nextCol;

         // move to from
         while (true)
         {
            var delta = HexaDirections.GetDelta(currCol);
            nextRow = currRow + delta[(int)to].row;
            nextCol = currCol + delta[(int)to].col;
            if (map.IsIndexOutOfRange(nextRow, nextCol)) break;
            if (map.IsIndexNotEnabled(nextRow, nextCol)) break;
            var nextBlock = map.GetBlock(nextRow, nextCol);
            if (nextBlock == null) break;
            if (!nextBlock.CanMatchWith(src)) break;

            currRow = nextRow;
            currCol = nextCol;
            collected.Add(nextBlock);
         }
      }

      /// <summary>
      /// Line Collect
      /// </summary>
      public static void CollectFromTo(Block src, HexaDirection from, HexaDirection to, List<Block> collected)
      {
         var map = BlockMap.Instance;
         int currRow = src.Row;
         int currCol = src.Col;
         int nextRow;
         int nextCol;

         // move to from
         while (true)
         {
            var delta = HexaDirections.GetDelta(currCol);
            nextRow = currRow + delta[(int)from].row;
            nextCol = currCol + delta[(int)from].col;
            if (map.IsIndexOutOfRange(nextRow, nextCol)) break;
            if (map.IsIndexNotEnabled(nextRow, nextCol)) break;
            var nextBlock = map.GetBlock(nextRow, nextCol);
            if (nextBlock == null) break;
            if (!nextBlock.CanMatchWith(src)) break;

            currRow = nextRow;
            currCol = nextCol;
         }

         // collect from ~ to
         collected.Clear();
         collected.Add(map.GetBlock(currRow, currCol));
         while (true)
         {
            var delta = HexaDirections.GetDelta(currCol);
            nextRow = currRow + delta[(int)to].row;
            nextCol = currCol + delta[(int)to].col;
            if (map.IsIndexOutOfRange(nextRow, nextCol)) break;
            if (map.IsIndexNotEnabled(nextRow, nextCol)) break;
            var nextBlock = map.GetBlock(nextRow, nextCol);
            if (nextBlock == null) break;
            if (!nextBlock.CanMatchWith(src)) break;

            collected.Add(nextBlock);
            currRow = nextRow;
            currCol = nextCol;
         }
      }
   }
}