﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Tricycle.Diagnostics
{
    /// <summary>
    /// Wraps <see cref="Process"/> so that it implements <see cref="IProcess"/>.
    /// </summary>
    /// <seealso cref="IProcess"/>
    public class ProcessWrapper : IProcess
    {
        readonly Process _process;

        public int ExitCode => _process.ExitCode;
        public bool HasExited => _process.HasExited;

        public event Action Exited;
        public event Action<string> ErrorDataReceived;
        public event Action<string> OutputDataReceived;

        public ProcessWrapper()
        {
            _process = new Process()
            {
                EnableRaisingEvents = true
            };

            _process.Exited += (sender, e) => Exited?.Invoke();
            _process.ErrorDataReceived += (sender, e) => ErrorDataReceived?.Invoke(e?.Data);
            _process.OutputDataReceived += (sender, e) => OutputDataReceived?.Invoke(e?.Data);
        }

        public void Dispose()
        {
            _process.Dispose();
        }

        public void Kill()
        {
            try
            {
                _process.Kill();
            }
            catch (NotSupportedException ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("An error occurred killing the process.", ex);
            }
            catch (Win32Exception ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("An error occurred killing the process.", ex);
            }
        }

        public bool Start(ProcessStartInfo startInfo)
        {
            _process.StartInfo = startInfo;

            try
            {
                bool result = _process.Start();

                if (!startInfo.UseShellExecute)
                {
                    if (startInfo.RedirectStandardOutput)
                    {
                        _process.BeginOutputReadLine();
                    }

                    if (startInfo.RedirectStandardError)
                    {
                        _process.BeginErrorReadLine();
                    }
                }

                return result;
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("An error occurred starting the process.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                throw new ArgumentException($"{nameof(startInfo)} is invalid.", ex);
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("An error occurred starting the process.", ex);
            }
            catch (Win32Exception ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("An error occurred starting the process.", ex);
            }
        }

        public void WaitForExit()
        {
            WaitForExit(-1);
        }

        public bool WaitForExit(int milliseconds)
        {
            try
            {
                if (milliseconds > 0)
                {
                    return _process.WaitForExit(milliseconds);
                }

                _process.WaitForExit();
                return true;
            }
            catch (Win32Exception ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("An error occurred waiting for the process.", ex);
            }
            catch (SystemException ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("An error occurred waiting for the process.", ex);
            }
        }
    }
}
