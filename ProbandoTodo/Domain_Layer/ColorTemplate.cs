using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer
{
    public class ColorTemplate
    {
        public class ColorData
        {
            public string Color { get; set; }
            public string Name { get; set; }
        }

        public class AvailableColors
        {
            public List<ColorData> ColorList { get; set; }

            public AvailableColors()
            {
                this.ColorList = new List<ColorData>();

                ColorList.Add(new ColorData() { Color = "black", Name = "Negro" });
                ColorList.Add(new ColorData() { Color = "blue", Name = "Azul" });
                ColorList.Add(new ColorData() { Color = "blueviolet", Name = "Violeta" });
                ColorList.Add(new ColorData() { Color = "brown", Name = "Marrón" });
                ColorList.Add(new ColorData() { Color = "coral", Name = "Naranja" });
                ColorList.Add(new ColorData() { Color = "gold", Name = "Amarillo" });
                ColorList.Add(new ColorData() { Color = "crimson", Name = "Rojo" });
                ColorList.Add(new ColorData() { Color = "darkgoldenrod", Name = "Marrón claro" });
                ColorList.Add(new ColorData() { Color = "darkturquoise", Name = "Turquesa" });
                ColorList.Add(new ColorData() { Color = "deeppink", Name = "Rosa" });
                ColorList.Add(new ColorData() { Color = "deepskyblue", Name = "Celeste" });
                ColorList.Add(new ColorData() { Color = "fuchsia", Name = "Fucsia" });
                ColorList.Add(new ColorData() { Color = "hotpink", Name = "Rosa claro" });
                ColorList.Add(new ColorData() { Color = "lightskyblue", Name = "Crema" });
                ColorList.Add(new ColorData() { Color = "limegreen", Name = "Verde" });
                ColorList.Add(new ColorData() { Color = "seagreen", Name = "Verde oscuro" });
            }
        }        
    }    
}
