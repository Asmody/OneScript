﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptEngine.HostedScript.Library.Binary
{
    class StreamWithTimeout : Stream
    {
        private readonly Stream _underlyingStream;
        private int _readTimeout;

        public override bool CanRead => _underlyingStream.CanRead;

        public override bool CanSeek => _underlyingStream.CanSeek;

        public override bool CanWrite => _underlyingStream.CanWrite;

        public override bool CanTimeout => true;

        public override long Length => _underlyingStream.Length;

        public override long Position
        {
            get
            {
                return _underlyingStream.Position;
            }
            set
            {
                _underlyingStream.Position = value;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return _readTimeout;
            }
            set
            {
                _readTimeout = value;
                if (_underlyingStream.CanTimeout)
                    _underlyingStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return _underlyingStream.WriteTimeout;
            }
            set
            {
                _underlyingStream.WriteTimeout = value;
            }
        }

        public override void Flush()
        {
            _underlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_readTimeout > 0 && !_underlyingStream.CanTimeout)
            {
                int read = -1;

                AutoResetEvent gotInput = new AutoResetEvent(false);
                Thread inputThread = new Thread(() =>
                {
                    try
                    {
                        read = _underlyingStream.Read(buffer, offset, count);
                        gotInput.Set();
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                    }
                })
                {
                    IsBackground = true
                };

                inputThread.Start();

                // Timeout expired?
                if (!gotInput.WaitOne(_readTimeout))
                {
                    inputThread.Abort();
                }

                return read;
            }
            else
                return _underlyingStream.Read(buffer, offset, count);
        }

        public new void CopyTo(Stream destination, int bufferSize = 0)
        {
            if (_readTimeout > 0 && !_underlyingStream.CanTimeout)
            {
                AutoResetEvent gotInput = new AutoResetEvent(false);
                Thread inputThread = new Thread(() =>
                {
                    try
                    {
                        if (bufferSize == 0)
                            _underlyingStream.CopyTo(destination);
                        else
                            _underlyingStream.CopyTo(destination, bufferSize);
                        gotInput.Set();
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                    }
                })
                {
                    IsBackground = true
                };

                inputThread.Start();

                // Timeout expired?
                if (!gotInput.WaitOne(_readTimeout))
                {
                    inputThread.Abort();
                }
            }
            else
                if (bufferSize == 0)
                    _underlyingStream.CopyTo(destination);
                else
                    _underlyingStream.CopyTo(destination, bufferSize);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _underlyingStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _underlyingStream.Write(buffer, offset, count);
        }

        public StreamWithTimeout(Stream underlyingStream)
        {
            _underlyingStream = underlyingStream;
        }
    }
}
