//The MIT License (MIT)

//Copyright (c) 2016 Senthilnathan Karuppaiah

// <author> </author>
// <date> </date>
// <summary> </summary>

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
using System;

namespace BASE.COMMON
{
    abstract class IEncriptWrapper
    {
        protected IEncriptStrategy encriptStrategy;
        protected IKeyGenerator keyGenerator;

        internal IEncriptStrategy EncriptStrategy
        {
            set
            {
                encriptStrategy = value;
            }
        }
        internal IKeyGenerator KeyGenerator
        {
            set
            {
                keyGenerator = value;
            }
        }

        public abstract string Decrypt(string cryptedString);
        public abstract string Encrypt(string originalString);

        public abstract string ReEncrypt(string cryptedString, IEncriptStrategy oldEncriptor, IEncriptStrategy newEncriptor);
    }
}
