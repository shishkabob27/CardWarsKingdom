using System;
using System.IO;

namespace Ionic.Zlib
{
	public class DeflateStream : Stream
	{
		internal ZlibBaseStream _baseStream;

		internal Stream _innerStream;

		private bool _disposed;

		public virtual FlushType FlushMode
		{
			get
			{
				return _baseStream._flushMode;
			}
			set
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("DeflateStream");
				}
				_baseStream._flushMode = value;
			}
		}

		public int BufferSize
		{
			get
			{
				return _baseStream._bufferSize;
			}
			set
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("DeflateStream");
				}
				if (_baseStream._workingBuffer != null)
				{
					throw new ZlibException("The working buffer is already set.");
				}
				if (value < 1024)
				{
					throw new ZlibException(string.Format("Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, 1024));
				}
				_baseStream._bufferSize = value;
			}
		}

		public CompressionStrategy Strategy
		{
			get
			{
				return _baseStream.Strategy;
			}
			set
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("DeflateStream");
				}
				_baseStream.Strategy = value;
			}
		}

		public virtual long TotalIn
		{
			get
			{
				return _baseStream._z.TotalBytesIn;
			}
		}

		public virtual long TotalOut
		{
			get
			{
				return _baseStream._z.TotalBytesOut;
			}
		}

		public override bool CanRead
		{
			get
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("DeflateStream");
				}
				return _baseStream._stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("DeflateStream");
				}
				return _baseStream._stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long Position
		{
			get
			{
				if (_baseStream._streamMode == ZlibBaseStream.StreamMode.Writer)
				{
					return _baseStream._z.TotalBytesOut;
				}
				if (_baseStream._streamMode == ZlibBaseStream.StreamMode.Reader)
				{
					return _baseStream._z.TotalBytesIn;
				}
				return 0L;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DeflateStream(Stream stream, CompressionMode mode)
			: this(stream, mode, CompressionLevel.Default, false)
		{
		}

		public DeflateStream(Stream stream, CompressionMode mode, CompressionLevel level)
			: this(stream, mode, level, false)
		{
		}

		public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
			: this(stream, mode, CompressionLevel.Default, leaveOpen)
		{
		}

		public DeflateStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
		{
			_innerStream = stream;
			_baseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.DEFLATE, leaveOpen);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!_disposed)
				{
					if (disposing && _baseStream != null)
					{
						_baseStream.Close();
					}
					_disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Flush()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("DeflateStream");
			}
			_baseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("DeflateStream");
			}
			return _baseStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("DeflateStream");
			}
			_baseStream.Write(buffer, offset, count);
		}

		public static byte[] CompressString(string s)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Stream compressor = new DeflateStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression);
				ZlibBaseStream.CompressString(s, compressor);
				return memoryStream.ToArray();
			}
		}

		public static byte[] CompressBuffer(byte[] b)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Stream compressor = new DeflateStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression);
				ZlibBaseStream.CompressBuffer(b, compressor);
				return memoryStream.ToArray();
			}
		}

		public static string UncompressString(byte[] compressed)
		{
			using (MemoryStream stream = new MemoryStream(compressed))
			{
				Stream decompressor = new DeflateStream(stream, CompressionMode.Decompress);
				return ZlibBaseStream.UncompressString(compressed, decompressor);
			}
		}

		public static byte[] UncompressBuffer(byte[] compressed)
		{
			using (MemoryStream stream = new MemoryStream(compressed))
			{
				Stream decompressor = new DeflateStream(stream, CompressionMode.Decompress);
				return ZlibBaseStream.UncompressBuffer(compressed, decompressor);
			}
		}
	}
}
