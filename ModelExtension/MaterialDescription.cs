namespace ModelExtension
{
    public class MaterialDescription
    {
        public MaterialDescription(string diffuse, string normal, string specular, string mask)
        {
            this.Diffuse  = diffuse;
            this.Normal   = normal;
            this.Specular = specular;
            this.Mask     = mask;
        }

        public string Diffuse { get; }
        public string Normal { get; }
        public string Specular { get; }
        public string Mask { get; }
    }
}
