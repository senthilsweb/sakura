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
    class EncriptWrapper : IEncriptWrapper
    {
        //TODO: Improve size detection of key in algorithms.
        const int KEY_SIZE = 8;

        public EncriptWrapper()
        {
            encriptStrategy = new EncriptStrategyDef();
            keyGenerator = new KeyGeneratorDef();
        }

        public EncriptWrapper(IEncriptStrategy encriptStrategy, IKeyGenerator keyGenerator)
        {
            this.encriptStrategy = encriptStrategy;
            this.keyGenerator = keyGenerator;
        }

        /// <summary>
        /// Decrypt a crypted string.
        /// </summary>
        /// <param name="cryptedString">The crypted string.</param>
        /// <returns>The decrypted string.</returns>
        public override string Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString) && cryptedString.Length <= KEY_SIZE)
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }

            string key = GetKeyFromCryptedString(cryptedString);
            return encriptStrategy.Decrypt(key, cryptedString.Substring(KEY_SIZE, cryptedString.Length - KEY_SIZE));
        }

        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="originalString">The original string.</param>
        /// <returns>The encrypted string.</returns>
        public override string Encrypt(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                    ("The string which needs to be encrypted can not be null.");
            }

            string key = keyGenerator.GenerateKey(KEY_SIZE);
            return key + encriptStrategy.Encrypt(key, originalString);
        }

        /// <summary>
        /// Decrypt CryptedString and Encrypt by other Encrypt Method.
        /// </summary>
        /// <param name="cryptedString">The encrypted string.</param>
        /// <param name="oldEncriptor">The encryptor that encrypted</param>
        /// <param name="newEncriptor">The encryptor that will encrypt.</param>
        /// <returns>The ReEncrypted string.</returns>
        public override string ReEncrypt(string cryptedString, IEncriptStrategy oldEncriptor, IEncriptStrategy newEncriptor)
        {
            if (String.IsNullOrEmpty(cryptedString) && cryptedString.Length <= KEY_SIZE)
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }

            string key = GetKeyFromCryptedString(cryptedString);
            return newEncriptor.Encrypt(keyGenerator.GenerateKey(KEY_SIZE), oldEncriptor.Decrypt(key, cryptedString.Substring(KEY_SIZE, cryptedString.Length - KEY_SIZE)));
        }

        private string GetKeyFromCryptedString(string cryptedString)
        {
            return cryptedString.Substring(0, KEY_SIZE);
        }
    }
}
