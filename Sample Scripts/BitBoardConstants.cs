using System.Collections.Generic;
using UnityEngine;

namespace Detective.Puzzles.PushFight
{
    /// <summary>
    /// Several useful constants for the BitBoard functions
    /// </summary>
    public static class BitBoardConstants
    {
        public const uint holes = 0b_1001_0001_0000_0000_0000_0000_1000_1001;
        public const uint canFall = 0b_0110_1010_0001_0000_0000_1000_0101_0110;
        public const uint middleOfBoard = 0b_0000_0000_0000_0110_0110_0000_0000_0000;
        //Does not include "canFall" spots
        public const uint pawnWeakSpots = 0b_0000_0100_1010_0001_1000_0101_0010_0000;

        public const uint fallsPushedUp = 0b_0110_1000_0001_0000_0000_0000_0000_0000;
        public const uint fallsPushedDown = 0b_0000_0000_0000_0000_0000_1000_0001_0110;
        public const uint fallsPushedLeft = 0b_0100_0000_0000_0000_0000_0000_0100_0100;
        public const uint fallsPushedRight = 0b_0010_0010_0000_0000_0000_0000_0000_0010;
        public static readonly uint[] fallsArray = new uint[4] { fallsPushedUp, fallsPushedDown, fallsPushedLeft, fallsPushedRight };

        //Squares that are not holes but have illegal pushes
        //Note that illegal pushes that move the pusher off the board are already
        //accounted for in the dictionary generation function. 
        public const uint illegalUpPush = 0b_0000_1000_0001_0000_0000_0000_0000_0000;
        public const uint illegalLeftPush = 0b_0100_0000_0000_0000_0000_0000_0100_0100;
        public const uint illegalRightPush = 0b_0010_0010_0000_0000_0000_0000_0000_0010;
        public const uint illegalDownPush = 0b_0000_0000_0000_0000_0000_1000_0001_0000;


        public const uint i73 = 1;
        public const uint i72 = 2;
        public const uint i71 = 4;
        public const uint i70 = 8;

        public const uint i63 = 16;
        public const uint i62 = 32;
        public const uint i61 = 64;
        public const uint i60 = 128;   

        public const uint i53 = 256;
        public const uint i52 = 512;
        public const uint i51 = 1024;
        public const uint i50 = 2048;   

        public const uint i43 = 4096;
        public const uint i42 = 8192;
        public const uint i41 = 16384;
        public const uint i40 = 32768;   

        public const uint i33 = 65536;
        public const uint i32 = 131072;
        public const uint i31 = 262144;
        public const uint i30 = 524288;   

        public const uint i23 = 1048576;
        public const uint i22 = 2097152;
        public const uint i21 = 4194304;
        public const uint i20 = 8388608;   

        public const uint i13 = 16777216;
        public const uint i12 = 33554432;
        public const uint i11 = 67108864;
        public const uint i10 = 134217728;

        public const uint i03 = 268435456;
        public const uint i02 = 536870912;
        public const uint i01 = 1073741824;
        public const uint i00 = 2147483648;

        public static readonly uint[, ] indexBitBoards = new uint[Constants.NUM_ROWS, Constants.NUM_COLS]
                                                {
                                                    {i00, i01, i02, i03},
                                                    {i10, i11, i12, i13},
                                                    {i20, i21, i22, i23},
                                                    {i30, i31, i32, i33},
                                                    {i40, i41, i42, i43},
                                                    {i50, i51, i52, i53},
                                                    {i60, i61, i62, i63},
                                                    {i70, i71, i72, i73}
                                                };

        /// <summary>
        /// Given BB representing one pusher, returns the board showing the
        /// (upto) four (possibly) legal pushes the piece can make. 
        /// </summary>
        public static Dictionary<uint, uint> indexToPushes;

        /// <summary>
        /// Given BB representing one pusher, returns the board showing the
        /// (upto) two (possibly) legal horizontal pushes the piece can make. 
        /// </summary>
        public static Dictionary<uint, uint> indexToHorizontalPushes;

        /// <summary>
        /// Given BB representing one pusher, returns the bb showing the 
        /// square furthest from the piece in this row. 
        /// </summary>
        public static Dictionary<uint, uint> indexToFarWall;
        public static Dictionary<uint, uint> indexToFarSide;

        /// <summary>
        /// Given BB representing one pusher, returns the bb showing the 
        /// square closest from the piece in this row. 
        /// </summary>
        public static Dictionary<uint, uint> indexToCloseWall;

        /// <summary>
        /// Attack zones of the blocker pointing up.
        /// </summary>
        public static Dictionary<uint, uint> indexToBlockerUp;

        /// <summary>
        /// Attack zones of the blocker pointing down.
        /// </summary>
        public static Dictionary<uint, uint> indexToBlockerDown;

        /// <summary>
        /// Attack zones of the blocker pointing left.
        /// </summary>
        public static Dictionary<uint, uint> indexToBlockerLeft;

        /// <summary>
        /// Attack zones of the blocker pointing right.
        /// </summary>
        public static Dictionary<uint, uint> indexToBlockerRight;

        /// <summary>
        /// Highlights the row of the given index
        /// </summary>
        public static Dictionary<uint, uint> indexToRow;

        private static void InitIndexToPushes(){
            indexToPushes = new Dictionary<uint, uint>();
            uint index = 0;
            uint pushes = 0;
            uint illegalPush = 0;

            for(int i = 0; i < Constants.NUM_ROWS; i++){
                for(int j = 0; j < Constants.NUM_COLS; j++){
                    index = indexBitBoards[i, j];
                    if((index & holes) != 0) { continue; }

                    if((index & illegalUpPush) != 0) {
                        illegalPush = index << 4;
                    } else if((index & illegalDownPush) != 0){
                        illegalPush = index >> 4;
                    } else if((index & illegalLeftPush) != 0){
                        illegalPush = index << 1;
                    } else if((index & illegalRightPush) != 0){
                        illegalPush = index >> 1;
                    } else {
                        illegalPush = 0;
                    }

                    if(indexToCloseWall.ContainsKey(index)){
                        illegalPush |= indexToCloseWall[index];
                    }   
                    
                    if(i == 0){
                        pushes = (index >> 1) | (index << 1) | (index >> 4);
                    } else if(j == 0){
                        pushes = (index >> 1) | (index >> 4) | (index << 4) ;
                    } else if(i == Constants.NUM_ROWS - 1){
                        pushes = (index >> 1) | (index << 1) | (index << 4);
                    } else if(j == Constants.NUM_COLS - 1){
                        pushes = (index << 1) | (index >> 4) | (index << 4);
                    } else {
                        pushes = (index >> 1) | (index << 1) | (index >> 4) | (index << 4);
                    }

                    indexToPushes.Add(index, pushes & ~illegalPush);
                }
            }
        }

        private static void InitIndexToHorizontalPushes(){
            indexToHorizontalPushes = new Dictionary<uint, uint>();
            uint index = 0;
            uint pushes = 0;
            uint illegalPush = 0;

            for(int i = 0; i < Constants.NUM_ROWS; i++){
                for(int j = 0; j < Constants.NUM_COLS; j++){
                    index = indexBitBoards[i, j];
                    if((index & holes) != 0) { continue; }

                    if((index & illegalUpPush) != 0) {
                        illegalPush = index << 4;
                    } else if((index & illegalDownPush) != 0){
                        illegalPush = index >> 4;
                    } else if((index & illegalLeftPush) != 0){
                        illegalPush = index << 1;
                    } else if((index & illegalRightPush) != 0){
                        illegalPush = index >> 1;
                    } else {
                        illegalPush = 0;
                    }

                    if(indexToCloseWall.ContainsKey(index)){
                        illegalPush |= indexToCloseWall[index];
                    }

                    if(i == 0){
                        pushes = (index >> 1) | (index << 1);
                    } else if(j == 0){
                        pushes = (index >> 1);
                    } else if(i == Constants.NUM_ROWS - 1){
                        pushes = (index >> 1) | (index << 1);
                    } else if(j == Constants.NUM_COLS - 1){
                        pushes = (index << 1);
                    } else {
                        pushes = (index >> 1) | (index << 1);
                    }

                    indexToHorizontalPushes.Add(index, pushes & ~illegalPush);
                }
            }
        }

        private static void InitIndexToCloseWall(){
            indexToCloseWall = new Dictionary<uint, uint>();
            uint index = 0;
            uint close = 0;

            for(int i = 0; i < Constants.NUM_ROWS; i++){
                for(int j = 0; j < Constants.NUM_COLS; j++){
                    index = indexBitBoards[i, j];
                    if ((index & holes) != 0) { continue; }

                    index = indexBitBoards[i, j];
                    close = ((j <= 1) ? indexBitBoards[i, 0] : indexBitBoards[i, Constants.NUM_COLS - 1]) & ~holes;
                    indexToCloseWall.Add(index, close);
                }
            }
        }

        private static void InitIndexToFarWall(){
            indexToFarWall = new Dictionary<uint, uint>();
            uint index = 0;
            uint far = 0;

            for(int i = 0; i < Constants.NUM_ROWS; i++){
                for(int j = 0; j < Constants.NUM_COLS; j++){
                    index = indexBitBoards[i, j];
                    if ((index & holes) != 0) { continue; }

                    index = indexBitBoards[i, j];
                    far = ((j <= 1) ? indexBitBoards[i, Constants.NUM_COLS - 1] : indexBitBoards[i, 0]) & ~holes;
                    indexToFarWall.Add(index, far);
                }
            }
        }

        private static void InitIndexToFarSide()
        {
            indexToFarSide = new Dictionary<uint, uint>();
            uint index = 0;
            uint far = 0;

            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    index = indexBitBoards[i, j];
                    if ((index & holes) != 0) { continue; }

                    index = indexBitBoards[i, j];
                    far = ((j <= 1) ? indexBitBoards[i, Constants.NUM_COLS - 1] : indexBitBoards[i, 0]) & ~holes;
                    far |= ((j <= 1) ? indexBitBoards[i, Constants.NUM_COLS - 2] : indexBitBoards[i, 1]) & ~holes;
                    indexToFarSide.Add(index, far);
                }
            }
        }

        private static void InitIndexToBlockers(){
            indexToBlockerUp = new Dictionary<uint, uint>();
            indexToBlockerDown = new Dictionary<uint, uint>();
            indexToBlockerLeft = new Dictionary<uint, uint>();
            indexToBlockerRight = new Dictionary<uint, uint>();

            uint start = 0;
            uint current = 0;
            uint attacked = 0;

            for(int i = 0; i < Constants.NUM_ROWS; i++){
                for(int j = 0; j < Constants.NUM_COLS; j++){
                    start = indexBitBoards[i, j];
                    if ((start & holes) != 0) { continue; }

                    for (int tmp = i; tmp >= 0; tmp--){
                        current = indexBitBoards[tmp, j];
                        attacked |= current;
                    }

                    indexToBlockerUp.Add(start, attacked);
                    attacked = 0;

                    for(int tmp = i; tmp < Constants.NUM_ROWS; tmp++){
                        current = indexBitBoards[tmp, j];
                        attacked |= current;
                    }

                    indexToBlockerDown.Add(start, attacked);
                    attacked = 0;

                    for(int tmp = j; tmp >= 0; tmp--){
                        current = indexBitBoards[i, tmp];
                        attacked |= current;
                    }

                    indexToBlockerLeft.Add(start, attacked);
                    attacked = 0;

                    for(int tmp = j; tmp < Constants.NUM_COLS; tmp++){
                        current = indexBitBoards[i, tmp];
                        attacked |= current;
                    }

                    indexToBlockerRight.Add(start, attacked);
                    attacked = 0;
                }
            }

            indexToBlockerUp.Add(0, 0);
            indexToBlockerDown.Add(0, 0);
            indexToBlockerLeft.Add(0, 0);
            indexToBlockerRight.Add(0, 0);
        }

        private static void InitIndexToRow(){
            indexToRow = new Dictionary<uint, uint>();
            uint index = 0;
            uint row = 0;

            for(int i = 0; i < Constants.NUM_ROWS; i++){
                for(int j = 0; j < Constants.NUM_COLS; j++){
                    index = indexBitBoards[i, j];
                    if ((index & holes) != 0) { continue; }
                    row = 0;

                    for(int tmp = 0; tmp < Constants.NUM_COLS; tmp++){
                        row |= indexBitBoards[i, tmp];
                    }

                    indexToRow.Add(index, row & ~holes);
                }
            }
        }

        public static void InitializeDictionaries(){
            if(null == indexToCloseWall) { InitIndexToCloseWall(); }
            if(null == indexToFarWall) { InitIndexToFarWall(); }
            if(null == indexToFarSide) { InitIndexToFarSide(); }
            if(null == indexToPushes) { InitIndexToPushes(); }
            if(null == indexToHorizontalPushes) { InitIndexToHorizontalPushes(); }
            if(null == indexToBlockerDown) { InitIndexToBlockers(); }
            if(null == indexToRow) { InitIndexToRow(); }
        }

        /// <summary>
        /// Given an index, finds the corresponding i/j coordinates in the board array.
        /// FOR DEBUGGING ONLY.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static Vector2Int IndexToVector2Int(uint index)
        {
            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    uint o1 = index & BitBoardConstants.indexBitBoards[i, j];
                    if(o1 != 0)
                    {
                        return new Vector2Int(i, j);
                    }
                }
            }

            Debug.LogError("Not found: " + index);
            return new Vector2Int(-1, -1);
        }

        /// <summary>
        /// Converts a push direction to a vector2int. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector2Int PushDirectionToVector2Int(PushDirection direction)
        {
            switch (direction)
            {
                case PushDirection.Up:
                    return new Vector2Int(-1, 0);

                case PushDirection.Down:
                    return new Vector2Int(1, 0);

                case PushDirection.Left:
                    return new Vector2Int(0, -1);

                case PushDirection.Right:
                    return new Vector2Int(0, 1);
            }

            Debug.LogError("Not found!");
            return new Vector2Int(-1, -1);
        }

    }
}
