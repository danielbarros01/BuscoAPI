using BuscoAPI.Entities;

namespace BuscoAPI.Helpers
{
    public static class DataSeed
    {
        //Crear categorias
        public static readonly ProfessionCategory[] Categories = new[] {
                new ProfessionCategory { Id = 1, Name = "Construcción y edificación"},
                new ProfessionCategory { Id = 2, Name = "Mantenimiento y reparación"},
                new ProfessionCategory { Id = 3, Name = "Fabricación y producción"},
                new ProfessionCategory { Id = 4, Name = "Instalaciones"},
                new ProfessionCategory { Id = 5, Name = "Limpieza"},
                new ProfessionCategory { Id = 6, Name = "Tecnología"}
                //new ProfessionCategory { Id = 7, Name = "Decoracion y acabados"},

            };

        //Crear profesiones
        public static readonly Profession[] Professions = new[] {
                //Construcción y edificación
                new Profession { Id = 1, Name = "Albañil", Description = "Trabaja con ladrillos, cemento, bloques y materiales de construcción para levantar estructuras.", CategoryId = 1 },
                new Profession { Id = 2, Name = "Carpintero", Description = "Realiza trabajos en madera, como puertas, ventanas, muebles y estructuras de soporte.", CategoryId = 1 },
                new Profession { Id = 3, Name = "Pintor", Description = "Pinta paredes, techos, y otras superficies, tanto en interiores como en exteriores.", CategoryId = 1 },
                new Profession { Id = 4, Name = "Plomero", Description = "Instala y repara sistemas de tuberías, desagües y otros sistemas de agua.", CategoryId = 1 },
                new Profession { Id = 5, Name = "Techador", Description = "Instala y repara techos en edificios y casas.", CategoryId = 1 },
              
                //Mantenimiento y reparación
                new Profession { Id = 6, Name = "Técnico de Electrodomésticos", Description = "Repara y mantiene electrodomésticos como lavadoras, refrigeradores, y hornos.", CategoryId = 2 },
                new Profession { Id = 7, Name = "Mecánico Automotriz", Description = "Repara y da mantenimiento a vehículos automotores.", CategoryId = 2 },
                new Profession { Id = 8, Name = "Técnico de Calefacción, Ventilación y Aire Acondicionado (HVAC)", Description = "Instala y repara sistemas de climatización.", CategoryId = 2 },
                new Profession { Id = 9, Name = "Jardinero", Description = "Mantiene jardines, céspedes y paisajismo.", CategoryId = 2 },
                new Profession { Id = 10, Name = "Cerrajero", Description = "Trabaja con cerraduras y sistemas de seguridad para puertas y ventanas.", CategoryId = 2 },

                //Fabricación y producción
                new Profession { Id = 11, Name = "Herrero", Description = "Trabaja con metal, fabricando y reparando estructuras y componentes metálicos.", CategoryId = 3 },
                new Profession { Id = 12, Name = "Soldador", Description = "Une piezas metálicas utilizando técnicas de soldadura.", CategoryId = 3 },
                new Profession { Id = 13, Name = "Operador de Maquinaria Pesada", Description = "Maneja maquinaria pesada utilizada", CategoryId = 3 },
                new Profession { Id = 14, Name = "Montador de Estructuras Metálicas", Description = "Ensambla y erige estructuras metálicas.", CategoryId = 3 },
                
                //Instalaciones
                new Profession { Id = 15, Name = "Electricista", Description = "Instala y repara sistemas eléctricos en edificios y estructuras.", CategoryId = 4 },
                new Profession { Id = 16, Name = "Instalador de Paneles Solares", Description = "Monta sistemas de energía solar en tejados y otras estructuras.", CategoryId = 4 },
                
                //Limpieza
                new Profession { Id = 17, Name = "Limpiador de Edificios", Description = "Mantiene limpias oficinas, casas, y otras instalaciones.", CategoryId = 5 },

                //Tecnologia
                new Profession { Id = 18, Name = "Técnico Informático", Description = "Proporciona asistencia técnica y resuelve problemas relacionados con hardware y software para usuarios finales.", CategoryId = 6 },
                new Profession { Id = 19, Name = "Ingeniero de Redes", Description = "Diseña, implementa y mantiene redes de comunicación de datos, garantizando su rendimiento y seguridad.", CategoryId = 6 }
            };
    }
}
