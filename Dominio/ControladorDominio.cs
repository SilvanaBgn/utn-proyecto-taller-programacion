﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace Dominio
{
    public class ControladorDominio
    {
        private IUnitOfWork iUoW;


        #region Generales

        public ControladorDominio(IUnitOfWork pUoW)
        {
            this.iUoW = pUoW;
        }

        public void GuardarCambios()
        {
            this.iUoW.GuardarCambios();
        }

        public void CancelarCambios()
        {
            this.iUoW.Rollback();
        }
        #endregion

        #region Banner
        public void AgregarBanner(Banner pBanner)
        {
            this.iUoW.RepositorioBanners.Agregar(pBanner);
        }

        public void ModificarBanner(Banner pBanner)
        {
            this.iUoW.RepositorioBanners.Modificar(pBanner);
        }

        public void BorrarBanner(Banner pBanner)
        {
            this.iUoW.RepositorioBanners.Borrar(pBanner);
        }

        public void BorrarBanner(int pCodigo)
        {
            this.iUoW.RepositorioBanners.Borrar(pCodigo);
        }

        public Banner BuscarBannerPorId(int pId)
        {
            return this.iUoW.RepositorioBanners.ObtenerPorId(pId); 
        }

        public List<Banner> BuscarBannerPorAtributo(Expression<Func<Banner, bool>> filter = null)
        {
            return this.iUoW.RepositorioBanners.Obtener(filter,null).ToList();
        }

        public List<Banner> FiltrarBanners(DateTime[] pFiltroFechas, TimeSpan[] pFiltroHoras, string pFiltroTitulo, string pFiltroDescripcion)
        {
            DateTime fechaInicio;
            DateTime fechaFin;
            TimeSpan horaInicio;
            TimeSpan horaFin;

            Expression<Func<Banner, bool>> filtroFechas=null;
            Expression<Func<Banner, bool>> filtroHoras=null;
            Expression<Func<Banner, bool>> filtroTitulo = null;
            Expression<Func<Banner, bool>> filtroDescripcion=null;

            if (pFiltroFechas!=null)
            {
                fechaInicio = pFiltroFechas[0];
                fechaFin = pFiltroFechas[1];
                filtroFechas = x => x.FechaInicio.CompareTo(fechaInicio) >= 0 && x.FechaFin.CompareTo(fechaFin) <= 0;
            }

            if (pFiltroHoras!=null)
            {
                horaInicio = pFiltroHoras[0];
                horaFin = pFiltroHoras[1];
                filtroHoras = x => x.HoraInicio.CompareTo(horaInicio) >= 0 && x.HoraFin.CompareTo(horaFin) <= 0;
            }

            if (pFiltroTitulo!=null)
            {
                filtroTitulo = x => x.Titulo.Contains(pFiltroTitulo);
            }

            if (pFiltroDescripcion!=null)
            {
                filtroDescripcion = x => x.Descripcion.Contains(pFiltroDescripcion);
            }


            return this.iUoW.RepositorioBanners.Filtrar(filtroFechas, filtroHoras, filtroTitulo, filtroDescripcion).ToList();
        }



        public List<Banner> ObtenerTodosLosBanners()
        {
            return this.iUoW.RepositorioBanners.Obtener(null, null).ToList();
        }

        /// <summary>
        /// Busca cuál es el próximo banner a pasar en el siguiente cuarto de hora
        /// </summary>
        /// <returns>Devuelve el banner a pasar en el siguiente cuarto de hora, sino devuelve null</returns>
        private Banner ProximoBannerAPasar(DateTime pFechaActual)
        {
            TimeSpan horaActual = new TimeSpan(pFechaActual.Hour, pFechaActual.Minute, pFechaActual.Second);
            //Buscamos el próximo banner a pasar:
            // que fechaActual sea mayor o igual a FechaInicio y menor o igual a FechaFin --> FechaInicio<=fechaActual<=FechaFin
            //HoraInicio <= horaActual < HoraFin
            //Debería devolver un solo banner
            List<Banner> posiblesBanners = this.BuscarBannerPorAtributo
                 (x => x.FechaInicio.CompareTo(pFechaActual) <= 0 && x.FechaFin.CompareTo(pFechaActual) >= 0
                      && x.HoraInicio.CompareTo(horaActual) <= 0 && x.HoraFin.CompareTo(horaActual) > 0
                 );

            if (posiblesBanners.Count > 0) //Si encontró algún banner para el próximo cuarto de hora
                return posiblesBanners[0]; //Tomamos el primer banner que encontró
            else
                return null;
        }

        //private string InfoLeidaBanner(Banner pBanner)
        //{
        //    string texto = "";
        //    Fuente fuenteDelBanner = this.BuscarFuentePorId(pBanner.FuenteId);
        //    texto = pBanner.Descripcion;
        //    texto += ": ";
        //    IList<Item> listaItems = (List<Item>)fuenteDelBanner.Leer();
        //    //Asignamos su contenido a la variable texto:
        //    for (int i = 0; i < listaItems.Count; i++)
        //    {
        //        texto += listaItems[i].ToString() + " • | • ";
        //    }
        //    return texto;
        //}

        /// <summary>
        /// Calcula cuántos milisegundos faltan al próximo cuarto de hora
        /// </summary>
        /// <param name="pHoraActual">Hora de tipo TimeSpan a partir de la que se quiere calcular el próximo cuarto de hora</param>
        /// <returns>Devuelve un double que representa los milisegundos al próximo cuarto de hora</returns>
        private double IntervaloAlProxCuartoDeHora(TimeSpan pHoraActual)
        {
            int segundos = 60 - pHoraActual.Seconds;
            int minutos;
            int hora = pHoraActual.Hours;
            if (pHoraActual.Minutes >= 0 && pHoraActual.Minutes < 15)
                minutos = 15;
            else if (pHoraActual.Minutes >= 15 && pHoraActual.Minutes < 30)
                minutos = 30;
            else if (pHoraActual.Minutes >= 30 && pHoraActual.Minutes < 45)
                minutos = 45;
            else //(horaActual.Minutes >= 45 && horaActual.Minutes < 60)
            {
                minutos = 0;
                hora++;
            }
            return (new TimeSpan(hora, minutos, segundos)).Subtract(pHoraActual).TotalMilliseconds;
        }

        /// <summary>
        /// Averigua el texto y el intervalo de tiempo del próximo banner a pasar
        /// </summary>
        /// <returns>Devuelve un array, donde el primer argumento es el texto y el segundo es el intervalo
        /// Si no encuentra un banner para pasar en el próximo cuarto de hora, devuelve un texto vacío
        /// y el intervalo con la cantidad de milisegundos hasta el próximo</returns>
        public object[] LeerProximoBanner()
        {
            string texto = "";
            int intervalo = 0;
            object[] array = new object[2];

            DateTime fechaActual = DateTime.Now;
            TimeSpan horaActual = new TimeSpan(fechaActual.Hour, fechaActual.Minute, fechaActual.Second);
            
            //Obtenemos el próximo banner:
            Banner bannerAPasar = this.ProximoBannerAPasar(fechaActual);

            if (bannerAPasar != null)
            {
                //*texto* Lo leemos, de acuerdo a la fuente que corresponde al bannerAPasar:
   //             texto = this.InfoLeidaBanner(bannerAPasar);

                //*intervalo*:
                //Cambia el intervalo al tiempo del nuevo banner a pasar:
                intervalo = Convert.ToInt32(bannerAPasar.HoraFin.Subtract(horaActual).TotalMilliseconds);
            }
            else {
                // *texto es vacío.
                //*intervalo*:
                intervalo = Convert.ToInt32(this.IntervaloAlProxCuartoDeHora(horaActual));
            }
            array[0] = texto;
            array[1] = intervalo;
            return array;
        }
        #endregion

        #region Campania

        public void AgregarCampania(Campania pCampania)
        {
            this.iUoW.RepositorioCampanias.Agregar(pCampania);
        }

        public void ModificarCampania(Campania pCampania)
        {
            this.iUoW.RepositorioCampanias.Modificar(pCampania);
        }

        public void BorrarCampania(Campania pCampania)
        {
            this.iUoW.RepositorioCampanias.Borrar(pCampania);
        }

        public void BorrarCampania(int pCodigo)
        {
            this.iUoW.RepositorioCampanias.Borrar(pCodigo);
        }

        public List<Campania> ObtenerTodasLasCampanias()
        {
            return iUoW.RepositorioCampanias.Obtener(null,null).ToList(); 
        }

        public Campania BuscarCampaniaPorId(int pId)
        {
            return this.iUoW.RepositorioCampanias.ObtenerPorId(pId); 
        }

        public List<Campania> BuscarCampaniaPorAtributo(Expression<Func<Campania, bool>> filter = null)
        {
            return this.iUoW.RepositorioCampanias.Obtener(filter,null).ToList(); 
        }
        #endregion

        #region FuenteRss
        public void AgregarFuente(Fuente pFuente)
        {
            this.iUoW.RepositorioFuentes.Agregar(pFuente);
        }

        public void ModificarFuente(Fuente pFuente)
        {
            this.iUoW.RepositorioFuentes.Modificar(pFuente);
        }

        public void BorrarFuente(int pCodigo)
        {
            this.iUoW.RepositorioFuentes.Borrar(pCodigo);
        }

        public Fuente BuscarFuentePorId(int pId) 
        {
            return this.iUoW.RepositorioFuentes.ObtenerPorId(pId);
        }

        public List<Fuente> BuscarFuentePorAtributo(Expression<Func<Fuente, bool>> filter = null)
        {
            return this.iUoW.RepositorioFuentes.Obtener(filter,null).ToList();
        }

        public List<Fuente> ObtenerTodasLasFuentes()
        {
            return iUoW.RepositorioFuentes.Obtener().ToList();
        }
        #endregion
    }
}
