using System;
using System.IO;

namespace Ionic.Zlib
{
	public class CrcCalculatorStream : Stream, IDisposable
	{
		private static readonly long UnsetLengthLimit = -99L;

		internal Stream _innerStream;

		private CRC32 _Crc32;

		private long _lengthLimit = -99L;

		private bool _leaveOpen;

		public long TotalBytesSlurped
		{
			get
			{
				return _Crc32.TotalBytesRead;
			}
		}

		public int Crc
		{
			get
			{
				return _Crc32.Crc32Result;
			}
		}

		public bool LeaveOpen
		{
			get
			{
				return _leaveOpen;
			}
			set
			{
				_leaveOpen = value;
			}
		}

		public override bool CanRead
		{
			get
			{
				return _innerStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return _innerStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return _innerStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				if (_lengthLimit == UnsetLengthLimit)
				{
					return _innerStream.Length;
				}
				return _lengthLimit;
			}
		}

		public override long Position
		{
			get
			{
				return _Crc32.TotalBytesRead;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public CrcCalculatorStream(Stream stream)
			: this(true, UnsetLengthLimit, stream)
		{
		}

		public CrcCalculatorStream(Stream stream, bool leaveOpen)
			: this(leaveOpen, UnsetLengthLimit, stream)
		{
		}

		public CrcCalculatorStream(Stream stream, long length)
			: this(true, length, stream)
		{
			if (length < 0)
			{
				throw new ArgumentException("length");
			}
		}

		public CrcCalculatorStream(Stream stream, long length, bool leaveOpen)
			: this(leaveOpen, length, stream)
		{
			if (length < 0)
			{
				throw new ArgumentException("length");
			}
		}

		private CrcCalculatorStream(bool leaveOpen, long length, Stream stream)
		{
			_innerStream = stream;
			_Crc32 = new CRC32();
			_lengthLimit = length;
			_leaveOpen = leaveOpen;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int count2 = count;
			if (_lengthLimit != UnsetLengthLimit)
			{
				if (_Crc32.TotalBytesRead >= _lengthLimit)
				{
					return 0;
				}
				long num = _lengthLimit - _Crc32.TotalBytesRead;
				if (num < count)
				{
					count2 = (int)num;
				}
			}
			int num2 = _innerStream.Read(buffer, offset, count2);
			if (num2 > 0)
			{
				_Crc32.SlurpBlock(buffer, offset, num2);
			}
			return num2;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count > 0)
			{
				_Crc32.SlurpBlock(buffer, offset, count);
			}
			_innerStream.Write(buffer, offset, count);
		}

		public override void Flush()
		{
			_innerStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		public override void Close()
		{
			base.Close();
			if (!_leaveOpen)
			{
				_innerStream.Close();
			}
		}
	}
}
