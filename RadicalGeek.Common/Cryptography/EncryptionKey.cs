namespace RadicalGeek.Common.Cryptography
{
    public class EncryptionKey
    {
        public string PassPhrase { get; set; }
        public string SaltValue { get; set; }
        public string InitVector { get; set; }
        public EncryptionKeySet Identity { get; set; }
    }
}