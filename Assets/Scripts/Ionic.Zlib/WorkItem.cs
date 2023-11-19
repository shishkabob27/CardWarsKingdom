namespace Ionic.Zlib
{
	internal class WorkItem
	{
		internal enum Status
		{
			None,
			Filling,
			Filled,
			Compressing,
			Compressed,
			Writing,
			Done
		}

		public byte[] buffer;

		public byte[] compressed;

		public int status;

		public int crc;

		public int index;

		public int inputBytesAvailable;

		public int compressedBytesAvailable;

		public ZlibCodec compressor;

		public WorkItem(int size, CompressionLevel compressLevel, CompressionStrategy strategy)
		{
			buffer = new byte[size];
			int num = size + (size / 32768 + 1) * 5 * 2;
			compressed = new byte[num];
			status = 0;
			compressor = new ZlibCodec();
			compressor.InitializeDeflate(compressLevel, false);
			compressor.OutputBuffer = compressed;
			compressor.InputBuffer = buffer;
		}
	}
}
