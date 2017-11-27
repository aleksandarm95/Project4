using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class DES
    {

        const int BLOCK_SIZE = 64; //size of block we want to encrypt and decrypt
        static int[] numberOfKeyShifts = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 }; //number of key shifts for each cycle 

        /* initial permutation IP */
        static int[] ip = {
            40,  8, 48, 16, 56, 24, 64, 32,
            39,  7, 47, 15, 55, 23, 63, 31,
            38,  6, 46, 14, 54, 22, 62, 30,
            37,  5, 45, 13, 53, 21, 61, 29,
            36,  4, 44, 12, 52, 20, 60, 28,
            35,  3, 43, 11, 51, 19, 59, 27,
            34,  2, 42, 10, 50, 18, 58, 26,
            33,  1, 41,  9, 49, 17, 57, 25
        };

        /* final permutation IP^-1 */
        static int[] fp = {
            58, 50, 42, 34, 26, 18, 10,  2,
            60, 52, 44, 36, 28, 20, 12,  4,
            62, 54, 46, 38, 30, 22, 14,  6,
            64, 56, 48, 40, 32, 24, 16,  8,
            57, 49, 41, 33, 25, 17,  9,  1,
            59, 51, 43, 35, 27, 19, 11,  3,
            61, 53, 45, 37, 29, 21, 13,  5,
            63, 55, 47, 39, 31, 23, 15,  7
        };

        /* key permutation */
        static int[] keyp = {
            5, 24, 7, 16, 6, 10, 20,  18,
            -1, 12, 3, 15, 23, 1, 9,  19,
            2, -1, 14, 22, 11, -1, 13,  4,
            -1, 17, 21, 8, 47, 31, 27,  48,
            35, 41, -1, 46, 28, -1,  39,  32,
            25, 44, -1, 37, 34, 43, 29,  36,
            38, 45, 33, 26, 42, -1, 30,  40
        };

        /* expansion permutation */
        static int[] ep = {
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8, 9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 1
        };

        /* last permutation in FeistelNetwork */
        static int[] per = {
            9, 17, 23, 31, 13, 28, 2, 18,
            24, 16, 30, 6, 26, 20, 10, 1,
            8, 14, 25, 3, 4, 29, 11, 19,
            32, 12, 22, 7, 5, 27, 15, 21
        };

        /* substitution matrix */
        static byte[,] sBoxes =
        {
            {
                14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
                0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
                4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
                15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13
            },
            {
                15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
                3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
                0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
                13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9
            },
            {
                10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
                13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
                13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
                1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12
            },
            {
                7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
                13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
                10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
                3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14
            },
            {
                2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
                14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
                4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
                11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3
            },
            {
                12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
                10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
                9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
                4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13
            },
            {
                4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
                13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
                1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
                6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12
            },
            {
                13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
                1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
                7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
                2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11
            }
        };

        ///<summary>
        ///Function we use to encrypt message we want to send.
        ///</summary>
        ///<param name="input"> Message we want to send. </param>
        ///<param name="firstyKey"> First key we use to encrypt message.</param>
        ///<param name="secondKey"> Second key we use to decrypt message.</param>
        ///<param name="thirdKey"> Third key we use to encrypt message.</param>
        ///<param name="paralelization"> Do we want paralelization in encrypt or not.</param>
        ///<returns>Encrypted byte[]</returns>
        public static byte[] TripleEncrypt(string input, string firstyKey, string secondKey, string thirdKey, bool paralelization)
        {

            if (input == null || input.Equals(string.Empty) || input.Equals('\n') || firstyKey == null || secondKey == null || thirdKey == null || firstyKey.Length != 8 || secondKey.Length != 8 || thirdKey.Length != 8)
            {
                Console.WriteLine("TripleEncrypt: Input error!");
                return null;
            }

            try
            {
                byte[] output = Encrypt(input, firstyKey, paralelization);
                string decStr = Decrypt(output, secondKey, paralelization);
                output = Encrypt(decStr, thirdKey, paralelization);
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"TripleEncrypt: {e.Message}");
                return null;
            }
        }

        ///<summary>
        ///Function we use to decrypt message we recieved.
        ///</summary>
        ///<param name="input"> Message we recieved. </param>
        ///<param name="firstyKey"> First key we use to decrypt message.</param>
        ///<param name="secondKey"> Second key we use to encrypt message.</param>
        ///<param name="thirdKey"> Third key we use to decrypt message.</param>
        ///<param name="paralelization"> Do we want paralelization in decrypt or not.</param>
        ///<returns>Decrypted message string</returns>
        public static string TripleDecrypt(byte[] input, string firstyKey, string secondKey, string thirdKey, bool paralelization)
        {

            if (input == null || input.Length < 1 || firstyKey == null || secondKey == null || thirdKey == null || firstyKey.Length != 8 || secondKey.Length != 8 || thirdKey.Length != 8)
            {
                Console.WriteLine("TripleDecrypt: Input error!");
                return null;
            }

            try
            {
                string decStr = Decrypt(input, thirdKey, paralelization);
                input = Encrypt(decStr, secondKey, paralelization);
                decStr = Decrypt(input, firstyKey, paralelization);
                string[] retVal = decStr.Split('\0');
                return retVal[0];
            }
            catch (Exception e)
            {
                Console.WriteLine($@"TripleDecrypt: {e.Message}");
                return null;
            }
        }

        ///<summary>
        ///Function we use to encrypt message we want to send.
        ///</summary>
        ///<param name="input"> Message we want to send. </param>
        ///<param name="keyString"> Key we use to encrypt message.</param>
        ///<param name="paralelization"> Do we want paralelization in encrypt or not.</param>
        ///<returns>Encrypted byte[]</returns>
        public static byte[] Encrypt(string input, string keyString, bool paralelization)
        {

            if (input == null || input.Equals(string.Empty) || input.Equals('\n') || keyString == null || keyString.Length != 8)
            {
                throw new ArgumentException("Input parameters are incorrect!");
            }

            byte[] plainText = CheckInputLenght(StringToByte(input));

            byte[][] blocks = GetBlocks(plainText);

            int rows = plainText.Length / BLOCK_SIZE;

            byte[][] encryptResult = new byte[rows][];

            for (int i = 0; i < rows; i++)
            {
                encryptResult[i] = new byte[BLOCK_SIZE];
            }

            Dictionary<int, byte[]> keys = new Dictionary<int, byte[]>();
            for (int i = 0; i < 16; i++)
            {
                keys[i] = KeyGenerate(keyString, i);
            }

            List<Thread> threadList = new List<Thread>();

            for (int cycle = 0; cycle < rows; cycle++)
            {
                int tempID = cycle;

                threadList.Add(new Thread(() => {
                    encryptResult[tempID] = RunEncrypt(blocks[tempID], keys, tempID);
                }));
                threadList.Last().Start();

                if (!paralelization)
                {
                    threadList.Last().Join();
                }
            }

            byte[] output = new byte[plainText.Length];

            if (paralelization)
            {
                foreach (Thread t in threadList)
                {
                    t.Join();
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < BLOCK_SIZE; j++)
                {
                    output[i * BLOCK_SIZE + j] = encryptResult[i][j];
                }
            }

            return output;
        }

        ///<summary>
        ///Function we use like task, for encrypt one block of message.
        ///</summary>
        ///<param name="block">Block we want to encrypt</param>
        ///<param name="keys">Array of keys we use for encrypt</param>
        ///<param name="id">Serial number of block</param>
        static byte[] RunEncrypt(byte[] block, Dictionary<int, byte[]> keys, int id)
        {

            if (block.Length != BLOCK_SIZE)
            {
                throw new ArgumentException($"Block size must be {BLOCK_SIZE} bits!");
            }

            byte[] retVal = new byte[BLOCK_SIZE];

            block = Permute(block, block.Length, ip);

            byte[] right = new byte[BLOCK_SIZE / 2];
            byte[] left = new byte[BLOCK_SIZE / 2];

            for (int i = 0, j = 0; i < BLOCK_SIZE; i++)
            {
                if (i < BLOCK_SIZE / 2)
                {
                    right[i] = block[i];
                }
                else
                {
                    left[j++] = block[i];
                }
            }

            BlockPair pair = new BlockPair(left, right);

            for (int i = 0; i < 16; i++)
            {
                pair = FeistelNetwork(pair, keys[i]);
            }

            retVal = Merge(pair.Left, pair.Right);

            retVal = Permute(retVal, retVal.Length, fp);

            return retVal;
        }

        ///<summary>
        ///Function we use to decrypt message we recieved.
        ///</summary>
        ///<param name="input"> Message we recieved. </param>
        ///<param name="keyString"> Key we use to decrypt message.</param>
        ///<param name="paralelization"> Do we want paralelization in decrypt or not.</param>
        ///<returns>Decrypted message string</returns>
        public static string Decrypt(byte[] input, string keyString, bool paralelization)
        {

            if (input == null || input.Length < 1 || keyString == null || keyString.Length != 8)
            {
                throw new ArgumentException("Input parameters are incorrect!");
            }

            byte[] chipeText = CheckInputLenght(input);

            byte[][] blocks = GetBlocks(chipeText);

            int rows = chipeText.Length / BLOCK_SIZE;

            byte[][] decryptResult = new byte[rows][];

            for (int i = 0; i < rows; i++)
            {
                decryptResult[i] = new byte[BLOCK_SIZE];
            }

            Dictionary<int, byte[]> keys = new Dictionary<int, byte[]>();
            for (int i = 0; i < 16; i++)
            {
                keys[15 - i] = KeyGenerate(keyString, i);
            }

            List<Thread> threadList = new List<Thread>();

            for (int cycle = 0; cycle < rows; cycle++)
            {
                int tempID = cycle;

                threadList.Add(new Thread(() => {
                    decryptResult[tempID] = RunDecrypt(blocks[tempID], keys, tempID);
                }));
                threadList.Last().Start();

                if (!paralelization)
                {
                    threadList.Last().Join();
                }
            }

            byte[] output = new byte[chipeText.Length];
            char[] outArray = new char[output.Length / 8];
            byte[] sub = new byte[8];

            if (paralelization)
            {
                foreach (Thread t in threadList)
                {
                    t.Join();
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < BLOCK_SIZE; j++)
                {
                    output[i * BLOCK_SIZE + j] = decryptResult[i][j];
                }
            }

            for (int i = 0, j = 0; i < output.Length; i += 8)
            {
                Array.Copy(output, i, sub, 0, 8);
                outArray[j++] = (char)ByteToInt(sub);
            }

            string outStr = new string(outArray);

            return outStr;
        }

        ///<summary>
        ///Function we use like task, for decrypt one block of message.
        ///</summary>
        ///<param name="block">Block we want to decrypt</param>
        ///<param name="keys">Array of keys we use for decrypt</param>
        ///<param name="id">Serial number of block</param>
        static byte[] RunDecrypt(byte[] block, Dictionary<int, byte[]> keys, int id)
        {

            if (block.Length != BLOCK_SIZE)
            {
                throw new ArgumentException($"Block size must be {BLOCK_SIZE} bits!");
            }

            byte[] retVal = new byte[BLOCK_SIZE];

            block = Permute(block, block.Length, ip);

            byte[] right = new byte[BLOCK_SIZE / 2];
            byte[] left = new byte[BLOCK_SIZE / 2];

            for (int i = 0, j = 0; i < BLOCK_SIZE; i++)
            {
                if (i < BLOCK_SIZE / 2)
                {
                    right[i] = block[i];
                }
                else
                {
                    left[j++] = block[i];
                }
            }

            BlockPair pair = new BlockPair(left, right);
            pair.Rotate();

            for (int i = 0; i < 16; i++)
            {
                pair = FeistelNetwork(pair, keys[i]);
            }

            pair.Rotate();

            retVal = Merge(pair.Left, pair.Right);

            retVal = Permute(retVal, retVal.Length, fp);

            return retVal;
        }

        /// <summary>
        /// Function we use like one cycle.
        /// </summary>
        /// <param name="input">Pair of left and right side of block</param>
        /// <param name="key">Array of keys we use for encrypt and decrypt</param>
        /// <returns>New pair of left and right side of block.</returns>
        static BlockPair FeistelNetwork(BlockPair input, byte[] key)
        {

            if (input == null || key == null)
            {
                throw new ArgumentException("Block pair or key can not be null!");
            }

            BlockPair retVal = new BlockPair();
            retVal.Left = input.Right;

            byte[] rightEP = Permute(input.Right, 48, ep);

            rightEP = XOR(key, rightEP);
            retVal.Right = Substitution(rightEP);
            retVal.Right = Permute(retVal.Right, retVal.Right.Length, per);
            retVal.Right = XOR(input.Left, retVal.Right);

            return retVal;
        }

        /// <summary>
        /// Binary XOR operation for two byte array.
        /// </summary>
        /// <param name="first">First byte array</param>
        /// <param name="second">Second byte array</param>
        /// <returns>Result of xor operation</returns>
        static byte[] XOR(byte[] first, byte[] second)
        {

            if (first.Length < 1 || first.Length != second.Length)
            {
                throw new ArgumentException("XOR arguments is not same size!");
            }

            byte[] result = new byte[first.Length];
            for (int i = 0; i < first.Length; i++)
            {
                result[i] = (byte)(first[i] ^ second[i]);
            }
            return result;
        }

        /// <summary>
        /// One of the steps in cycle. We use [,]sBoxes to change size of right data half from 48 to 32 bits 
        /// </summary>
        /// <param name="input">Right data half after xor operation between key and expansion permuted right data half</param>
        /// <returns>32 bits size right data half</returns>
        static byte[] Substitution(byte[] input)
        {

            if (input == null)
            {
                throw new ArgumentException("Substitution can not be null!");
            }

            if (input.Length != 48)
            {
                throw new ArgumentException("Substitution input size must bi 48 bits!");
            }

            byte[] retVal = new byte[32];
            byte[] sub = new byte[6];
            byte[] row = new byte[2];
            byte[] col = new byte[4];

            for (int i = 0, j = 0, s = 0; i < input.Length; i += 6)
            {
                Array.Copy(input, i, sub, 0, 6);
                row[0] = sub[0];
                row[1] = sub[5];
                Array.Copy(sub, 1, col, 0, 4);
                int r = ByteToInt(row);
                int c = ByteToInt(col);

                int sboxVal = sBoxes[s++, r * 16 + c];

                Array.Copy(IntToByte(sboxVal, 4), 0, retVal, j, 4);
                j += 4;
            }

            return retVal;
        }

        /// <summary>
        /// Convert byte array to integer.
        /// </summary>
        /// <param name="input">Byte array we want to convert</param>
        /// <returns>Byte array like integer</returns>
        static int ByteToInt(byte[] input)
        {

            if (input.Length < 1 || input.Length > 32)
            {
                throw new ArgumentException("Wrong input size!");
            }

            int retVal = (int)input[0];

            for (int i = 1; i < input.Length; i++)
            {
                retVal += ((int)Math.Pow(2, i)) * input[i];
            }

            return retVal;
        }

        /// <summary>
        /// Convert integer to byte array
        /// </summary>
        /// <param name="number">Integer we want to convert</param>
        /// <param name="length">Length of byte array we want to create</param>
        /// <returns>Byte array</returns>
        static byte[] IntToByte(int number, int length)
        {

            if (number < 0 || length < 1 || length > 32)
            {
                throw new ArgumentException("Wrong input size!");
            }

            byte[] retVal = new byte[length];

            for (int i = 0; i < length; i++)
            {
                retVal[i] = (byte)(number & 0x00000001);
                number >>= 1;
            }

            return retVal;
        }

        /// <summary>
        /// Convert string to byte array.
        /// </summary>
        /// <param name="input">String we want to convert</param>
        /// <returns>Byte array</returns>
        static byte[] StringToByte(string input)
        {

            if (input.Length < 1)
            {
                throw new ArgumentException("Wrong input size!");
            }

            byte[] retVal = new byte[input.Length * 8];
            int s = 0;

            for (int i = 0, j = 0; i < input.Length; i++, j += 8)
            {
                s = input[i];
                Array.Copy(IntToByte(s, 8), 0, retVal, j, 8);
            }

            return retVal;
        }

        /// <summary>
        /// Split plaintext to blocks
        /// </summary>
        /// <param name="input">Plaintext we want to split</param>
        /// <returns>Array of blocks</returns>
        static byte[][] GetBlocks(byte[] input)
        {

            if (input.Length % BLOCK_SIZE != 0)
            {
                throw new ArgumentException("Block size error!");
            }

            int coloms = BLOCK_SIZE;
            int rows = input.Length / BLOCK_SIZE;

            byte[][] retVal = new byte[rows][];
            for (int i = 0; i < rows; i++)
            {
                retVal[i] = new byte[coloms];
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < coloms; j++)
                {
                    retVal[i][j] = input[i * BLOCK_SIZE + j];
                }
            }

            return retVal;
        }

        /// <summary>
        /// Function we use to check lenght. If the length is less than 64 bits we fill it with zeroes to make 64 bits length. 
        /// </summary>
        /// <param name="input">Byte array we check</param>
        /// <returns>64 bits byte array</returns>
        static byte[] CheckInputLenght(byte[] input)
        {

            if (input == null || input.Length < 1)
            {
                throw new ArgumentException("Block can not be null or empty!");
            }

            if (input.Length % BLOCK_SIZE == 0)
            {
                return input;
            }

            int size = input.Length + BLOCK_SIZE - (input.Length % BLOCK_SIZE);
            byte[] retVal = new byte[size];

            for (int i = 0; i < size; i++)
            {
                if (i < input.Length)
                {
                    retVal[i] = input[i];
                }
                else
                {
                    retVal[i] = 0;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Function we use to permute byte array
        /// </summary>
        /// <param name="input">Byte Array we want to pemrute</param>
        /// <param name="size">Size of new byte array</param>
        /// <param name="positions">Array of new positions</param>
        /// <returns>Permuted byte array</returns>
        static byte[] Permute(byte[] input, int size, int[] positions)
        {

            if (input == null || size < 1 || input.Length < 1)
            {
                throw new ArgumentException("Block can not be null or empty!");
            }

            byte[] retVal = new byte[size];

            for (int i = 0; i < size; i++)
            {
                retVal[i] = input[positions[i] - 1];
            }

            return retVal;
        }

        /// <summary>
        /// Function we use to merge left and right data half.
        /// </summary>
        /// <param name="left">Left data half</param>
        /// <param name="right">Right data half</param>
        /// <returns>Merged left and right data half in one byte array</returns>
        static byte[] Merge(byte[] left, byte[] right)
        {

            if (left == null || right == null || left.Length != right.Length || left.Length != 32)
            {
                throw new ArgumentException("Input sides can not be null or different size!");
            }

            byte[] retVal = new byte[BLOCK_SIZE];

            for (int i = 0; i < right.Length; i++)
            {
                retVal[i] = right[i];
                retVal[i + 32] = left[i];
            }

            return retVal;
        }

        /// <summary>
        /// Function we use to generate new key for each cycle.
        /// </summary>
        /// <param name="stringKey">String we use to generate new key</param>
        /// <param name="cycle">Number of cycle</param>
        /// <returns>New key</returns>
        static byte[] KeyGenerate(string stringKey, int cycle)
        {

            if (stringKey.Length != 8)
            {
                throw new ArgumentException("Key length must be 8 cgaracters!");
            }

            byte[] key = StringToByte(stringKey);
            key = ShortKey(key);
            key = ShiftKey(key, numberOfKeyShifts[cycle]);

            return PermuteKey(key);
        }

        /// <summary>
        /// Function we use to change length of key from 64 to 56.
        /// </summary>
        /// <param name="key">Key we want to change</param>
        /// <returns>56 bits key</returns>
        static byte[] ShortKey(byte[] key)
        {

            if (key.Length != 64)
            {
                throw new ArgumentException("Key length must be 64 bits!");

            }

            byte[] shortKey = new byte[56];
            int j = 0;
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 != 7)
                {
                    shortKey[j++] = key[i];
                }
            }

            return shortKey;
        }

        /// <summary>
        /// Function we use to shift key in every cycle.
        /// </summary>
        /// <param name="key">Key we shift</param>
        /// <param name="shifts">For how much places we shift key </param>
        /// <returns>Shifted key</returns>
        static byte[] ShiftKey(byte[] key, int shifts)
        {

            if (key.Length != 56 || shifts < 1)
            {
                throw new ArgumentException("Key length must be 56 bits!");
            }

            while ((shifts--) > 0)
            {
                byte first = key[0];
                for (int i = 0; i < 55; i++)
                {
                    key[i] = key[i + 1];

                }
                key[55] = first;
            }

            return key;
        }

        /// <summary>
        /// Function we use to permute byte array which we use like key.
        /// </summary>
        /// <param name="input">Key we want to permute</param>
        /// <returns>Permuted key</returns>
        static byte[] PermuteKey(byte[] input)
        {

            if (input == null)
            {
                throw new ArgumentException("Input can not be null!");
            }

            byte[] retVal = new byte[48];

            for (int i = 0, j = 0; i < input.Length; i++)
            {
                if (keyp[i] != -1)
                {
                    retVal[j++] = input[keyp[i] - 1];
                }
            }

            return retVal;
        }

        /// <summary>
        /// Get password from file
        /// </summary>
        /// <param name="fileName"> Name of file </param>
        /// <returns> Password from file </returns>
        public static string ReadKeyFromFile(string fileName)
        {
            try
            {
                StreamReader sr = new StreamReader(fileName);
                string psw = sr.ReadLine();
                sr.Close();

                if (psw == null || psw.Length != 8)
                {
                    throw new FormatException($"'{fileName}': Password length must be 8 characters!");
                }

                return psw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                return null;
            }
        }
    }
}
