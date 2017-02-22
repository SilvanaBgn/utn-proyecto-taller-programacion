﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using System.Text.RegularExpressions;

namespace UI.NuevasPantallas
{
    public partial class VAbstractCrearModificarFuente : Form
    {
        protected ControladorDominio iControladorDominio;

        //CONSTRUCTOR
        public VAbstractCrearModificarFuente()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public VAbstractCrearModificarFuente(ref ControladorDominio pControladorDominio) : this()
        {
            this.iControladorDominio = pControladorDominio;

            //Centramos la pantalla en el centro:
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Si se selecciona RSS en el comboBox se muestra el panel RSS
        /// Si se selecciona Texto Fijo en el comboBox se muestra el panel texto fijo
        /// </summary>
        private void MostrarPanel(object sender, EventArgs e)
        {
            switch (this.comboBoxTipoFuente.SelectedItem.ToString())
            {
                case "Rss":
                    this.panelTextoFijo.Visible = false;
                    this.panelRss.Visible = true;
                    break;
                case "Texto Fijo":
                    this.panelRss.Visible = false;
                    this.panelTextoFijo.Visible = true;
                    break;
            }
        }

        private void InputValido(KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\s\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void textBoxValido_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.InputValido(e);
        }

    }
}