using System;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.Cryptography
{
    internal class CryptoBytes : IEquatable<CryptoBytes>
    {
        public byte[] Vector;
        public byte[] Password;

        public CryptoBytes(byte[] vectorBytes, byte[] passwordBytes)
        {
            Vector = vectorBytes;
            Password = passwordBytes;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CryptoBytes other)
        {
            return other != null && other.Password.ContainsSameItemsAs(Password) &&
                   other.Vector.ContainsSameItemsAs(Vector);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public new bool Equals(object other)
        {
            return (other is CryptoBytes) && Equals((CryptoBytes)other);
        }
    }
}