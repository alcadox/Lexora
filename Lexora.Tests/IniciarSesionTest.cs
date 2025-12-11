using System;
using System.Reflection;
using System.Runtime.Serialization; // <-- IMPORTANTE
using Lexora;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lexora.Tests
{
    [TestClass]
    public class InicioSesionTests
    {
        private string InvocarCalcularHashSHA256(string texto)
        {
            // Creamos una instancia SIN llamar al constructor ni a InitializeComponent
            var form = (InicioSesion)FormatterServices.GetUninitializedObject(typeof(InicioSesion));

            var metodo = typeof(InicioSesion).GetMethod(
                "CalcularHashSHA256",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            Assert.IsNotNull(metodo, "No se ha encontrado el método CalcularHashSHA256.");

            var resultado = metodo.Invoke(form, new object[] { texto });

            // Por seguridad, comprobamos que es string
            Assert.IsInstanceOfType(resultado, typeof(string));

            return (string)resultado;
        }

        [TestMethod]
        public void CalcularHashSHA256_MismoTexto_DevuelveMismoHash()
        {
            string texto = "contraSegura123";

            string hash1 = InvocarCalcularHashSHA256(texto);
            string hash2 = InvocarCalcularHashSHA256(texto);

            Assert.AreEqual(hash1, hash2);
            Assert.AreEqual(64, hash1.Length);
        }

        [TestMethod]
        public void CalcularHashSHA256_TextosDistintos_DevuelvenHashDistinto()
        {
            string hash1 = InvocarCalcularHashSHA256("password1");
            string hash2 = InvocarCalcularHashSHA256("password2");

            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void CalcularHashSHA256_CadenaVacia_NoEsNullNiVacia()
        {
            string hash = InvocarCalcularHashSHA256(string.Empty);

            Assert.IsFalse(string.IsNullOrEmpty(hash));
            Assert.AreEqual(64, hash.Length);
        }
    }
}

