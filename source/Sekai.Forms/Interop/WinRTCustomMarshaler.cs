#nullable disable
#pragma warning disable IDE0001
#pragma warning disable IDE0003
#pragma warning disable IDE0044
#pragma warning disable IDE0073
#pragma warning disable IDE0130
#pragma warning disable IDE0161

//The MIT License (MIT)

//Copyright(c) Microsoft Corporation

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

//Remove when closed: https://github.com/microsoft/CsWin32/issues/397

namespace Windows.Win32.CsWin32.InteropServices

{
    internal class WinRTCustomMarshaler : global::System.Runtime.InteropServices.ICustomMarshaler
    {
        private string winrtClassName;
        private bool lookedForFromAbi;
        private global::System.Reflection.MethodInfo fromAbi;

        private WinRTCustomMarshaler(string cookie)
        {
            this.winrtClassName = cookie;
        }

        /// <summary>
        /// Gets an instance of the marshaler given a cookie
        /// </summary>
        /// <param name="cookie">Cookie used to create marshaler</param>
        /// <returns>Marshaler</returns>
        public static global::System.Runtime.InteropServices.ICustomMarshaler GetInstance(string cookie)
        {
            return new WinRTCustomMarshaler(cookie);
        }

        void global::System.Runtime.InteropServices.ICustomMarshaler.CleanUpManagedData(object ManagedObj)
        {
        }

        void global::System.Runtime.InteropServices.ICustomMarshaler.CleanUpNativeData(global::System.IntPtr pNativeData)
        {
            global::System.Runtime.InteropServices.Marshal.Release(pNativeData);
        }

        int global::System.Runtime.InteropServices.ICustomMarshaler.GetNativeDataSize()
        {
            throw new global::System.NotImplementedException();
        }

        global::System.IntPtr global::System.Runtime.InteropServices.ICustomMarshaler.MarshalManagedToNative(object ManagedObj)
        {
            throw new global::System.NotImplementedException();
        }

        object global::System.Runtime.InteropServices.ICustomMarshaler.MarshalNativeToManaged(global::System.IntPtr pNativeData)
        {
            if (!this.lookedForFromAbi)
            {
                var assembly = typeof(global::Windows.Foundation.IMemoryBuffer).Assembly;
                var type = global::System.Type.GetType($"{this.winrtClassName}, {assembly.FullName}");

                this.fromAbi = type.GetMethod("FromAbi");
                this.lookedForFromAbi = true;
            }

            if (this.fromAbi != null)
            {
                return this.fromAbi.Invoke(null, new object[] { pNativeData });
            }
            else
            {
                return global::System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(pNativeData);
            }
        }
    }
}
