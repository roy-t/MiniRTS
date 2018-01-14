namespace ModelExtension
{
    public class MaterialDescription
    {
        public MaterialDescription(string diffuse, string normal, string specular)
        {
            this.Diffuse  = diffuse;
            this.Normal   = normal;
            this.Specular = specular;
        }

        public string Diffuse { get; }
        public string Normal { get; }
        public string Specular { get; }
    }
}
