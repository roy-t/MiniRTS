namespace ModelExtension
{
    public class MaterialDescription
    {
        public MaterialDescription(string diffuse, string normal, string specular, string mask, string reflection)
        {
            this.Diffuse = diffuse;
            this.Normal = normal;
            this.Specular = specular;
            this.Mask = mask;
            this.Reflection = reflection;
        }

        public string Diffuse { get; }
        public string Normal { get; }
        public string Specular { get; }
        public string Mask { get; }
        public string Reflection { get; }
    }
}
