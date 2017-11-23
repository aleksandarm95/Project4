using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Pair of left and right data half.
    /// </summary>
	public class BlockPair
    {
        byte[] left;
        byte[] right;

        /// <summary>
        /// Parametrized Constructor of Block Pair class
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
		public BlockPair(byte[] left, byte[] right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Default Constructor of Block Pair class
        /// </summary>
		public BlockPair()
        {
            this.Left = new byte[32];
            this.Right = new byte[32];
        }

        /// <summary>
        /// Function we use to switch left and right side.
        /// </summary>
		public void Rotate()
        {
            byte[] temp = this.Left;
            this.Left = this.Right;
            this.Right = temp;
        }

        /// <summary>
        /// Property of field left in Block Pair class
        /// </summary>
		public byte[] Left
        {
            get
            {
                return left;
            }

            set
            {
                left = value;
            }
        }

        /// <summary>
        /// Property of field right in Block Pair class
        /// </summary>
		public byte[] Right
        {
            get
            {
                return right;
            }

            set
            {
                right = value;
            }
        }
    }
}
