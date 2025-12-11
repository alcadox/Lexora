using System.Reflection;
using System.Runtime.Serialization; // <-- IMPORTANTE
using Lexora;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lexora.Tests
{
    [TestClass]
    public class MainFormTests
    {
        private string InvocarFormatearTamaño(long bytes)
        {
            // Instancia sin constructor
            var form = (MainForm)FormatterServices.GetUninitializedObject(typeof(MainForm));

            var metodo = typeof(MainForm).GetMethod(
                "FormatearTamaño",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            Assert.IsNotNull(metodo, "No se ha encontrado el método FormatearTamaño.");

            var resultado = metodo.Invoke(form, new object[] { bytes });

            Assert.IsInstanceOfType(resultado, typeof(string));

            return (string)resultado;
        }

        [TestMethod]
        public void FormatearTamaño_MenosDe1KB_DevuelveBytes()
        {
            long bytes = 500;

            string resultado = InvocarFormatearTamaño(bytes);

            Assert.AreEqual("500 B", resultado);
        }

        [TestMethod]
        public void FormatearTamaño_Entre1KBy1MB_DevuelveKB()
        {
            long bytes = 2048; // 2 KB

            string resultado = InvocarFormatearTamaño(bytes);

            Assert.AreEqual("2,0 KB", resultado);
        }

        [TestMethod]
        public void FormatearTamaño_Entre1MBy1GB_DevuelveMB()
        {
            long bytes = 5 * 1024 * 1024; // 5 MB

            string resultado = InvocarFormatearTamaño(bytes);

            Assert.AreEqual("5,0 MB", resultado);
        }

        [TestMethod]
        public void FormatearTamaño_MayorDe1GB_DevuelveGB()
        {
            long bytes = 3L * 1024 * 1024 * 1024; // 3 GB

            string resultado = InvocarFormatearTamaño(bytes);

            Assert.AreEqual("3,0 GB", resultado);
        }
    }
}
