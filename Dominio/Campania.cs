﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Dominio
{
    public class Campania
    {
        public int CampaniaId { get; set; }

        /// <summary>
        /// Título referente que se desea mostrar en la pantalla publicitaria
        /// </summary>
        [Required]
        public string Titulo { get; set; }

        /// <summary>
        /// Explicación del contenido de la campaña
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Duración en segundos de cada imagen de la campaña
        /// </summary>
        [Range(1,999)]
        public int DuracionImagen { get; set; }

        public virtual ICollection<Imagen> Imagenes { get; set; }

        [DataType(DataType.Date), Required]
        public DateTime FechaInicio { get; set; }
        [DataType(DataType.Date), Required]
        public DateTime FechaFin { get; set; }

        [Timestamp, Required]
        public TimeSpan HoraInicio { get; set; }
        [Timestamp, Required]
        public TimeSpan HoraFin { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Campania()
        {
            this.Imagenes = new List<Imagen>();
        }
    }
}
