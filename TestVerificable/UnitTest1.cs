using verificable.Controllers;
using verificable.Models;

namespace TestVerificable
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestTotalRightPercentage()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            List<Adquirente> compraventaAdquirentes = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 1, RunRut = "2-7", PorcentajeDerecho = 20, NoAcreditado = false },
                new Adquirente { Id = 2, NumAtencion = 2, RunRut = "3-5", PorcentajeDerecho = 30, NoAcreditado = false },
            };
            double? expectedOutput = 50;
            double? actualOutput = controller.TotalRightPercentage(compraventaAdquirentes);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void TestOneAdquirenteAndEnajenanteCaseWithMultipropietarios()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            List<Adquirente> adquirenteCandidates = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 1, RunRut = "3-5", PorcentajeDerecho = 25, NoAcreditado = false }
            };
            List<Enajenante> enajenanteCandidates = new List<Enajenante>
            {
                new Enajenante { Id = 13, NumAtencion = 3, RunRut = "125-2", PorcentajeDerecho = 25, NoAcreditado = false }
            };
            List<Multipropietario> multipropietarios = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 50},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "3-5", PorcentajeDerecho = 50}
            };
            Formulario formulario = new Formulario { NumAtencion = 1, Cne = "Compraventa", Comuna = "Ancud", Manzana = "1", Predio = "1", Fojas = 99, NumInscripcion = 22, FechaInscripcion = new DateTime(2018, 6, 30, 0, 0, 0) };
            List<Multipropietario> actualOutput = controller.OneAdquirenteAndEnajenanteCase(adquirenteCandidates, enajenanteCandidates, multipropietarios, formulario);
            List<Multipropietario> expectedOutput = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 50},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "3-5", PorcentajeDerecho = 50},
                new Multipropietario { Id = 3, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "125-2", PorcentajeDerecho = 0}
            };
            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].Comuna, actualOutput[item].Comuna);
                Assert.AreEqual(expectedOutput[item].Manzana, actualOutput[item].Manzana);
                Assert.AreEqual(expectedOutput[item].Predio, actualOutput[item].Predio);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
            }

        }
        [Test]
        public void TestOneAdquirenteAndEnajenanteCaseWithoutMultipropietarios()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            List<Adquirente> adquirenteCandidates = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 1, RunRut = "2-7", PorcentajeDerecho = 20, NoAcreditado = false }
            };
            List<Enajenante> enajenanteCandidates = new List<Enajenante>
            {
                new Enajenante { Id = 13, NumAtencion = 3, RunRut = "1-9", PorcentajeDerecho = 20, NoAcreditado = false }
            };
            List<Multipropietario> multipropietarios = new List<Multipropietario>();
            Formulario formulario = new Formulario { NumAtencion = 1, Cne = "Compraventa", Comuna = "Ancud", Manzana = "1", Predio = "1", Fojas = 99, NumInscripcion = 22, FechaInscripcion = new DateTime(2018, 6, 30, 0, 0, 0) };
            List<Multipropietario> actualOutput = controller.OneAdquirenteAndEnajenanteCase(adquirenteCandidates, enajenanteCandidates, multipropietarios, formulario);
            List<Multipropietario> expectedOutput = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 80},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 20},
            };
            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].Comuna, actualOutput[item].Comuna);
                Assert.AreEqual(expectedOutput[item].Manzana, actualOutput[item].Manzana);
                Assert.AreEqual(expectedOutput[item].Predio, actualOutput[item].Predio);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
            }
        }

        [Test]
        public void TestTotalTransferCase()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            Formulario formulario = new Formulario { NumAtencion = 1, Cne = "Compraventa", Comuna = "Ancud", Manzana = "1", Predio = "1", Fojas = 99, NumInscripcion = 22, FechaInscripcion = new DateTime(2018, 6, 30, 0, 0, 0) };
            List<Enajenante> enajenanteCandidates = new List<Enajenante>
            {
                new Enajenante { Id = 1, NumAtencion = 3, RunRut = "2-7", PorcentajeDerecho = 0, NoAcreditado = false },
                new Enajenante { Id = 2, NumAtencion = 3, RunRut = "3-5", PorcentajeDerecho = 0, NoAcreditado = false }
            };
            List<Adquirente> adquirenteCandidates = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 3, RunRut = "125-2", PorcentajeDerecho = 10, NoAcreditado = false },
                new Adquirente { Id = 2, NumAtencion = 3, RunRut = "132-5", PorcentajeDerecho = 20, NoAcreditado = false },
                new Adquirente { Id = 3, NumAtencion = 3, RunRut = "144-9", PorcentajeDerecho = 70, NoAcreditado = false }
            };
            List<Multipropietario> multipropietarios = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 10},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "3-5", PorcentajeDerecho = 20},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 30},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "6-K", PorcentajeDerecho = 40}
            };
            List<Multipropietario> actualOutput = controller.TotalTransferCase(formulario, enajenanteCandidates, adquirenteCandidates, multipropietarios);
            List<Multipropietario> expectedOutput = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "125-2", PorcentajeDerecho = 3},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "132-5", PorcentajeDerecho = 6},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "144-9", PorcentajeDerecho = 21},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 30},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "6-K", PorcentajeDerecho = 40},
            };
            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].Comuna, actualOutput[item].Comuna);
                Assert.AreEqual(expectedOutput[item].Manzana, actualOutput[item].Manzana);
                Assert.AreEqual(expectedOutput[item].Predio, actualOutput[item].Predio);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
            }

        }

        [Test]
        public void TestCasesToTransferOneAndOne()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            bool oneEnajenanteAndAdquirente = true;
            List<Adquirente> adquirenteCandidates = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 1, RunRut = "2-7", PorcentajeDerecho = 20, NoAcreditado = false }
            };
            List<Enajenante> enajenanteCandidates = new List<Enajenante>
            {
                new Enajenante { Id = 13, NumAtencion = 3, RunRut = "1-9", PorcentajeDerecho = 20, NoAcreditado = false }
            };
            List<Multipropietario> multipropietarios = new List<Multipropietario>();
            List<Multipropietario> multipropietariosToAdd = new List<Multipropietario>();
            Formulario formulario = new Formulario { NumAtencion = 1, Cne = "Compraventa", Comuna = "Ancud", Manzana = "1", Predio = "1", Fojas = 99, NumInscripcion = 22, FechaInscripcion = new DateTime(2018, 6, 30, 0, 0, 0) };
            List<Multipropietario> actualOutput = controller.CasesToTransfer(adquirenteCandidates, enajenanteCandidates, oneEnajenanteAndAdquirente, formulario, multipropietarios, multipropietariosToAdd);
            List<Multipropietario> expectedOutput = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 80},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 20},
            };
            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].Comuna, actualOutput[item].Comuna);
                Assert.AreEqual(expectedOutput[item].Manzana, actualOutput[item].Manzana);
                Assert.AreEqual(expectedOutput[item].Predio, actualOutput[item].Predio);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
            }
        }

        [Test]
        public void TestCasesToTransferTotalRight()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            bool oneEnajenanteAndAdquirente = false;
            List<Multipropietario> multipropietariosToAdd = new List<Multipropietario>();
            Formulario formulario = new Formulario { NumAtencion = 1, Cne = "Compraventa", Comuna = "Ancud", Manzana = "1", Predio = "1", Fojas = 99, NumInscripcion = 22, FechaInscripcion = new DateTime(2018, 6, 30, 0, 0, 0) };
            List<Enajenante> enajenanteCandidates = new List<Enajenante>
            {
                new Enajenante { Id = 1, NumAtencion = 3, RunRut = "2-7", PorcentajeDerecho = 0, NoAcreditado = false },
                new Enajenante { Id = 2, NumAtencion = 3, RunRut = "3-5", PorcentajeDerecho = 0, NoAcreditado = false }
            };
            List<Adquirente> adquirenteCandidates = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 3, RunRut = "125-2", PorcentajeDerecho = 10, NoAcreditado = false },
                new Adquirente { Id = 2, NumAtencion = 3, RunRut = "132-5", PorcentajeDerecho = 20, NoAcreditado = false },
                new Adquirente { Id = 3, NumAtencion = 3, RunRut = "144-9", PorcentajeDerecho = 70, NoAcreditado = false }
            };
            List<Multipropietario> multipropietarios = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 10},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "3-5", PorcentajeDerecho = 20},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 30},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "6-K", PorcentajeDerecho = 40}
            };
            List<Multipropietario> actualOutput = controller.CasesToTransfer(adquirenteCandidates, enajenanteCandidates, oneEnajenanteAndAdquirente, formulario, multipropietarios, multipropietariosToAdd);
            List<Multipropietario> expectedOutput = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "125-2", PorcentajeDerecho = 3},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "132-5", PorcentajeDerecho = 6},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "144-9", PorcentajeDerecho = 21},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 30},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "6-K", PorcentajeDerecho = 40},
            };
            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].Comuna, actualOutput[item].Comuna);
                Assert.AreEqual(expectedOutput[item].Manzana, actualOutput[item].Manzana);
                Assert.AreEqual(expectedOutput[item].Predio, actualOutput[item].Predio);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
            }
        }

        [Test]
        public void TestMergeSameMultipropietariosPercentage()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);

            List<Multipropietario> multipropietariosToAdd = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 5},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 10},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 15},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 20},
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "6-K", PorcentajeDerecho = 25},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "6-K", PorcentajeDerecho = 25}
            };

            List<Multipropietario> actualOutput = controller.MergeSameMultipropietariosPercentage(multipropietariosToAdd);

            List<Multipropietario> expectedOutput = new List<Multipropietario>
            {
                new Multipropietario { Id = 1, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "2-7", PorcentajeDerecho = 15},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "1-9", PorcentajeDerecho = 35},
                new Multipropietario { Id = 2, Comuna = "Ancud", Manzana = "1", Predio = "1", RunRut = "6-K", PorcentajeDerecho = 50}

            };

            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].Comuna, actualOutput[item].Comuna);
                Assert.AreEqual(expectedOutput[item].Manzana, actualOutput[item].Manzana);
                Assert.AreEqual(expectedOutput[item].Predio, actualOutput[item].Predio);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
            }
        }

        [Test]
        public void TestGetEnajenanteCantidates()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            int numEnajenantes = 1;
            float percentagePerEna = 0;
            Formulario formulario = new Formulario { NumAtencion = 3, Cne = "Compraventa", Comuna = "Ancud", Manzana = "1", Predio = "1", Fojas = 99, NumInscripcion = 22, FechaInscripcion = new DateTime(2018, 6, 30, 0, 0, 0) };
            
            var dataFormHTML = new Dictionary<string, string>
            {
                { "enajenantes[0].run_rut", "2-7" },
                { "enajenantes[0].porcentaje_derecho", "30.0" },
                { "enajenantes[1].run_rut", "2-8" },
                { "enajenantes[1].porcentaje_derecho", "40.0" },
                { "enajenantes[1].no_acreditado", "true" }
                
            };

            List<Enajenante> actualOutput = controller.GetEnajenanteCantidates(numEnajenantes, formulario, percentagePerEna, dataFormHTML);
            
            List<Enajenante> expectedOutput = new List<Enajenante>
            {
                new Enajenante { Id = 1, NumAtencion = 3, RunRut = "2-7", PorcentajeDerecho = 30, NoAcreditado = false },
                new Enajenante { Id = 1, NumAtencion = 3, RunRut = "2-8", PorcentajeDerecho = 40, NoAcreditado = true },
            };
            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
                Assert.AreEqual(expectedOutput[item].NoAcreditado, actualOutput[item].NoAcreditado);
                Assert.AreEqual(expectedOutput[item].NumAtencion, actualOutput[item].NumAtencion);
            }
        }

        [Test]
        public void TestGetAdquirentesCantidates()
        {
            var context = new BbddverificableContext();
            var controller = new FormulariosController(context);
            int numAdquirentes = 1;
            float percentagePerEna = 0;
            Formulario formulario = new Formulario { NumAtencion = 3, Cne = "Compraventa", Comuna = "Ancud", Manzana = "1", Predio = "1", Fojas = 99, NumInscripcion = 22, FechaInscripcion = new DateTime(2018, 6, 30, 0, 0, 0) };

            var dataFormHTML = new Dictionary<string, string>
            {
                { "adquirentes[0].run_rut", "2-7" },
                { "adquirentes[0].porcentaje_derecho", "30.0" },
                { "adquirentes[1].run_rut", "2-8" },
                { "adquirentes[1].porcentaje_derecho", "40.0" },
                { "adquirentes[1].no_acreditado", "true" }
            };

            List<Adquirente> actualOutput = controller.GetAdquirienteCantidates(numAdquirentes, formulario, percentagePerEna, dataFormHTML);

            List<Adquirente> expectedOutput = new List<Adquirente>
            {
                new Adquirente { Id = 1, NumAtencion = 3, RunRut = "2-7", PorcentajeDerecho = 30, NoAcreditado = false },
                new Adquirente { Id = 1, NumAtencion = 3, RunRut = "2-8", PorcentajeDerecho = 40, NoAcreditado = true },
            };
            for (var item = 0; item < actualOutput.Count - 1; item++)
            {
                Console.WriteLine(actualOutput[item]);
                Assert.AreEqual(expectedOutput[item].RunRut, actualOutput[item].RunRut);
                Assert.AreEqual(expectedOutput[item].PorcentajeDerecho, actualOutput[item].PorcentajeDerecho);
                Assert.AreEqual(expectedOutput[item].NoAcreditado, actualOutput[item].NoAcreditado);
                Assert.AreEqual(expectedOutput[item].NumAtencion, actualOutput[item].NumAtencion);
            }
        }
    }
}