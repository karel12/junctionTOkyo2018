using System;

namespace Assets.Utils
{
    public static class ByteParser
    {
        /// <summary>
        /// Slices a chunk from the given input array.
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <param name="numberOfBytes">The chunk size to slice off.</param>
		public static byte[] Slice(this byte[] inputArray, int numberOfBytes)
		{
			return Slice (inputArray, numberOfBytes, null);
		}

        /// <summary>
        /// Slices a chunk from the given input array.
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <param name="numberOfBytes">The chunk size to slice off.</param>
        /// <param name="outputAction">Retreives the chunk that was sliced off.</param>
        public static byte[] Slice(this byte[] inputArray, int numberOfBytes, Action<byte[]> outputAction)
        {
            if (inputArray == null)
            {
                return new byte[0];
            }

            var restLength = Math.Max(inputArray.Length - numberOfBytes, 0);
            var sliceLength = Math.Min(numberOfBytes, inputArray.Length);
                
            var rest = new byte[restLength];
            var slice = new byte[sliceLength];

            if (slice.Length > 0)
            {
                Array.Copy(inputArray, 0, slice, 0, slice.Length);
            }

            if (rest.Length > 0)
            {
                Array.Copy(inputArray, numberOfBytes, rest, 0, rest.Length);
            }
            
            if (outputAction != null && slice.Length > 0)
            {
                outputAction(slice);
            }
            return rest;
        }

        /// <summary>
        /// Tries to convert the given byte array into an Int32 value.
        /// </summary>
        public static Int32 TryGetInt32(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return 0;
            }
            try
            {
                return BitConverter.ToInt32(byteArray, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Tries to convert the given byte array into an Int32 value.
        /// </summary>
        public static Int16 TryGetInt16(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return 0;
            }
            try
            {
                return BitConverter.ToInt16(byteArray, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }


        /// <summary>
        /// Tries to convert the given byte array into an Int32 value.
        /// </summary>
        public static UInt32 TryGetUInt32(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return 0;
            }
            try
            {
                return BitConverter.ToUInt32(byteArray, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Tries to convert the given byte array into an UInt16 value.
        /// </summary>
        public static UInt16 TryGetUInt16(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return 0;
            }
            try
            {
                return BitConverter.ToUInt16(byteArray, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Tries to convert the given byte array into an single byte value.
        /// </summary>
        public static byte TryGetByte(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return 0;
            }
            return byteArray[0];
        }

        /// <summary>
        /// Tries to convert the given byte array into an string value.
        /// </summary>
        public static String TryGetString(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return string.Empty;
            }
            return BitConverter.ToString(byteArray);
        }

		/// <summary>
		/// Tries to get the firmware version.
		/// </summary>
		public static String TryGetVersionString(this byte[] byteArray)
		{
			if (byteArray.Length < 4)
			{
				return string.Empty;
			}
			return string.Format("{0}.{1}.{2}.{3}", byteArray[0], byteArray[1], byteArray[2], byteArray[3]);
		}
    }
}
