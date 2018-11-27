namespace NoHUDPlus.Util
{
    class Icon
    {
        public static byte[] GetResource(string ResourceName)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
    }
}
